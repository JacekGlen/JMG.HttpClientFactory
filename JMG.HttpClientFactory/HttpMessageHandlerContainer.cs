using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JMG.HttpClientFactory
{
    public class HttpMessageHandlerContainer 
    {
        private readonly IHandlerExpirationMonitor monitor;

        public HttpMessageHandlerContainer(HttpMessageHandler handler, IHandlerExpirationMonitor monitor, int nextContainerIndex)
        {
            Handler = handler;
            this.monitor = monitor;
            NextContainerIndex = nextContainerIndex;
        }

        public HttpMessageHandler Handler { get; }
        public int NextContainerIndex { get; }

        public bool IsExpired()
        {
            return Handler == null || monitor.IsExpired();
        }
    }
}
