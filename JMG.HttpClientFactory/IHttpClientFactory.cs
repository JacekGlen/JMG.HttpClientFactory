using System;
using System.Net.Http;

namespace JMG.HttpClientFactory
{
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Creates a new instance of HttpClient using default builders for client and handler, and default expiration policy.
        /// Ready-to-eat: No prior setup is required
        /// </summary>
        /// <returns>HttpClient instance</returns>
        HttpClient Build();

        /// <summary>
        /// Creates a new instance of HttpClient using previously set up builders. If no builder was specified then default is used.
        /// </summary>
        /// <param name="namedInstance">Configured name for this instance</param>
        /// <returns>HttpClient instance</returns>
        HttpClient Build(string namedInstance);

        /// <summary>
        /// Creates a new instance of <c>THttpClient</c> using previously set up builders. If no builder was specified then default is used.
        /// </summary>
        /// <typeparam name="THttpClient"></typeparam>
        /// <returns><c>THttpClient</c> instance</returns>
        THttpClient Build<THttpClient>() where THttpClient : HttpClient;

        /// <summary>
        /// Sets up a builder than can be then used to create a new instance of <c><HttpClient/c>
        /// </summary>
        /// <param name="namedInstance">A string that uniquely identifies this builder. Used to get the instance.</param>
        /// <param name="clientBuilder">A function that creates a new instance of <c>HttpClient</c>. This has to take <c>HttpMessageHandler</c> parameter</param>
        /// <param name="handlerBuilder">A function that creates a new instance of <c>HttpMessageHandler</c>. Used when expiration policy indicates the need to refresh the handler.</param>
        /// <param name="policy">A policy that governs when a new instance of <c>HttpMessageHandler</c> is required.</param>
        /// <example>
        /// <code>
        /// sut.Setup("MyClient", 
        ///        (handler) => new HttpClient(handler), 
        ///        () => { var handler = new HttpClientHandler(); handler.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials; return handler; },
        ///        new FixedExpirationPolicy(TimeSpan.FromSeconds(10))
        /// );
        /// </code>
        /// </example>
        void Setup(string namedInstance, Func<HttpMessageHandler, HttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy);

        /// <summary>
        /// Sets up a builder than can be then used to create a new instance of <c><HttpClient/c>. This uses default 
        /// </summary>
        /// <param name="namedInstance">A string that uniquely identifies this builder. Used to get the instance.</param>
        /// <param name="clientBuilder">A function that creates a new instance of <c>HttpClient</c>. This has to take <c>HttpMessageHandler</c> parameter</param>
        /// <param name="policy"></param>
        void Setup(string namedInstance, Func<HttpMessageHandler, HttpClient> clientBuilder, IExpirationPolicy policy);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namedInstance"></param>
        /// <param name="policy"></param>
        void Setup(string namedInstance, IExpirationPolicy policy);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="THttpClient"></typeparam>
        void Setup<THttpClient>() where THttpClient : HttpClient;
        void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy) where THttpClient : HttpClient;
        void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, IExpirationPolicy policy) where THttpClient : HttpClient;
        void Setup<THttpClient>(Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy) where THttpClient : HttpClient;
        void Setup<THttpClient>(IExpirationPolicy policy) where THttpClient : HttpClient;
    }
}