using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;


namespace JMG.HttpClientFactory
{
    public class InstanceManager
    {
        public Func<HttpClient> GetClient { get; set; }
        private readonly List<HttpMessageHandlerProxy> ActivePool;

        private readonly Func<HttpMessageHandler, HttpClient> _clientBuilder;
        private readonly Func<HttpMessageHandler> _handlerBuilder;

        public InstanceManager(Func<HttpMessageHandler, HttpClient> builder, Func<HttpMessageHandler> handlerBuilder)
        {
            _clientBuilder = builder;
            _handlerBuilder = handlerBuilder;
        }

        public HttpClient Build()
        {
            return _clientBuilder(_handlerBuilder());
        }

        public static InstanceManager Default
        {
            get
            {
                return new InstanceManager((handler) => new HttpClient(handler), () => new HttpClientHandler());
            }
        }
    }
}
