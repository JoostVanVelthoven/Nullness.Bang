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
            var testhost = new BangCodeFixTestHost()
            {
                TestCode = """ 
                #nullable enable
                public class X {
                    public void nulla(string? x)
                    {
                        _ = x.ToLower(System.Globalization.CultureInfo.CurrentCulture);
                    } 
                }
                """,
                FixedCode = """ 
                #nullable enable
                public class X {
                    public void nulla(string? x)
                    {
                        _ = x!.ToLower(System.Globalization.CultureInfo.CurrentCulture);
                    } 
                }
                """,
            };

            testhost.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerWarning("CS8602").WithSpan(5, 13, 5, 14)
            );

            await testhost.RunAsync();
        }

        [TestMethod]
        public async Task CS8602CodeFixTests_Multi_props()
        {
            var testhost = new BangCodeFixTestHost()
            {
                TestCode = """ 
                #nullable enable
                public class MyClass
                {

                    public string? MyProperty { get; set; }

                    public MyClass MyClassProp => this;
                }

                public class X
                {
                    public void run()
                    {
                        new MyClass().MyClassProp.MyProperty.ToLower();
                    } 
                }

                """,
                FixedCode = """ 
                #nullable enable
                public class MyClass
                {
                
                    public string? MyProperty { get; set; }
                
                    public MyClass MyClassProp => this;
                }
                
                public class X
                {
                    public void run()
                    {
                        new MyClass().MyClassProp.MyProperty!.ToLower();
                    } 
                }
                
                """,
            };

            testhost.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerWarning("CS8602").WithSpan(14, 9, 14, 45)
            );

            await testhost.RunAsync();
        }

        [TestMethod]
        public async Task CS8602CodeFixTests_Multi_Null_Main()
        {
            var testhost = new BangCodeFixTestHost()
            {
                TestCode = """ 
                #nullable enable
                public class MyClass
                {

                    public string? MyProperty { get; set; }

                    public MyClass MyClassProp => this;

                    public MyClass? MyClassNullableProp => this;

                }

                public class X
                {
                    public void run()
                    {
                        new MyClass().MyClassProp.MyClassNullableProp.MyProperty.ToLower();
                    } 
                }

                """,
                FixedCode = """ 
                #nullable enable
                public class MyClass
                {
                
                    public string? MyProperty { get; set; }
                
                    public MyClass MyClassProp => this;
                
                    public MyClass? MyClassNullableProp => this;
                
                }
                
                public class X
                {
                    public void run()
                    {
                        new MyClass().MyClassProp.MyClassNullableProp!.MyProperty!.ToLower();
                    } 
                }
                
                """,
            };

            testhost.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerWarning("CS8602").WithSpan(17, 9, 17, 65)
            );
            testhost.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerWarning("CS8602").WithSpan(17, 9, 17, 54)
            );

            testhost.NumberOfFixAllIterations = 2;      

            await testhost.RunAsync();
        }

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
