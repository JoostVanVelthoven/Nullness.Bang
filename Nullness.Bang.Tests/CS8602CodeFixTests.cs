using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nullness.Bang.Tests.utility;

namespace Nullness.Bang.Tests
{
    [TestClass]
    public class CS8602CodeFixTests
    {
        [TestMethod]
        public async Task CS8602CodeFixTests_Main()
        {
            var testhost = new CS8602CodeFixTestHost()
            {
                TestCode = """ 
                #nullable enable
                public class X {
                    public void nulla(string? x)
                {
                    _ = x.ToLower(System.Globalization.CultureInfo.CurrentCulture);
                } }
                """,
                FixedCode = """ 
                #nullable enable
                public class X {
                    public void nulla(string? x)
                {
                    _ = x!.ToLower(System.Globalization.CultureInfo.CurrentCulture);
                } }
                """,
            };

            testhost.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerWarning("CS8602").WithSpan(5, 9, 5, 10)
            );

            await testhost.RunAsync();
        }
    }
}
