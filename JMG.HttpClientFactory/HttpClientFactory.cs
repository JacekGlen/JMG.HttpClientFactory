using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace JMG.HttpClientFactory
{

    public class HttpClientFactory : IHttpClientFactory
    {
        private static readonly Dictionary<string, InstanceManager> _instaceManagers = new Dictionary<string, InstanceManager>
        {
             {  String.Empty, InstanceManager.Default }
        };

        public void Setup<THttpClient>() where THttpClient : HttpClient
        {
            Setup(CreateHttpClientBuilder<THttpClient>(), DefaultHttpMessageHandlerBuilder, new DefaultHandlerPolicy());
        }

        public void Setup<THttpClient>(IExpirationPolicy policy) where THttpClient : HttpClient
        {
            Setup(CreateHttpClientBuilder<THttpClient>(), DefaultHttpMessageHandlerBuilder, policy);
        }

        public void Setup<THttpClient>(Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy) where THttpClient : HttpClient
        {
            Setup(CreateHttpClientBuilder<THttpClient>(), handlerBuilder, policy);
        }

        public void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, IExpirationPolicy policy) where THttpClient : HttpClient
        {
            Setup(clientBuilder, DefaultHttpMessageHandlerBuilder, policy);
        }

        public void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy) where THttpClient : HttpClient
        {
            _instaceManagers[InstanceKey<THttpClient>()] = new InstanceManager(clientBuilder, handlerBuilder, policy);
        }


        public void Setup(string namedInstance, IExpirationPolicy policy)
        {
            Setup(namedInstance, CreateHttpClientBuilder<HttpClient>(), DefaultHttpMessageHandlerBuilder, policy);
        }

        public void Setup(string namedInstance, Func<HttpMessageHandler, HttpClient> clientBuilder, IExpirationPolicy policy)
        {
            Setup(namedInstance, clientBuilder, DefaultHttpMessageHandlerBuilder, policy);
        }

        public void Setup(string namedInstance, Func<HttpMessageHandler, HttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy)
        {
            _instaceManagers[InstanceKey(namedInstance)] = new InstanceManager(clientBuilder, handlerBuilder, policy);
        }



        private string InstanceKey<THttpClient>() where THttpClient : HttpClient
        {
            return "Type:" + typeof(THttpClient).FullName;
        }

        private string InstanceKey(string namedInstance)
        {
            return "NamedInstance:" + namedInstance;
        }

        public HttpClient Build()
        {
            return _instaceManagers[String.Empty].Build();
        }

        public THttpClient Build<THttpClient>() where THttpClient : HttpClient
        {
            var key = InstanceKey<THttpClient>();
            var instanceManager = _instaceManagers[key];

            return (THttpClient)instanceManager.Build();
        }

        public HttpClient Build(string namedInstance)
        {
            var key = InstanceKey(namedInstance);
            var instanceManager = _instaceManagers[key];

            return instanceManager.Build();
        }

        //build without prior setup using default settings
        //public THttpClient BuildDefault<THttpClient>() where THttpClient : HttpClient
        //{
        //    var key = InstanceKey<THttpClient>();
        //    var instanceManager = _instaceManagers[key];

        //    return (THttpClient)instanceManager.Build();
        //}


        private HttpMessageHandler DefaultHttpMessageHandlerBuilder()
        {
            return new HttpClientHandler();
        }

        private HttpClient DefaultHttpClientBuilder(HttpMessageHandler httpClientHandler)
        {
            return new HttpClient(httpClientHandler, false);
        }

        private Func<HttpMessageHandler, THttpClient> CreateHttpClientBuilder<THttpClient>() where THttpClient : HttpClient
        {
            var constructurWithHandler = typeof(THttpClient).GetConstructor(new[] { typeof(HttpMessageHandler) });

            if (constructurWithHandler == null)
            {
                throw new ArgumentException($"The type {typeof(THttpClient).FullName} does not have constructor that takes HttpMessageHandler as a parameter.");
            }

            Func<HttpMessageHandler, THttpClient> builder = (handler) =>
            {
                return constructurWithHandler.Invoke(new[] { handler }) as THttpClient;
            };

            return builder;
        }
    }
}
