using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;


namespace JMG.HttpClientFactory
{
    public class InstanceManager
    {
        private readonly Func<HttpMessageHandler, HttpClient> _clientBuilder;
        private readonly Func<HttpMessageHandler> _handlerBuilder;
        private readonly IExpirationPolicy _hanndlerPolicy;
        private HttpMessageHandler _handler;

        public InstanceManager(Func<HttpMessageHandler, HttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy)
        {
            _clientBuilder = clientBuilder;
            _handlerBuilder = handlerBuilder;
            _hanndlerPolicy = policy;

            _handler = handlerBuilder();
        }

        public HttpClient Build()
        {
            if (_hanndlerPolicy.HandlerExpired())
            {
                _handler = _handlerBuilder();
            }

            var httpClient = _clientBuilder(_handler);

            return httpClient;
        }

        public static InstanceManager Default
        {
            get
            {
                return new InstanceManager((handler) => new HttpClient(handler, false), () => new HttpClientHandler(), new DefaultHandlerPolicy());
            }
        }
    }
}

