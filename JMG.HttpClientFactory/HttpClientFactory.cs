using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace JMG.HttpClientFactory
{

    public class HttpClientFactory
    {
        private static readonly Dictionary<string, InstanceManager> _instaceManagers = new Dictionary<string, InstanceManager>
        {
             {  "", InstanceManager.Default }
        };

        public HttpClientFactory()
        {

        }

        public void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder) where THttpClient : HttpClient
        {
            _instaceManagers.Add(
                InstanceKey<THttpClient>(),
                new InstanceManager(clientBuilder, handlerBuilder)
                );
        }

        public HttpClient Build()
        {
            return _instaceManagers[""].Build();
        }

        private string InstanceKey<THttpClient>() where THttpClient : HttpClient
        {
            return typeof(THttpClient).FullName;
        }

        public THttpClient Setup<THttpClient>() where THttpClient : HttpClient
        {
            var key = InstanceKey<THttpClient>();
            var instanceManager = _instaceManagers[key];

            return (THttpClient)instanceManager.Build();
        }
    }
}
