﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.LanguageServer.Handler.SemanticTokens;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace Microsoft.CodeAnalysis.ExternalAccess.Razor
{
    internal class RazorSemanticTokensAccessor
    {
        [Obsolete("Use GetTokenTypes")]
        public static ImmutableArray<string> RoslynTokenTypes => SemanticTokensHelpers.LegacyGetAllTokenTypesForRazor();

        public static ImmutableArray<string> GetTokenTypes(ClientCapabilities capabilities) => SemanticTokensHelpers.GetAllTokenTypes(capabilities);
    }
}
