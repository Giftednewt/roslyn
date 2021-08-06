﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.ErrorReporting;
using Microsoft.CodeAnalysis.Internal.Log;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Remote;
using Microsoft.CodeAnalysis.Serialization;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    internal partial class SolutionState
    {
        public bool TryGetStateChecksums([NotNullWhen(true)] out SolutionStateChecksums? stateChecksums)
            => _lazyChecksums.TryGetValue(out stateChecksums);

        public bool TryGetStateChecksums(ProjectId projectId, [NotNullWhen(true)] out SolutionStateChecksums? stateChecksums)
        {
            ValueSource<SolutionStateChecksums>? value;
            lock (_lazyProjectChecksums)
            {
                if (!_lazyProjectChecksums.TryGetValue(projectId, out value) ||
                    value == null)
                {
                    stateChecksums = null;
                    return false;
                }
            }

            return value.TryGetValue(out stateChecksums);
        }

        public Task<SolutionStateChecksums> GetStateChecksumsAsync(CancellationToken cancellationToken)
            => _lazyChecksums.GetValueAsync(cancellationToken);

        public async Task<Checksum> GetChecksumAsync(CancellationToken cancellationToken)
        {
            var collection = await GetStateChecksumsAsync(cancellationToken).ConfigureAwait(false);
            return collection.Checksum;
        }

        /// <param name="projectId">If specified, the checksum will only contain information about the 
        /// provided project (and any projects it depends on)</param>
        public async Task<SolutionStateChecksums> GetStateChecksumsAsync(ProjectId? projectId, CancellationToken cancellationToken)
        {
            if (projectId == null)
                return await GetStateChecksumsAsync(cancellationToken).ConfigureAwait(false);

            ValueSource<SolutionStateChecksums>? lazyChecksum;
            lock (_lazyProjectChecksums)
            {
                if (!_lazyProjectChecksums.TryGetValue(projectId, out lazyChecksum))
                {
                    lazyChecksum = CreateLazyChecksum(projectId);
                    _lazyProjectChecksums.Add(projectId, lazyChecksum);
                }
            }

            var collection = await lazyChecksum.GetValueAsync(cancellationToken).ConfigureAwait(false);
            return collection;

            // Extracted as a local function to prevent delegate allocations when not needed.
            ValueSource<SolutionStateChecksums> CreateLazyChecksum(ProjectId? projectId)
            {
                return new AsyncLazy<SolutionStateChecksums>(
                    c => ComputeChecksumsAsync(projectId, c), cacheResult: true);
            }
        }

        /// <param name="projectId">If specified, the checksum will only contain information about the 
        /// provided project (and any projects it depends on)</param>
        public async Task<Checksum> GetChecksumAsync(ProjectId? projectId, CancellationToken cancellationToken)
        {
            var checksums = await GetStateChecksumsAsync(projectId, cancellationToken).ConfigureAwait(false);
            return checksums.Checksum;
        }

        private async Task<SolutionStateChecksums> ComputeChecksumsAsync(
            ProjectId? projectId, CancellationToken cancellationToken)
        {
            try
            {
                using (Logger.LogBlock(FunctionId.SolutionState_ComputeChecksumsAsync, FilePath, cancellationToken))
                {
                    // get states by id order to have deterministic checksum.  Limit to the requested set of projects
                    // if applicable.
                    var orderedProjectIds = ChecksumCache.GetOrCreate(ProjectIds, _ => ProjectIds.OrderBy(id => id.Id).ToImmutableArray());
                    var projectsToInclude = GetProjectsToInclude(projectId);
                    var projectChecksumTasks = orderedProjectIds.Where(id => projectsToInclude == null || projectsToInclude.Contains(id))
                                                                .Select(id => ProjectStates[id])
                                                                .Where(s => RemoteSupportedLanguages.IsSupported(s.Language))
                                                                .Select(s => s.GetChecksumAsync(cancellationToken))
                                                                .ToArray();

                    var serializer = _solutionServices.Workspace.Services.GetRequiredService<ISerializerService>();
                    var attributesChecksum = serializer.CreateChecksum(SolutionAttributes, cancellationToken);

                    // If we ware only syncing a subset of projects over to the OOP side, we may only need to
                    // sync a subset of options as well.
                    var options = GetOptionsToSerialize(projectsToInclude);
                    var optionsChecksum = serializer.CreateChecksum(options, cancellationToken);

                    var frozenSourceGeneratedDocumentIdentityChecksum = Checksum.Null;
                    var frozenSourceGeneratedDocumentTextChecksum = Checksum.Null;

                    if (FrozenSourceGeneratedDocumentState != null)
                    {
                        frozenSourceGeneratedDocumentIdentityChecksum = serializer.CreateChecksum(FrozenSourceGeneratedDocumentState.Identity, cancellationToken);
                        frozenSourceGeneratedDocumentTextChecksum = (await FrozenSourceGeneratedDocumentState.GetStateChecksumsAsync(cancellationToken).ConfigureAwait(false)).Text;
                    }

                    var analyzerReferenceChecksums = ChecksumCache.GetOrCreate<ChecksumCollection>(AnalyzerReferences,
                        _ => new ChecksumCollection(AnalyzerReferences.Select(r => serializer.CreateChecksum(r, cancellationToken)).ToArray()));

                    var projectChecksums = await Task.WhenAll(projectChecksumTasks).ConfigureAwait(false);
                    return new SolutionStateChecksums(attributesChecksum, optionsChecksum, new ChecksumCollection(projectChecksums), analyzerReferenceChecksums, frozenSourceGeneratedDocumentIdentityChecksum, frozenSourceGeneratedDocumentTextChecksum);
                }
            }
            catch (Exception e) when (FatalError.ReportAndPropagateUnlessCanceled(e, cancellationToken))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private SerializableOptionSet GetOptionsToSerialize(HashSet<ProjectId>? projectsToInclude)
        {
            // we're syncing the entire solution, so sync all the options.
            if (projectsToInclude == null)
                return this.Options;

            // we're syncing a subset of projects, so only sync the options for the particular languages
            // we're syncing over.
            var languages = projectsToInclude.Select(id => ProjectStates[id].Language)
                                             .Where(s => RemoteSupportedLanguages.IsSupported(s))
                                             .ToImmutableHashSet();

            return this.Options.WithLanguages(languages);
        }

        private HashSet<ProjectId>? GetProjectsToInclude(ProjectId? projectId)
        {
            if (projectId == null)
                return null;

            var result = new HashSet<ProjectId>();
            AddReferencedProjects(result, projectId);
            return result;
        }

        private void AddReferencedProjects(HashSet<ProjectId> result, ProjectId projectId)
        {
            if (!result.Add(projectId))
                return;

            var projectState = this.GetProjectState(projectId);
            if (projectState == null)
                return;

            foreach (var refProject in projectState.ProjectReferences)
                AddReferencedProjects(result, refProject.ProjectId);
        }
    }
}
