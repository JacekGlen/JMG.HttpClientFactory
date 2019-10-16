using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace JMG.HttpClientFactory
{
    public class HttpClientFactoryMock : IHttpClientFactory
    {

        private HttpMessageHandler _handler;

        public HttpClientFactoryMock(HttpMessageHandler mockedHandler)
        {
            _handler = mockedHandler;
        }

        public HttpClient Build()
        {
            return CreateHttpClientBuilder<HttpClient>()(_handler);
        }

        public HttpClient Build(string namedInstance)
        {
            return CreateHttpClientBuilder<HttpClient>()(_handler);
        }

        public THttpClient Build<THttpClient>() where THttpClient : HttpClient
        {
            return CreateHttpClientBuilder<THttpClient>()(_handler);
        }

        public void Setup(string namedInstance, Func<HttpMessageHandler, HttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy)
        {
            throw new NotImplementedException();
        }

        public void Setup(string namedInstance, Func<HttpMessageHandler, HttpClient> clientBuilder, IExpirationPolicy policy)
        {
            throw new NotImplementedException();
        }

        public void Setup(string namedInstance, IExpirationPolicy policy)
        {
            throw new NotImplementedException();
        }

        public void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy) where THttpClient : HttpClient
        {
            throw new NotImplementedException();
        }

        public void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, IExpirationPolicy policy) where THttpClient : HttpClient
        {
            throw new NotImplementedException();
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
