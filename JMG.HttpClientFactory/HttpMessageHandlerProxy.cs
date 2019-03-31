using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JMG.HttpClientFactory
{
    public class HttpMessageHandlerProxy : HttpClientHandler, IDisposable
    {
        private HttpClientHandler _internalHandler;

        public HttpMessageHandlerProxy(HttpClientHandler internalHandler)
        {
            _internalHandler = internalHandler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var a =  _internalHandler.SendAsync(request, cancellationToken);

            this.SendAsync

            return a;
        }



    }
}
