using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Diagnostics.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class CSharpMissingSwitchLabelsDiagnosticAnalyzer : DiagnosticAnalyzer, IBuiltInAnalyzer
    {
        private static readonly LocalizableString s_localizableTitle = new LocalizableResourceString(nameof(FeaturesResources.RemoveUnnecessaryCast), FeaturesResources.ResourceManager, typeof(FeaturesResources));
        private static readonly LocalizableString s_localizableMessage = new LocalizableResourceString(nameof(WorkspacesResources.CastIsRedundant), WorkspacesResources.ResourceManager, typeof(WorkspacesResources));

        private static readonly DiagnosticDescriptor s_descriptor = new DiagnosticDescriptor(IDEDiagnosticIds.MissingSwitchLabelsDiagnosticId,
                                                                            s_localizableTitle,
                                                                            s_localizableMessage,
                                                                            DiagnosticCategory.Style,
                                                                            DiagnosticSeverity.Hidden,
                                                                            isEnabledByDefault: false,
                                                                            customTags: DiagnosticCustomTags.Unnecessary);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(s_descriptor);

        public DiagnosticAnalyzerCategory GetAnalyzerCategory()
        {
            return DiagnosticAnalyzerCategory.SemanticSpanAnalysis;
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                (nodeContext) =>
                {
                    Diagnostic diagnostic;
                    if (HasMissingLabels(nodeContext.SemanticModel, nodeContext.Node, out diagnostic, nodeContext.CancellationToken))
                    {
                        nodeContext.ReportDiagnostic(diagnostic);
                    }
                },
                ImmutableArray.Create(SyntaxKind.SwitchStatement));
        }

        private bool HasMissingLabels(
            SemanticModel model, SyntaxNode node, out Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            diagnostic = default(Diagnostic);

            var tree = model.SyntaxTree;
            var span = GetDiagnosticSpan(node);
            if (tree.OverlapsHiddenPosition(span, cancellationToken))
            {
                return false;
            }

            diagnostic = Diagnostic.Create(s_descriptor, tree.GetLocation(span));
            return true;
        }

        private TextSpan GetDiagnosticSpan(SyntaxNode node)
        {
            var switchStatement = (SwitchStatementSyntax)node;
            return TextSpan.FromBounds(switchStatement.OpenParenToken.SpanStart, switchStatement.CloseParenToken.Span.End);
        }
    }
}
