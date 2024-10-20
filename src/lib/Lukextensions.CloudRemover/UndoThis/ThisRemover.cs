using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Lukextensions.CloudRemover
{
    public class ThisRemover
    {
        private readonly SyntaxTree _original;
        private SyntaxNode modified;
        public ThisRemover(string content)
        {
            _original = CSharpSyntaxTree.ParseText(content);
        }

        public string UndoThis()
        {
            modified = _original.GetRoot();
            var classes = modified.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .ToArray();

            foreach (var c in classes) 
            {
                ProcessClass(c);                
            }

            return modified
                .NormalizeWhitespace()
                .ToString();
        }

        private void ProcessClass(ClassDeclarationSyntax classDeclaration)
        {
            var constructors = classDeclaration.DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .ToArray();

            var privateReadonlyFields = classDeclaration.DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .Where(x => x.Declaration.Variables.Count == 1 && x.Modifiers.Any(y => y.IsKind(SyntaxKind.PrivateKeyword)) && x.Modifiers.Any(y => y.IsKind(SyntaxKind.ReadOnlyKeyword)));

            Dictionary<SyntaxNode, SyntaxNode> nodesToReplace = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var field in privateReadonlyFields)
            {
                string originalFieldName = field.Declaration.Variables.First().Identifier.ToString();
                string fieldName = $"_{originalFieldName}";

                nodesToReplace.Add(field.Declaration.Variables.First(), field.Declaration.Variables.First().WithIdentifier(SyntaxFactory.Identifier(fieldName)));

                foreach (var constructor in constructors)
                {
                    var assignments = constructor.DescendantNodes()
                        .OfType<AssignmentExpressionSyntax>()
                        .Where(x => x.IsKind(SyntaxKind.SimpleAssignmentExpression) &&
                            (                                
                                (x.Left is MemberAccessExpressionSyntax memberAccess 
                                && memberAccess.Expression.IsKind(SyntaxKind.ThisExpression)
                                && memberAccess.Name.ToString() == originalFieldName)
                                || 
                                (x.Left is IdentifierNameSyntax identifierName
                                && identifierName.Identifier.ToString() == originalFieldName)
                            )
                        );

                    foreach (var assignment in assignments)
                    {
                        nodesToReplace.Add(assignment.Left, SyntaxFactory.IdentifierName(fieldName));
                    }
                }

                var memberAccesses = classDeclaration.DescendantNodes()
                    .OfType<MemberAccessExpressionSyntax>()
                    .Where(x => x.Expression is IdentifierNameSyntax identifierName && identifierName.Identifier.ToString() == originalFieldName && !nodesToReplace.ContainsKey(x));
                foreach (var memberAccess in memberAccesses)
                {
                    nodesToReplace.Add(memberAccess, memberAccess.WithExpression(SyntaxFactory.IdentifierName(fieldName)));
                }
                
                var identifiers = classDeclaration.DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Where(x => x.Identifier.ToString() == originalFieldName && !nodesToReplace.ContainsKey(x));
                foreach (var identifier in identifiers)
                {
                    nodesToReplace.Add(identifier, SyntaxFactory.IdentifierName(fieldName));
                }                                
            }

            modified = modified.ReplaceNodes(nodesToReplace.Keys, (node, _) => nodesToReplace[node]);
        }


    }
}
