using JMG.HttpClientFactory;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JMG.HttpClientFactoryTestsF
{
    [TestFixture]
    public class HttpClientFactoryTest
    {
        [Test]
        public void CreatesDefaultClient()
        {
            IHttpClientFactory sut = new HttpClientFactory.HttpClientFactory();

            var result = sut.Build();

            Assert.IsNotNull(result);
        }

        [Test]
        public void CreatesDefaultNamedClient()
        {
            var sut = new HttpClientFactory.HttpClientFactory();
            sut.Setup("myclient", new DefaultHandlerPolicy());

            var result = sut.Build("myclient");

            Assert.IsNotNull(result);
        }

        [Test]
        public void DefaultNamedClientIsDifferentToDefaultClient()
        {
            var sut = new HttpClientFactory.HttpClientFactory();
            sut.Setup("myclient", new DefaultHandlerPolicy());

            var result1 = sut.Build("myclient");
            var result2 = sut.Build();

            Assert.AreNotSame(result1, result2);
        }

        [Test]
        public void CreatesNamedInstanceFromBuilder()
        {
            var sut = new HttpClientFactory.HttpClientFactory();
            sut.Setup("myclient", (handler) => new HttpClient(handler), new DefaultHandlerPolicy()); ;

            var result = sut.Build("myclient");

            Assert.IsNotNull(result);
        }

        [Test]
        public void CreatesNamedInstanceFromBuilderAndHandlerBuilder()
        {
            var sut = new HttpClientFactory.HttpClientFactory();
            sut.Setup("myclient", (handler) => new HttpClient(handler), () => new HttpClientHandler(), new DefaultHandlerPolicy()); ;

            var result = sut.Build("myclient");

            Assert.IsNotNull(result);
        }

        public class HttpClientTest : HttpClient
        {
            public HttpClientTest(HttpMessageHandler handler, string baseUri, string apiAuthToken) : base(handler)
            {
                this.BaseAddress = new Uri(baseUri);
                this.DefaultRequestHeaders.Add("Bearer", apiAuthToken);
            }

            public HttpClientTest(HttpMessageHandler handler, string baseUri) : base(handler)
            {
                this.BaseAddress = new Uri(baseUri);
            }

            public HttpClientTest(HttpMessageHandler handler) : base(handler)
            {
            }
        }

        private string GetBaseUri()
        {
            return "http://example.com/api";
        }

        private string GetApiAuthToken()
        {
            return Guid.NewGuid().ToString();
        }

        [Test]
        public void CreatesTypedInstanceUsingClientBuilderAndHandlerBuilder()
        {
            var sut = new HttpClientFactory.HttpClientFactory();
            sut.Setup(
                (handler) => new HttpClientTest(handler, GetBaseUri(), GetApiAuthToken()),
                () => new HttpClientHandler(),
                new DefaultHandlerPolicy()); ;

            var result = sut.Build<HttpClientTest>();

            Assert.IsNotNull(result);
            Assert.AreEqual("http://example.com/api", result.BaseAddress.AbsoluteUri);
            Assert.IsTrue(result.DefaultRequestHeaders.Any(h => h.Key == "Bearer"));
        }

        private HttpClientHandler CreateHandlerWithCertificate()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate());

            return handler;
        }

        [Test]
        public void CreatesTypedInstanceWithHandlerCertificateFunction()
        {
            var sut = new HttpClientFactory.HttpClientFactory();
            sut.Setup(
                (handler) => new HttpClientTest(handler, GetBaseUri(), GetApiAuthToken()),
                () => CreateHandlerWithCertificate(),
                new DefaultHandlerPolicy()); ;

            var result = sut.Build<HttpClientTest>();

            Assert.IsNotNull(result);
            Assert.AreEqual("http://example.com/api", result.BaseAddress.AbsoluteUri);
            Assert.IsTrue(result.DefaultRequestHeaders.Any(h => h.Key == "Bearer"));
        }

        [Test]
        public void CreatesTypedInstanceUsingDefaultConstructor()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup<HttpClientTest>();

            var result = sut.Build<HttpClientTest>();

            Assert.IsNotNull(result);
        }

        [Test]
        public void CreatesTypedInstanceUsingDefaultConstructorAndPolicy()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup<HttpClientTest>(new DefaultHandlerPolicy());

            var result = sut.Build<HttpClientTest>();

            Assert.IsNotNull(result);
        }

        [Test]
        public void CreatesTypedInstanceUsingHandlerBuilderAndPolicy()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup<HttpClientTest>(
                CreateHandlerWithCertificate,
                new CountExpirationPolicy(10)
            );

            var result = sut.Build<HttpClientTest>();

            Assert.IsNotNull(result);
        }

        [Test]
        public void CreatesTypedInstanceUsingClientBuilderAndPolicy()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup(
                (handler) => new HttpClientTest(handler),
                new CountExpirationPolicy(10)
            ); ;

            var result = sut.Build<HttpClientTest>();

            Assert.IsNotNull(result);
        }

        private class HttpMessageHandlerTest : HttpMessageHandler
        {
            readonly string message;

            public HttpMessageHandlerTest()
            {
                message = DateTime.UtcNow.ToString();
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(message) });
            }
        }


    }
}
