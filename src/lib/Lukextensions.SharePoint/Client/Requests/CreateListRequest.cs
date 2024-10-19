using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lukextensions.SharePoint.Requests
{
    public class CreateListRequest : GraphRequestBase<SharepointListCreatedResponse>
    {
        private readonly string _siteId;
        private readonly NewListDefinition _list;
        public CreateListRequest(string siteId, NewListDefinition list)
        {
            _siteId = siteId;
            _list = list;
        }

        protected override Task<HttpResponseMessage> SendRequestAsync(HttpClient client)
        {
            var content = new StringContent(JsonConvert.SerializeObject(_list), Encoding.UTF8, "application/json");
            return client.PostAsync($"v1.0/sites/{_siteId}/lists", content);
        }
    }
}
