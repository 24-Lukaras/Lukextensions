using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lukextensions.Common
{
    public class MapperBuilder
    {

        private readonly SyntaxTree _syntaxTree;
        private readonly ICompilationProvider _compilationProvider;
        public MapperBuilder(string content, ICompilationProvider compilationProvider)
        {
            _syntaxTree = CSharpSyntaxTree.ParseText(content);
            _compilationProvider = compilationProvider;
        }

        public async Task<string> Process()
        {           
            var root = await _syntaxTree.GetRootAsync();

            var tupleAliases = root.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Where(MapperMethodDefinition.IsUsingDirectiveSuitable)
                .ToList();

            var compilation = _compilationProvider.GetCompilationWith(new List<SyntaxTree>() { _syntaxTree });
            var model = compilation.GetSemanticModel(_syntaxTree);

            var methodDefinitions = tupleAliases.Select(x => new MapperMethodDefinition(x, model)).ToList();

            var newRoot = root.RemoveNodes(tupleAliases, SyntaxRemoveOptions.KeepNoTrivia);

            var mapperClass = newRoot.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();

            var modifiedClass = mapperClass.AddMembers(methodDefinitions.Select(x => x.CreateMethod()).ToArray());
            if (modifiedClass.Modifiers.Any(x => !x.IsKind(SyntaxKind.StaticKeyword)))
            {
                modifiedClass = modifiedClass.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            }

            newRoot = newRoot.ReplaceNode(mapperClass, modifiedClass);
            newRoot = newRoot.NormalizeWhitespace();

            var result = SyntaxFactory.SyntaxTree(newRoot);
            return result.ToString();
        }     
    }
}
