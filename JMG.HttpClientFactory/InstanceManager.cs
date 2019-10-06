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
        private readonly List<HttpMessageHandlerContainer> ActivePool;

        private readonly Func<HttpMessageHandler, HttpClient> _clientBuilder;
        private readonly Func<HttpMessageHandler> _handlerBuilder;
        private readonly HandlerPolicy policy;

        private int _currentHandlerIndex;

        private readonly object handlerBuilderLock = new object();

        public InstanceManager(Func<HttpMessageHandler, HttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, HandlerPolicy policy)
        {
            _clientBuilder = clientBuilder;
            _handlerBuilder = handlerBuilder;
            this.policy = policy;

            ActivePool = new List<HttpMessageHandlerContainer>(policy.PoolSize);

            for (int i = 0; i < policy.PoolSize; ++i)
            {
                var nextContainerIndex = i + 1 == policy.PoolSize ?
                    0 :
                    i + 1;
                ActivePool.Add(new HttpMessageHandlerContainer(_handlerBuilder, policy, nextContainerIndex));
            }

            _currentHandlerIndex = 0;
        }

        public HttpClient Build()
        {
            HttpMessageHandler httpMessageHandler;
            httpMessageHandler = ActivePool[_currentHandlerIndex].Get();
            _currentHandlerIndex = ActivePool[_currentHandlerIndex].NextContainerIndex;

            return _clientBuilder(httpMessageHandler);
        }

        public static InstanceManager Default
        {
            get
            {
                return new InstanceManager((handler) => new HttpClient(handler), () => new HttpClientHandler(), new DefaultHandlerPolicy());
            }
        }
    }
}

