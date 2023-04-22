using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Nullness.Bang.Tests.utility
{
    public class CS8602CodeFixTestHost : CSharpCodeFixTest<DummyAnalyzer, EmptyCodeFixProvider, MSTestVerifier>
    {
      

        protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
        {
            yield return new CS8602CodeFixProvider();
        }
    }
}
