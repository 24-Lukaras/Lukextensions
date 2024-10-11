using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Lukextensions.Common
{
    public interface ICompilationProvider
    {
        CSharpCompilation GetCompilation();
        CSharpCompilation GetCompilationWith(IEnumerable<SyntaxTree> syntaxTrees);
    }
}
