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

namespace Nullness.Bang
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BangOperatorFixer)), Shared]
    public class BangOperatorFixer : Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider
    {
        private const string Title = "Add null-forgiving operator";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create("CS8602", "CS8603"); }
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

            if (diagnostic.Id == "CS8602")
            {
                // Check if null-forgiving operator is already present
                var memberAccess = token.Parent
                    .AncestorsAndSelf()
                    .TakeWhile(x =>
                    {
                        if (x is PostfixUnaryExpressionSyntax p)
                        {
                            return p.Kind() != SyntaxKind.SuppressNullableWarningExpression;
                        }
                        return true;
                    })
                    .OfType<MemberAccessExpressionSyntax>()
                    .LastOrDefault();

                if (memberAccess == null)
                {
                    return;
                }

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
            else if (diagnostic.Id == "CS8603")
            {
                var methodDeclaration = token.Parent
                    .AncestorsAndSelf()
                    .OfType<MethodDeclarationSyntax>()
                    .FirstOrDefault();

                if (methodDeclaration == null)
                {
                    return;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Update return type to nullable",
                        createChangedDocument: cancellationToken =>
                            UpdateReturnTypeToNullableAsync(context.Document, methodDeclaration, cancellationToken),
                        equivalenceKey: "UpdateReturnTypeToNullable"
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

        private async Task<Document> UpdateReturnTypeToNullableAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken
        )
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var newReturnType = SyntaxFactory.NullableType(methodDeclaration.ReturnType);

            var newMethodDeclaration = methodDeclaration.WithReturnType(newReturnType);

            var newRoot = root.ReplaceNode(methodDeclaration, newMethodDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
