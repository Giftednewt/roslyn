using Microsoft.CodeAnalysis.CodeFixes;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.CodeFixes.AddMissingSwitchLabels
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = PredefinedCodeFixProviderNames.AddMissingCaseLabels), Shared]
    internal partial class AddMissingSwitchLabelsCodeFixProvider : CodeFixProvider
    {
        internal const string IDE0006 = "IDE0006"; // 'SomeClass.SomeMember' hides inherited member 'SomeClass.SomeMember'. Use the new keyword if hiding was intended.

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(IDE0006);


        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            context.RegisterCodeFix(new AddMissingLabelsAction(context.Document, root), context.Diagnostics);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
        }
    }
}
