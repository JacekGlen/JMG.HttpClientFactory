using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace JMG.HttpClientFactory
{

    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly Dictionary<string, InstanceManager> _instaceManagers;

        public HttpClientFactory()
        {
            _instaceManagers = new Dictionary<string, InstanceManager>
                {
                       {  String.Empty, InstanceManager.Default }
                };
        }

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


        public void Setup<THttpClient, THttpMessageHandler>() where THttpClient : HttpClient where THttpMessageHandler : HttpMessageHandler
        {
            _instaceManagers[InstanceKey<THttpClient>()] = new InstanceManager(
                CreateHttpClientBuilder<THttpClient>(),
                CreateHttpMessageBuilder<THttpMessageHandler>(),
                new DefaultHandlerPolicy());
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
            return Build<THttpClient>(key);
        }

        public HttpClient Build(string namedInstance)
        {
            var key = InstanceKey(namedInstance);
            return Build<HttpClient>(key);
        }

        private THttpClient Build<THttpClient>(string instanceManagerKey) where THttpClient : HttpClient
        {
            if (!_instaceManagers.ContainsKey(instanceManagerKey))
            {
                throw new Exception($"Could not find builder for {instanceManagerKey}. Please use Setup() first.");
            }

            var instanceManager = _instaceManagers[instanceManagerKey];

            return (THttpClient)instanceManager.Build();
        }

        public THttpClient BuildAdHoc<THttpClient>() where THttpClient : HttpClient
        {
            var client = CreateHttpClientBuilder<THttpClient>()(
                            CreateHttpMessageBuilder<HttpClientHandler>()()
                        );

            return client;
        }


        private HttpMessageHandler DefaultHttpMessageHandlerBuilder()
        {
            return new HttpClientHandler();
        }

        private Func<HttpMessageHandler, THttpClient> CreateHttpClientBuilder<THttpClient>() where THttpClient : HttpClient
        {
            var constructorWithHandler = typeof(THttpClient).GetConstructor(new[] { typeof(HttpMessageHandler) });

            if (constructorWithHandler == null)
            {
                throw new ArgumentException($"The type {typeof(THttpClient).FullName} does not have constructor that takes HttpMessageHandler as a parameter.");
            }

            Func<HttpMessageHandler, THttpClient> builder = (handler) =>
            {
                return constructorWithHandler.Invoke(new[] { handler }) as THttpClient;
            };

            return builder;
        }

        private Func<HttpMessageHandler> CreateHttpMessageBuilder<THttpMessageHandler>() where THttpMessageHandler : HttpMessageHandler
        {
            var constructor = typeof(THttpMessageHandler).GetConstructor(Array.Empty<Type>());

            if (constructor == null)
            {
                throw new ArgumentException($"The type does not have parameterless constructor.");
            }

            Func<HttpMessageHandler> builder = () =>
            {
                return constructor.Invoke(Array.Empty<object>()) as THttpMessageHandler;
            };

            return builder;
        }

        private static HttpClientFactory _defaultInstance = new HttpClientFactory();
        public static HttpClientFactory Default => _defaultInstance;
    }
}
