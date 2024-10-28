using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Lukextensions.Shared
{
    public static class ClassMemberFactory
    {

        public static MemberDeclarationSyntax StringField(string fieldName, string value = null, bool isPublic = true, bool isConstant = false)
        {
            var modifiers = SyntaxFactory.TokenList();
            if (isPublic)
            {
                modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            }
            if (isConstant)
            {
                modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ConstKeyword));
            }

            var variableDeclarator = SyntaxFactory.VariableDeclarator(fieldName);
            if (value != null)
            {
                variableDeclarator = variableDeclarator.WithInitializer(
                    SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value))
                    )
                );
            }

            return SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                    .WithVariables(SyntaxFactory.SeparatedList(
                        new List<VariableDeclaratorSyntax>() {
                            variableDeclarator
                        }
                    )
                )
            ).WithModifiers(modifiers);
        }
    }
}
