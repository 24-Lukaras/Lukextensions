using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lukextensions.Common
{
    public class MapperMethodDefinition
    {
        public string MethodName { get; set; }
        public string SourceType { get; set; }
        public string ResultType { get; set; }

        public List<PropertyDefinition> Properties { get; set; }
        public HashSet<string> RequiredNamespaces { get; private set; } = new HashSet<string>();

        public MapperMethodDefinition(UsingDirectiveSyntax usingDirective, SemanticModel model)
        {
            MethodName = usingDirective.Alias.Name.ToString();

            var tuppleAlias = usingDirective.NamespaceOrType as TupleTypeSyntax;
            var sourceType = tuppleAlias.Elements[0].Type;
            SourceType = sourceType.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.Identifier.ValueText;
            TryAddRequiredNamespace(sourceType);
            var sourceInfo = model.GetTypeInfo(sourceType);
            var gettableProperties = sourceInfo.Type.GetMembers()
                .OfType<IPropertySymbol>()
                .Select(x => new PropertyDefinition(x))
                .ToList();

            var resultType = tuppleAlias.Elements[1].Type;
            ResultType = resultType.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.Identifier.ValueText;
            TryAddRequiredNamespace(resultType);
            var resultInfo = model.GetTypeInfo(resultType);
            var settableProperties = resultInfo.Type.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => x.SetMethod != null)
                .Select(x => new PropertyDefinition(x))
                .ToList();

            Properties = gettableProperties.Intersect(settableProperties, new PropertyDefinitionEqualityComparer()).ToList();
        }

        public static bool IsUsingDirectiveSuitable(UsingDirectiveSyntax usingDirective) =>
            usingDirective.Alias != null && usingDirective.NamespaceOrType is TupleTypeSyntax tuple && tuple.Elements.Count == 2;

        private void TryAddRequiredNamespace(TypeSyntax typeSyntax)
        {
            var qualifiedNames = typeSyntax.ChildNodes().OfType<QualifiedNameSyntax>();
            foreach (var qualifiedName in qualifiedNames)
            {
                RequiredNamespaces.Add(qualifiedName.ToString());
            }
        }

        public MethodDeclarationSyntax CreateMethod()
        {
            var parameters = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(new List<ParameterSyntax> {
                        SyntaxFactory.Parameter(
                            attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                            modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)),
                            type: SyntaxFactory.ParseTypeName(SourceType),
                            identifier: SyntaxFactory.Identifier("source"),
                            @default: null
                        )
                    }
                )
            );


            List<SyntaxNodeOrToken> propertyAssignmentList = new List<SyntaxNodeOrToken>();
            for (int i = 0; i < Properties.Count; i++)
            {
                var property = Properties[i];
                propertyAssignmentList.Add(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName(property.PropertyName),
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("source"),
                        SyntaxFactory.IdentifierName(property.PropertyName)
                        )
                    )
                );
                if (i < Properties.Count - 1)
                {
                    propertyAssignmentList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                }
            }

            var methodBlock = SyntaxFactory.Block(
                    SyntaxFactory.SingletonList<StatementSyntax>(
                            SyntaxFactory.ReturnStatement(
                                SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(ResultType))
                                    .WithInitializer(
                                        SyntaxFactory.InitializerExpression(
                                            SyntaxKind.ObjectInitializerExpression,
                                            SyntaxFactory.SeparatedList<ExpressionSyntax>(propertyAssignmentList)
                                    )
                                )
                            )
                        )
                    );

            return SyntaxFactory.MethodDeclaration(
                attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                returnType: SyntaxFactory.ParseTypeName(ResultType),
                explicitInterfaceSpecifier: null,
                identifier: SyntaxFactory.Identifier(MethodName),
                typeParameterList: null,
                parameterList: parameters,
                constraintClauses: SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
                body: methodBlock,
                semicolonToken: SyntaxFactory.Token(SyntaxKind.None)
            );
        }
    }
}
