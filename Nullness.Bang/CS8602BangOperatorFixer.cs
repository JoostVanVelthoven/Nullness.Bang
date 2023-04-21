using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Nullness.Bang
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CS8602CodeFixProvider)), Shared]
    public class CS8602CodeFixProvider : CodeFixProvider
    {
        private const string Title = "Add null-forgiving operator";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create("CS8602"); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var token = root.FindToken(diagnosticSpan.Start);
            var memberAccess = token.Parent
                .AncestorsAndSelf()
                .OfType<MemberAccessExpressionSyntax>()
                .Last();

            // Check if null-forgiving operator is already present
            var hasNullForgivingOperator = token.Parent
                .AncestorsAndSelf()
                .OfType<PostfixUnaryExpressionSyntax>()
                .Any(p => p.Kind() == SyntaxKind.SuppressNullableWarningExpression);

            if (!hasNullForgivingOperator)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Add null-forgiving operator",
                        createChangedDocument: cancellationToken =>
                            AddBangOperatorAsync(context.Document, memberAccess, cancellationToken),
                        equivalenceKey: "AddNullForgivingOperator"
                    ),
                    diagnostic
                );
            }
        }

        private async Task<Document> AddBangOperatorAsync(
            Document document,
            MemberAccessExpressionSyntax memberAccess,
            CancellationToken cancellationToken
        )
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var newMemberAccess = memberAccess
                .WithExpression(
                    SyntaxFactory.PostfixUnaryExpression(
                        SyntaxKind.SuppressNullableWarningExpression,
                        memberAccess.Expression
                    )
                )
                .WithTriviaFrom(memberAccess);

            var newRoot = root.ReplaceNode(memberAccess, newMemberAccess);

            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> AddNullForgivingOperatorAsync(
            Document document,
            ArgumentSyntax argumentSyntax,
            CancellationToken cancellationToken
        )
        {
            var editor = await DocumentEditor
                .CreateAsync(document, cancellationToken)
                .ConfigureAwait(false);
            var newArgumentSyntax = argumentSyntax.WithExpression(
                SyntaxFactory.PostfixUnaryExpression(
                    SyntaxKind.SuppressNullableWarningExpression,
                    argumentSyntax.Expression
                )
            );
            editor.ReplaceNode(argumentSyntax, newArgumentSyntax);

            return editor.GetChangedDocument();
        }
    }
}
