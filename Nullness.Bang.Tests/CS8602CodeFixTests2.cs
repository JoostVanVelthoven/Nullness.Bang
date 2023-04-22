using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nullness.Bang.Tests.utility;

namespace Nullness.Bang.Tests
{
    [TestClass]
    public class CS8602Code2FixTests
    {
        [TestMethod]
        public async Task CS8602CodeFixTests_Main()
        {
            var testhost = new CS8602CodeFixTestHost()
            {
                TestCode = """ 
                #nullable enable
                public class X {
                    public string method(string? input)
                    {
                        return input;
                    } 
                }
                """,
            };

            testhost.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerWarning("CS8603").WithSpan(5, 13, 5, 14)
            );

            await testhost.RunAsync();
        }
    }
}

