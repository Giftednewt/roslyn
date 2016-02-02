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
        private static readonly LocalizableString s_localizableTitle = new LocalizableResourceString(nameof(FeaturesResources.AddMissingCaseLabels), FeaturesResources.ResourceManager, typeof(FeaturesResources));
        private static readonly LocalizableString s_localizableMessage = new LocalizableResourceString(nameof(WorkspacesResources.CaseLabelsAreMissing), WorkspacesResources.ResourceManager, typeof(WorkspacesResources));

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

            var switchStatement = ((SwitchStatementSyntax)node);

            if(switchStatement.Sections.Count == 0)
            {
                return false;
            }

            var info = model.GetSymbolInfo(switchStatement.Expression, cancellationToken);
            
            if (info.Symbol == null)
            {
                return false;
            }

            var symbolType = info.Symbol.GetSymbolType();

            if(symbolType.TypeKind != TypeKind.Enum)
            {
                return false;
            }

            //Get all the labels defined for the switch
            if (LabelsExist(model, symbolType, switchStatement))
            {
                return false;
            }

            var tree = model.SyntaxTree;
            var span = switchStatement.Sections.Span;
            if (tree.OverlapsHiddenPosition(span, cancellationToken))
            {
                return false;
            }

            diagnostic = Diagnostic.Create(s_descriptor, tree.GetLocation(span));
            return true;
        }

        private bool LabelsExist(SemanticModel semanticModel, ITypeSymbol symbolType, SwitchStatementSyntax switchStatement)
        {
            var members = symbolType.GetMembers().OfType<IFieldSymbol>().Select(f=> f.ConstantValue).ToList();

            foreach (var value in switchStatement.Sections.SelectMany(s=> s.Labels.OfType<CaseSwitchLabelSyntax>().Select(l=> semanticModel.GetConstantValue(l.Value))))
            {
                members.Remove(value.Value);
            }

            return members.Count == 0;
        }

        private TextSpan GetDiagnosticSpan(SyntaxNode node)
        {
            var switchStatement = (SwitchStatementSyntax)node;
            return TextSpan.FromBounds(switchStatement.OpenParenToken.SpanStart, switchStatement.CloseParenToken.Span.End);
        }
    }
}
