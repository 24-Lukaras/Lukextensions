using Lukextensions.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.IO;

namespace Lukextensions.Common
{
    public class RecursiveCompilationProvider : ICompilationProvider
    {
        private readonly string _rootPath;
        public RecursiveCompilationProvider(string rootPath)
        {
            _rootPath = Path.GetDirectoryName(rootPath);
        }

        public CSharpCompilation GetCompilation()
        {
            return CSharpCompilation
                .Create(null, FilesHelper.GetTrees(_rootPath));
        }

        public CSharpCompilation GetCompilationWith(IEnumerable<SyntaxTree> syntaxTrees)
        {
            return CSharpCompilation
                .Create(null, FilesHelper.GetTrees(_rootPath))
                .AddSyntaxTrees(syntaxTrees);
        }                
    }
}
