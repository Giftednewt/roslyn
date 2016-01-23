using Microsoft.CodeAnalysis.Formatting;
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
            private SyntaxNode _node;

            public override string Title
            {
                get
                {
                    return CSharpFeaturesResources.AddMissingCaseLabels;
                }
            }

            public AddMissingLabelsAction(Document document, SyntaxNode node)
            {
                _document = document;
                _node = node;
            }

            protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
            {
                var root = await _document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                var newNode = GetNewNode(_document, _node, cancellationToken);
                var newRoot = root.ReplaceNode(_node, newNode);

                return _document.WithSyntaxRoot(newRoot);
            }

            private SyntaxNode GetNewNode(Document document, SyntaxNode node, CancellationToken cancellationToken)
            {
                SyntaxNode newNode = node;

                return newNode.WithAdditionalAnnotations(Formatter.Annotation);
            }
        }

    }
}
