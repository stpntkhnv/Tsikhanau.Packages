using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Tsikhanau.Railway.Analyzers.Tests;

internal static class Verifier
{
    private const String Usings = """
        using System;
        using System.Collections.Generic;
        using System.Diagnostics;
        using System.Linq;
        using System.Threading.Tasks;
        using Tsikhanau.Railway;

        """;

    public static Task AnalyzeAsync<TAnalyzer>(String source)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            TestCode = Usings + source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100
        };
        test.TestState.AdditionalReferences.Add(typeof(Result<>).Assembly);

        return test.RunAsync(TestContext.Current.CancellationToken);
    }

    public static Task FixAsync<TAnalyzer, TCodeFix>(String source, String fixedSource)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        var test = new CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
        {
            TestCode = Usings + source,
            FixedCode = Usings + fixedSource,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100
        };
        test.TestState.AdditionalReferences.Add(typeof(Result<>).Assembly);
        test.FixedState.AdditionalReferences.Add(typeof(Result<>).Assembly);

        return test.RunAsync(TestContext.Current.CancellationToken);
    }
}
