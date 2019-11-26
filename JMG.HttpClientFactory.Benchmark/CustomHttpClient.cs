using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JMG.HttpClientFactory.Benchmark
{
    public class CustomHttpClient : HttpClient
    {
        public CustomHttpClient(HttpMessageHandler handler, string baseUri, string apiAuthToken) : base(handler)
        {
            this.BaseAddress = new Uri(baseUri);
            this.DefaultRequestHeaders.Add("Bearer", apiAuthToken);
        }

        public CustomHttpClient(HttpMessageHandler handler, string baseUri) : base(handler)
        {
            this.BaseAddress = new Uri(baseUri);
        }

        public CustomHttpClient(HttpMessageHandler handler) : base(handler)
        {
        }
    }
}
