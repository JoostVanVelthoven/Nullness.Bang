using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Nullness.Bang.Tests.utility
{
    public class BangCodeFixTestHost : CSharpCodeFixTest<DummyAnalyzer, EmptyCodeFixProvider, MSTestVerifier>
    {
      

        protected override IEnumerable<Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider> GetCodeFixProviders()
        {
            yield return new BangOperatorFixer();
        }
    }
}
