using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lukextensions.CloudRemover
{
    public class TenantRemover
    {
        private const string GUID_TYPE = "Guid";
        private const string LOGGED_USER_TYPE = "LoggedUser";
        private const string TENANT_PROPERTY_NAME = "Tenant";
        private IReadOnlyList<string> ConvertMethodsToPropertiesList = new List<string>() { "All", "AllAsNoTracking" };

        private readonly SyntaxTree _original;
        private SyntaxNode modified;
        public TenantRemover(string content)
        {
            _original = CSharpSyntaxTree.ParseText(content);
        }

        public string RemoveTenantParameters()
        {
            modified = _original.GetRoot();

            RemoveLoggedUserTenantAccesses();
            ConvertMethodsToProperties();
            RemoveGuidParameters();

            modified = modified.NormalizeWhitespace();
            return modified.ToString();
        }

        private void RemoveLoggedUserTenantAccesses()
        {
            List<SyntaxNode> nodesToRemove = new List<SyntaxNode>();

            var loggedUserParameters = modified.DescendantNodes()
                .OfType<ParameterSyntax>()
                .Where(x => x.Type != null && x.Type.ToString() == LOGGED_USER_TYPE);

            foreach (var parameter in loggedUserParameters)
            {
                var parameterName = parameter.Identifier.ToString();
                var tenantAccesses = parameter.Parent.Parent.DescendantNodes()
                    .OfType<MemberAccessExpressionSyntax>()
                    .Where(x => x.Expression is IdentifierNameSyntax expressionName && expressionName.Identifier.ToString() == parameterName && x.Name.ToString() == TENANT_PROPERTY_NAME);
                nodesToRemove.AddRange(tenantAccesses.Select(x => x.Parent));
            }

            modified = modified.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepNoTrivia);
        }

        private void ConvertMethodsToProperties()
        {
            var memberAcceses = modified.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>()
                .Where(x => ConvertMethodsToPropertiesList.Contains(x.Name.ToString()));

            Dictionary<SyntaxNode, SyntaxNode> nodesToReplace = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var access in memberAcceses)
            {
                if (access.Parent is InvocationExpressionSyntax)
                {
                    nodesToReplace.Add(access.Parent, access);
                }
            }

            modified = modified.ReplaceNodes(nodesToReplace.Keys, (node, node2) => { return nodesToReplace[node]; });
        }

        private void RemoveGuidParameters()
        {
            List<SyntaxNode> nodesToRemove = new List<SyntaxNode>();

            var guidParameters = modified.DescendantNodes()
                .OfType<ParameterSyntax>()
                .Where(x => x.Type != null && x.Type.ToString() == GUID_TYPE);
            nodesToRemove.AddRange(guidParameters);

            foreach (var parameter in guidParameters)
            {
                var parameterName = parameter.Identifier.ToString();
                var guidAccesses = parameter.Parent.Parent.DescendantNodes()
                    .OfType<MemberAccessExpressionSyntax>()
                    .Where(x => parameterName == x.Name.Identifier.ToString());
                nodesToRemove.AddRange(guidAccesses.Select(x => x.Parent));

                var guidArguments = parameter.Parent.Parent.DescendantNodes()
                    .OfType<ArgumentSyntax>()
                    .Where(x => x.Expression is IdentifierNameSyntax identifierName && identifierName.Identifier.ToString() == parameterName);
                nodesToRemove.AddRange(guidArguments);
            }

            modified = modified.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepNoTrivia);
        }
    }
}
