using System;
using System.Net.Http;

namespace JMG.HttpClientFactory
{
    public interface IHttpClientFactory
    {
        HttpClient Build();
        HttpClient Build(string namedInstance);
        THttpClient Build<THttpClient>() where THttpClient : HttpClient;
        void Setup(string namedInstance, Func<HttpMessageHandler, HttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy);
        void Setup(string namedInstance, Func<HttpMessageHandler, HttpClient> clientBuilder, IExpirationPolicy policy);
        void Setup(string namedInstance, IExpirationPolicy policy);
        void Setup<THttpClient>() where THttpClient : HttpClient;
        void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy) where THttpClient : HttpClient;
        void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, IExpirationPolicy policy) where THttpClient : HttpClient;
        void Setup<THttpClient>(Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy) where THttpClient : HttpClient;
        void Setup<THttpClient>(IExpirationPolicy policy) where THttpClient : HttpClient;
    }
}