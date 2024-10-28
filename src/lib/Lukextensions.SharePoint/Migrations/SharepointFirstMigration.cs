using Lukextensions.Shared;
using Lukextensions.SharePoint.Requests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lukextensions.SharePoint
{
    public class SharepointFirstMigration
    {        
        private readonly GraphClient _client;
        public SharepointFirstMigration(GraphClient client)
        {
            _client = client;
        }

        public async Task<string> MigrateAsync(string fileContent, string searchPhrase, string siteId)
        {
            string listGuid = searchPhrase;

            if (!Guid.TryParse(searchPhrase, out _))
            {
                var lists = await _client.Request(new SearchForListsRequest(siteId));

                var list = lists.Items.FirstOrDefault(x => x.Name == searchPhrase || x.DisplayName == searchPhrase);

                if (list is null)
                    throw new KeyNotFoundException($"List with identifier {searchPhrase} was not found.");

                listGuid = list.Id;
            }

            var columns = await _client.Request(new GetListColumnsRequest(siteId, listGuid));

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(fileContent);

            var root = await syntaxTree.GetRootAsync();
            var @class = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            string[] blacklist = new string[] { "Edit", "Attachments", "ContentType" };
            var properties = columns.Columns.Where(x => !x.ReadOnly && !blacklist.Contains(x.Name))
                .Select(x => ClassMemberFactory.Property(GetPropertyType(x), x.Name.Replace(' ', '_')))
                .ToList<MemberDeclarationSyntax>();

            properties.Insert(0, ClassMemberFactory.StringField("ListId", listGuid, true, true));
            properties.Insert(0, ClassMemberFactory.StringField("SiteId", siteId, true, true));

            var modifiedClass = @class.WithMembers(SyntaxFactory.List(properties));

            return root.ReplaceNode(@class, modifiedClass).NormalizeWhitespace().ToString();
        }

        private string GetPropertyType(SharepointColumn column)
        {
            if (column.Boolean != null)
                return column.Required ? "bool" : "bool?";
            else if (column.Text != null)
                return column.Required ? "string" : "string?";
            else if (column.Number != null)
                return column.Required ? "decimal" : "decimal?";
            else if (column.DateTime != null)
                return column.Required ? "DateTime" : "DateTime?";

            return "object";
        }
    }
}
