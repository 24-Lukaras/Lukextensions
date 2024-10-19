using System.Net.Http;
using System.Threading.Tasks;

namespace Lukextensions.SharePoint.Requests
{
    public class SearchForListsRequest : GraphRequestBase<SharepointListsResponse>
    {
        private readonly string _siteId;
        private readonly string _searchPhrase;
        public SearchForListsRequest(string siteId, string searchPhrase = null)
        {
            _siteId = siteId;
            _searchPhrase = searchPhrase;
        }

        protected override async Task<HttpResponseMessage> SendRequestAsync(HttpClient client)
        {
            if (!string.IsNullOrEmpty(_searchPhrase))
            {
                return await client.GetAsync($"v1.0/sites/{_siteId}/lists?$search={client}");
            }
            return await client.GetAsync($"v1.0/sites/{_siteId}/lists");
        }
    }
}
