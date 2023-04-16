using Microsoft.CodeAnalysis.CodeFixes;
using System.Collections.Immutable;

namespace Nullness.Bang.Tests.utility
{
    public sealed class EmptyCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray<string>.Empty;

        public override Task RegisterCodeFixesAsync(CodeFixContext context) =>
            Task.FromResult(true);
    }
}
