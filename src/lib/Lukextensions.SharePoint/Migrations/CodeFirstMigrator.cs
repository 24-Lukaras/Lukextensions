using Lukextensions.SharePoint.Requests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lukextensions.SharePoint
{
    public class CodeFirstMigrator
    {

        private readonly GraphClient _client;
        public CodeFirstMigrator(GraphClient client)
        {
            _client = client;
        }

        public async Task<string> MigrateAsync(string fileContents, string siteId)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContents);
            var root = await tree.GetRootAsync();

            var @class = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();

            if (@class == null)
                return null;

            var allMembers = @class.Members;

            var properties = allMembers
                .OfType<PropertyDeclarationSyntax>();

            NewListDefinition listDefinition = new NewListDefinition()
            {
                DisplayName = @class.Identifier.ToString()
            };

            foreach (var property in properties)
            {
                string columnName = property.Identifier.ValueText;

                if (columnName == "Title" || columnName == "Id")
                    continue;

                if (!property.AccessorList.Accessors.Any(x => x.IsKind(SyntaxKind.SetAccessorDeclaration)))
                    continue;

                switch (property.Type.ToString())
                {
                    case "string":
                        listDefinition.Columns.Add(ColumnDefinition.TextColumn(columnName));
                        break;

                    case "decimal":
                    case "double":
                    case "float":
                        listDefinition.Columns.Add(ColumnDefinition.NumberColumn(columnName));
                        break;

                    case "int":
                    case "int?":
                        listDefinition.Columns.Add(ColumnDefinition.IntColumn(columnName));
                        break;

                    case "DateTime":
                    case "DateTime?":
                        listDefinition.Columns.Add(ColumnDefinition.DateTimeColumn(columnName));
                        break;

                    case "DateOnly":
                    case "DateOnly?":
                        listDefinition.Columns.Add(ColumnDefinition.DateColumn(columnName));
                        break;

                    case "bool":
                    case "bool?":
                        listDefinition.Columns.Add(ColumnDefinition.BoolColumn(columnName));
                        break;

                    default:
                        break;
                }                
            }

            var result = await _client.Request(new CreateListRequest(siteId, listDefinition));

            allMembers = allMembers.InsertRange(0, new List<MemberDeclarationSyntax>() 
                {
                    SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                            .WithVariables(SyntaxFactory.SeparatedList(
                                new List<VariableDeclaratorSyntax>() {
                                    SyntaxFactory.VariableDeclarator("SiteId")
                                        .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(siteId)))
                                    )
                                }
                            )                            
                        )
                    ).WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    ,
                    SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                            .WithVariables(SyntaxFactory.SeparatedList(
                                new List<VariableDeclaratorSyntax>() {
                                    SyntaxFactory.VariableDeclarator("ListId")
                                        .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(result.Id)))
                                    )
                                }
                            )
                        )
                    ).WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                }
            );

            var newClass = @class.WithMembers(allMembers);

            return root.ReplaceNode(@class, newClass)
                .NormalizeWhitespace()
                .ToString();
        }

    }
}
