using System.Threading.Tasks;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<AnnotationAnalyzer.NoManualSetAnalyzer>;

namespace AnnotationAnalyzer.Tests;

public class NoManualSetAnalyzer
{
    [Fact]
    public async Task Set_Injected_Fields_Should_Be_Detected()
    {
        const string code = """
                            using System;

                            [AttributeUsage(AttributeTargets.Field)]
                            public class NoManualSetAttribute : Attribute
                            {
                            }

                            public class Examples
                            {
                                [NoManualSet] private int _blabla;
                                public Examples() => _blabla = 5;
                                public void SetBlabla(int value) => _blabla = value;
                            }
                            """;

        var expected1 = Verifier.Diagnostic()
            .WithSpan(12, 41, 12, 56)
            .WithArguments("_blabla");

        var expected2 = Verifier.Diagnostic()
            .WithSpan(11, 26, 11, 37)
            .WithArguments("_blabla");

        await Verifier.VerifyAnalyzerAsync(code, expected1, expected2).ConfigureAwait(false);
    }
}