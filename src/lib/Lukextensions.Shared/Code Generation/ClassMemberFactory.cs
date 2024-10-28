using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Lukextensions.Shared
{
    public static class ClassMemberFactory
    {

        public static FieldDeclarationSyntax StringField(string fieldName, string value = null, bool isPublic = true, bool isConstant = false)
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

        public static PropertyDeclarationSyntax Property(string type, string propertyName)
        {
            return SyntaxFactory.PropertyDeclaration(
                attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                type: SyntaxFactory.IdentifierName(type),
                explicitInterfaceSpecifier: null,
                identifier: SyntaxFactory.Identifier(propertyName),
                accessorList: SyntaxFactory.AccessorList(
                    SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    SyntaxFactory.List(new List<AccessorDeclarationSyntax>()
                    {
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    }),
                    SyntaxFactory.Token(SyntaxKind.CloseBraceToken)));
        }
    }
}
