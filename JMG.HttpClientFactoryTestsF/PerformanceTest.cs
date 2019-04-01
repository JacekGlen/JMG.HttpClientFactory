using JMG.HttpClientFactory;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace JMG.HttpClientFactoryTestsF
{
    [TestFixture]
    public class PerformanceTest
    {
        [Test]
        public void SingleThreadInstanceInvokingShouldBeNotMuchSlowerThanDefaultHttpClient()
        {
            var timer = new Stopwatch();

            Action a = () => { new HttpClient(); };

            a();

            timer.Start();
            SingleThreadRepetition(a, 1000);
            timer.Stop();

            TestContext.WriteLine($"HttpClient executed in {timer.Elapsed}");

            var sut = new HttpClientFactory.HttpClientFactory();

            sut.Setup<HttpClient>((h) => new HttpClient(h, false), () => new HttpClientHandler(), new DefaultHandlerPolicy());

            sut.Build();

            Action b = () => { sut.Build(); };

            timer.Reset();
            timer.Start();
            SingleThreadRepetition(b, 1000);
            timer.Stop();

            TestContext.WriteLine($"HttpClientFactory executed in {timer.Elapsed}");

        }

        public void SingleThreadRepetition(Action a, long numberOfRepetitions)
        {
            for (long i = 0; i < numberOfRepetitions; i++)
            {
                a();
            }
        }
    }
}
