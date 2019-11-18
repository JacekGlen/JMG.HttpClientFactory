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


        private string GetBaseUri()
        {
            return "http://example.com/api";
        }

        private string GetApiAuthToken()
        {
            return Guid.NewGuid().ToString();
        }
        private HttpClientHandler CreateHandlerWithCertificate()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate());

            return handler;
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

        [Test]
        public void CreatesTypedInstanceOfClientAndHandler()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup<HttpClientTest, HttpMessageHandlerTest>();

            var result = sut.Build<HttpClientTest>();

            Assert.IsNotNull(result);
        }

        [Test]
        public void CreatesTypedInstanceWithoutPriorSetup()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            var result = sut.BuildAdHoc<HttpClientTest>();

            Assert.IsNotNull(result);
        }

        [Test]
        public void CreatesFromDefaultInstanceFactory()
        {
            var sut = HttpClientFactory.HttpClientFactory.Default;

            var result = sut.Build();

            Assert.IsNotNull(result);
        }

        [Test]
        public void CannotCreateUsingTypeNotSetPrior()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            Assert.Throws<Exception>(() => { sut.Build<HttpClientTest>(); });
        }

        [Test]
        public void CannotCreateUsingNamedInstanceNotSetPrior()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            Assert.Throws<Exception>(() => { sut.Build("MyInstance"); });
        }

        [Test]
        public void DefaultInstanceFactoryAndNewedUpFactoryDoNotShareConfig()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup<HttpClientTest>(new SlidingExpirationPolicy(TimeSpan.FromSeconds(2)));

            var client = sut.Build<HttpClientTest>();

            Assert.IsNotNull(client);


            var sutDefault = HttpClientFactory.HttpClientFactory.Default;

            Assert.Throws<Exception>(() => { sutDefault.Build<HttpClientTest>(); });
        }

        [Test]
        public async Task ReusesTheSameInstanceOfHandlerForDefaultPolicy()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup(
                    (handler) => new HttpClientTest(handler),
                    () => new HttpMessageHandlerTest(),
                    new DefaultHandlerPolicy()
                ); ;

            var client1 = sut.Build<HttpClientTest>();
            var response1 = await client1.GetAsync("http://example.com/api/users");
            var result1 = await response1.Content.ReadAsStringAsync();

            await Task.Delay(100);

            var client2 = sut.Build<HttpClientTest>();
            var response2 = await client1.GetAsync("http://example.com/api/users");
            var result2 = await response1.Content.ReadAsStringAsync();

            Assert.AreEqual(result1, result2);
        }

        [Test]
        public async Task CreatesNewInstanceOfHandlerWhenPolicyDictates()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup(
                    (handler) => new HttpClientTest(handler),
                    () => new HttpMessageHandlerTest(),
                    new CountExpirationPolicy(1)
                );

            var client1 = sut.Build<HttpClientTest>();
            var response1 = await client1.GetAsync("http://example.com/api/users");
            var result1 = await response1.Content.ReadAsStringAsync();

            await Task.Delay(100);

            var client2 = sut.Build<HttpClientTest>();
            var response2 = await client2.GetAsync("http://example.com/api/users");
            var result2 = await response2.Content.ReadAsStringAsync();

            Assert.AreNotEqual(result1, result2);
        }

        [Test]
        public async Task DisposingClientDoesNotDisposeMessageHandler()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup(
                    (handler) => new HttpClientTest(handler),
                    () => new HttpMessageHandlerTest(),
                    new DefaultHandlerPolicy()
                );

            string result1, result2, result3;

            using(var client1 = sut.Build<HttpClientTest>())
            {
                var response1 = await client1.GetAsync("http://example.com/api/users");
                result1 = await response1.Content.ReadAsStringAsync();
            }

            using (var client2 = sut.Build<HttpClientTest>())
            {
                var response2 = await client2.GetAsync("http://example.com/api/users");
                result2 = await response2.Content.ReadAsStringAsync();
            }
             
            using (var client3 = sut.Build<HttpClientTest>())
            {
                var response3 = await client3.GetAsync("http://example.com/api/users");
                result3 = await response3.Content.ReadAsStringAsync();
            }

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);

            Assert.AreEqual(result1, result2);
            Assert.AreEqual(result1, result3);
        }

        [Test]
        public async Task DisposingClientDoesNotDisposeClientHandler()
        {
            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup(
                    (handler) => new HttpClientTest(handler),
                    () => new HttpClientHandlerTest(),
                    new DefaultHandlerPolicy()
                );

            string result1, result2, result3;

            using (var client1 = sut.Build<HttpClientTest>())
            {
                var response1 = await client1.GetAsync("http://example.com/api/users");
                result1 = await response1.Content.ReadAsStringAsync();
            }

            using (var client2 = sut.Build<HttpClientTest>())
            {
                var response2 = await client2.GetAsync("http://example.com/api/users");
                result2 = await response2.Content.ReadAsStringAsync();
            }

            using (var client3 = sut.Build<HttpClientTest>())
            {
                var response3 = await client3.GetAsync("http://example.com/api/users");
                result3 = await response3.Content.ReadAsStringAsync();
            }

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);

            Assert.AreEqual(result1, result2);
            Assert.AreEqual(result1, result3);
        }

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
    public class HttpMessageHandlerTest : HttpMessageHandler
    {
        readonly string message;

        public HttpMessageHandlerTest()
        {
            message = DateTime.UtcNow.Ticks.ToString();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(message) });
        }
    }

    public class HttpClientHandlerTest : HttpClientHandler
    {
        readonly string message;

        public HttpClientHandlerTest()
        {
            message = DateTime.UtcNow.Ticks.ToString();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(message) });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
