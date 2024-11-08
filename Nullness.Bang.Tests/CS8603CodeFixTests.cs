using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nullness.Bang.Tests.utility;

namespace Nullness.Bang.Tests
{
    [TestClass]
    public class CS8603CodeFixTests
    {

        [TestMethod]
        public async Task CS8603CodeFixTests_UpdateReturnTypeToNullable()
        {
            var testhost = new BangCodeFixTestHost()
            {
                TestCode = """ 
                #nullable enable
                public class X {
                    public string demo()
                    {
                        return null;
                    } 
                }
                """,
                FixedCode = """ 
                #nullable enable
                public class X {
                    public string? demo()
                    {
                        return null;
                    } 
                }
                """,
            };

            testhost.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerWarning("CS8603").WithSpan(5, 16, 5, 20)
            );

            await testhost.RunAsync();
        }
    }
}
