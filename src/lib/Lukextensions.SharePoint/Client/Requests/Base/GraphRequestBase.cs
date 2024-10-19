using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lukextensions.SharePoint.Requests
{
    public abstract class GraphRequestBase<T>
    {

        public async Task<T> ExecuteRequest(HttpClient client)
        {
            var response = await SendRequestAsync(client);
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        protected abstract Task<HttpResponseMessage> SendRequestAsync(HttpClient client);
    }
}
