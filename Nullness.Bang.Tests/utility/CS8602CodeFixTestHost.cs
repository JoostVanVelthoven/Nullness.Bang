using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Nullness.Bang.Tests.utility
{
    public class CS8602CodeFixTestHost : CSharpCodeFixTest<DummyAnalyzer, EmptyCodeFixProvider, MSTestVerifier>
    {
        public CS8602CodeFixTestHost()
        {
            //ReferenceAssemblies = CodeAnalyzerHelper.CurrentXunitV2;

            // xunit diagnostics are reported in both normal and generated code
            TestBehaviors |= TestBehaviors.SkipGeneratedCodeCheck;
        }

        protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
        {
            yield return new CS8602CodeFixProvider();
        }
    }
}
