using System;
using System.Net.Http;

namespace JMG.HttpClientFactory
{
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Creates a new instance of HttpClient using default builders for the client and the handler, and default expiration policy.
        /// Ready-to-eat: No prior setup is required
        /// </summary>
        /// <returns>HttpClient instance</returns>
        /// <example>
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
        /// <typeparam name="THttpClient">A class deriving from HttpClient. The class has to have a contstutor which takes a single HttpMessageHandler parameter</typeparam>
        /// <returns><c>THttpClient</c> instance</returns>
        THttpClient Build<THttpClient>() where THttpClient : HttpClient;

        /// <summary>
        /// Creates a new instance of <c>THttpClient</c> without prior setup.
        /// Warning: This method will not actively manage HttpMessageHandler and therefore creates a new instance for each call. Not recommended.
        /// </summary>
        /// <typeparam name="THttpClient">A class deriving from HttpClient. The class has to have a contstutor which takes a single HttpMessageHandler parameter</typeparam>
        /// <returns><c>THttpClient</c> instance</returns>
        THttpClient BuildAdHoc<THttpClient>() where THttpClient : HttpClient;

        /// <summary>
        /// Sets up a builder than can be then used to create a new instance of <c><HttpClient/c>
        /// </summary>
        /// <param name="namedInstance">A string that uniquely identifies this builder. Used to get the instance.</param>
        /// <param name="clientBuilder">A function that creates a new instance of <c>HttpClient</c>. This has to take <c>HttpMessageHandler</c> parameter</param>
        /// <param name="handlerBuilder">A function that creates a new instance of <c>HttpMessageHandler</c>. Used when expiration policy indicates the need to refresh the handler.</param>
        /// <param name="policy">A policy that governs when a new instance of <c>HttpMessageHandler</c> should be created.</param>
        /// <example>
        /// <code>
        /// factory.Setup("MyClient", 
        ///        (handler) => new HttpClient(handler), 
        ///        () => { var handler = new HttpClientHandler(); handler.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials; return handler; },
        ///        new FixedExpirationPolicy(TimeSpan.FromSeconds(10))
        /// );
        /// </code>
        /// </example>
        void Setup(string namedInstance, Func<HttpMessageHandler, HttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy);

        /// <summary>
        /// Sets up a builder than can be then used to create a new instance of <c><HttpClient/c>. This uses default HttpMessageHandler.
        /// </summary>
        /// <param name="namedInstance">A string that uniquely identifies this builder. Used to get the instance.</param>
        /// <param name="clientBuilder">A function that creates a new instance of <c>HttpClient</c>. This has to take <c>HttpMessageHandler</c> parameter</param>
        /// <param name="policy">A policy that governs when a new instance of <c>HttpMessageHandler</c> should be created.</param>
        /// <example>
        /// <code>
        /// factory.Setup("MyClient", 
        ///        (handler) => new HttpClient(handler), 
        ///        new FixedExpirationPolicy(TimeSpan.FromSeconds(10))
        /// );
        /// </code>
        /// </example>
        void Setup(string namedInstance, Func<HttpMessageHandler, HttpClient> clientBuilder, IExpirationPolicy policy);

        /// <summary>
        /// Sets up a named instance using the default HttpClient and default HttpMessageHandler
        /// </summary>
        /// <param name="namedInstance">A string that uniquely identifies this builder. Used to get the instance.</param>
        /// <param name="policy">A policy that governs when a new instance of <c>HttpMessageHandler</c> should be created.</param>
        void Setup(string namedInstance, IExpirationPolicy policy);

        /// <summary>
        /// Sets up a strongly typed HttpClient. Useful when creating a type per API.
        /// </summary>
        /// <typeparam name="THttpClient">A class deriving from HttpClient. The class has to have a contstutor which takes a single HttpMessageHandler parameter</typeparam>
        void Setup<THttpClient>() where THttpClient : HttpClient;

        /// <summary>
        /// Sets up a strongly typed HttpClient with its own builders. The most natural use of the factory and this allows to specify all aspects of creating the client.
        /// The handler builder should include all application specific settings like security, certificates, etc.
        /// </summary>
        /// <typeparam name="THttpClient">A class deriving from HttpClient.</typeparam>
        /// <param name="clientBuilder">A function that creates a new instance of <c>HttpClient</c>. This has to take <c>HttpMessageHandler</c> parameter</param>
        /// <param name="handlerBuilder">A function that creates a new instance of <c>HttpMessageHandler</c>. Used when expiration policy indicates the need to refresh the handler.</param>
        /// <param name="policy">A policy that governs when a new instance of <c>HttpMessageHandler</c> should be created.</param>
        /// <example>
        /// <code>
        /// factory.Setup( 
        ///        (handler) => new UserApiHttpClient(handler),
        ///        () => { var handler = new HttpClientHandler(); handler.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials; return handler; },
        ///        new FixedExpirationPolicy(TimeSpan.FromSeconds(10))
        /// );
        /// </code>
        /// </example>
        void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy) where THttpClient : HttpClient;

        /// <summary>
        /// Sets up a strongly typed HttpClient with its own builder and default HttpMessageHandler
        /// </summary>
        /// <typeparam name="THttpClient">A class deriving from HttpClient.</typeparam>
        /// <param name="clientBuilder">A function that creates a new instance of <c>HttpClient</c>. This has to take <c>HttpMessageHandler</c> parameter</param>
        /// <param name="policy">A policy that governs when a new instance of <c>HttpMessageHandler</c> should be created.</param>
        void Setup<THttpClient>(Func<HttpMessageHandler, THttpClient> clientBuilder, IExpirationPolicy policy) where THttpClient : HttpClient;

        /// <summary>
        /// Sets up a strongly typed HttpClient with its own builder. Useful when creating a client type per API.
        /// </summary>
        /// <typeparam name="THttpClient">A class deriving from HttpClient. The class has to have a contstutor which takes a single HttpMessageHandler parameter</typeparam>
        /// <param name="handlerBuilder">A function that creates a new instance of <c>HttpMessageHandler</c>. Used when expiration policy indicates the need to refresh the handler.</param>
        /// <param name="policy">A policy that governs when a new instance of <c>HttpMessageHandler</c> should be created.</param>
        void Setup<THttpClient>(Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy) where THttpClient : HttpClient;

        /// <summary>
        /// Sets up a strongly typed HttpClient. Useful when creating a client type per API.
        /// </summary>
        /// <typeparam name="THttpClient">A class deriving from HttpClient. The class has to have a contstutor which takes a single HttpMessageHandler parameter</typeparam>
        /// <param name="policy">A policy that governs when a new instance of <c>HttpMessageHandler</c> should be created.</param>
        void Setup<THttpClient>(IExpirationPolicy policy) where THttpClient : HttpClient;

        /// <summary>
        /// Sets up strongly typed HttpClient and HttpMessageHandler.
        /// </summary>
        /// <typeparam name="THttpClient">A class deriving from HttpClient. The class has to have a contstutor which takes a single HttpMessageHandler parameter</typeparam>
        /// <typeparam name="THttpMessageHandler">A class deriving from HttpMessageHandler. The class has to have parameterless constructor.</typeparam>
        void Setup<THttpClient, THttpMessageHandler>() where THttpClient : HttpClient where THttpMessageHandler : HttpMessageHandler;
    }
}