using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.CSharp.CodeFixes.AddMissingSwitchLabels
{
    internal partial class AddMissingSwitchLabelsCodeFixProvider
    {
        private class AddMissingLabelsAction : CodeActions.CodeAction
        {
            private Document _document;
            private SwitchStatementSyntax _node;

            public override string Title
            {
                get
                {
                    return CSharpFeaturesResources.AddMissingCaseLabels;
                }
            }

            public AddMissingLabelsAction(Document document, SwitchStatementSyntax node)
            {
                _document = document;
                _node = node;
            }

            protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
            {
                var root = await _document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                var newNode = await GetNewNode(_document, _node, cancellationToken).ConfigureAwait(false);
                var newRoot = root.ReplaceNode(_node, newNode);

                return _document.WithSyntaxRoot(newRoot);
            }

            private async Task<SyntaxNode> GetNewNode(Document document, SwitchStatementSyntax node, CancellationToken cancellationToken)
            {
                var missingLabels = await GetMissingLabels(node, cancellationToken).ConfigureAwait(false);
                SyntaxNode newNode = node.AddSections(missingLabels);

                return newNode.WithAdditionalAnnotations(Formatter.Annotation);
            }

            private async Task<SwitchSectionSyntax> GetMissingLabels(SwitchStatementSyntax node, CancellationToken cancellationToken)
            {
                var semanticModel = await _document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                var symbol = semanticModel.GetSymbolInfo(node.Expression);

                var type = symbol.Symbol.GetSymbolType();

                var members = type.GetMembers().OfType<IFieldSymbol>().ToDictionary(k => k.ConstantValue, f => SyntaxFactory.CaseSwitchLabel(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(type.Name), SyntaxFactory.IdentifierName(f.Name))));

                foreach (var label in node.Sections.SelectMany(x=> x.Labels.OfType<CaseSwitchLabelSyntax>()))
                {
                    var labelValueSymbol = semanticModel.GetSymbolInfo(label.Value,cancellationToken);
                    var fieldSymbol = labelValueSymbol.Symbol as IFieldSymbol;
                    if(members.ContainsKey(fieldSymbol.ConstantValue))
                    {
                        members.Remove(fieldSymbol.ConstantValue);
                    }
                }

                //members.Except(switchStatement.Sections.SelectMany(s => s.Labels.OfType<CaseSwitchLabelSyntax>().Select(l => semanticModel.GetConstantValue(l.Value))));
                return SyntaxFactory.SwitchSection(SyntaxFactory.List<SwitchLabelSyntax>(members.Values), SyntaxFactory.List<StatementSyntax>(new[] { SyntaxFactory.BreakStatement() }));
            }
        }

    }
}
