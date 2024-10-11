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
                .Create(null, GetTrees(_rootPath));
        }

        public CSharpCompilation GetCompilationWith(IEnumerable<SyntaxTree> syntaxTrees)
        {
            return CSharpCompilation
                .Create(null, GetTrees(_rootPath))
                .AddSyntaxTrees(syntaxTrees);
        }

        private List<SyntaxTree> GetTrees(string path)
        {
            var result = new List<SyntaxTree>();

            FillTrees(new DirectoryInfo(path), result);

            return result;
        }

        private void FillTrees(DirectoryInfo directory, List<SyntaxTree> trees)
        {
            var files = directory.GetFiles("*.cs");
            foreach (var file in files)
            {
                using (StreamReader reader = new StreamReader(file.OpenRead()))
                {
                    trees.Add(CSharpSyntaxTree.ParseText(reader.ReadToEnd()));
                }
            }

            var directories = directory.GetDirectories();
            foreach (var d in directories)
            {
                FillTrees(d, trees);
            }
        }        
    }
}
