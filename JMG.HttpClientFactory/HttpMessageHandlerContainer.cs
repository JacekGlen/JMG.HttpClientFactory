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
        private IHandlerExpirationMonitor _monitor;
        private HttpMessageHandler _handler;
        private readonly Func<HttpMessageHandler> _handlerBuilder;
        private readonly HandlerPolicy _handlerPolicy;
        private Object _builderLock = new object();

        public HttpMessageHandlerContainer(Func<HttpMessageHandler> handlerBuilder, HandlerPolicy handlerPolicy, int nextContainerIndex)
        {
            _handlerBuilder = handlerBuilder;
            _handlerPolicy = handlerPolicy;
            NextContainerIndex = nextContainerIndex;

            _handler = _handlerBuilder();
            _monitor = _handlerPolicy.StartExpirationMonitor();
        }

        public int NextContainerIndex { get; }

        private bool IsExpired()
        {
            return _handler == null || _monitor.IsExpired();
        }

        public HttpMessageHandler Get()
        {

            if (IsExpired())
            {
                lock (_builderLock)
                {
                    if (IsExpired())
                    {
                        _handler.Dispose();
                        _handler = _handlerBuilder();
                        _monitor = _handlerPolicy.StartExpirationMonitor();
                    }
                }
            }

            return _handler;
        }
    }
}
