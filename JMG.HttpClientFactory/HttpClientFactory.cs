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
             {  String.Empty, InstanceManager.Default }
        };

        public void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy) where THttpClient : HttpClient
        {
            _instaceManagers.Add(
                InstanceKey<THttpClient>(),
                new InstanceManager(clientBuilder, handlerBuilder, policy)
                );
        }

        private string InstanceKey<THttpClient>() where THttpClient : HttpClient
        {
            return typeof(THttpClient).FullName;
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
    }
}
