using System.Net.Http;
using System.Threading.Tasks;

namespace Lukextensions.SharePoint.Requests
{
    public class GetListColumnsRequest : GraphRequestBase<SharepointColumnsResponse>
    {
        private readonly string _siteId;
        private readonly string _listId;
        public GetListColumnsRequest(string siteId, string listId)
        {
            _siteId = siteId;
            _listId = listId;
        }

        protected override async Task<HttpResponseMessage> SendRequestAsync(HttpClient client)
        {
            return await client.GetAsync($"v1.0/sites/{_siteId}/lists/{_listId}/columns");
        }
    }
}
