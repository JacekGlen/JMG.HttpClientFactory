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
            var repetitionCount = 1000000;

            Action a = () => { new HttpClient(); };
            a();

            timer.Start();
            SingleThreadRepetition(a, repetitionCount);
            timer.Stop();

            var orignalClientCreatingTime = timer.Elapsed;
            TestContext.WriteLine($"HttpClient instance creation executed in {orignalClientCreatingTime}");

            var sut = new HttpClientFactory.HttpClientFactory();
            sut.Build();

            Action b = () => { sut.Build(); };
            b();

            timer.Reset();
            timer.Start();
            SingleThreadRepetition(b, repetitionCount);
            timer.Stop();

            var factoryClientCreationTime = timer.Elapsed;
            TestContext.WriteLine($"HttpClientFactory instance creation executed in {factoryClientCreationTime}");

            var ratio = factoryClientCreationTime.TotalMilliseconds / orignalClientCreatingTime.TotalMilliseconds;
            TestContext.WriteLine($"HttpClientFactory instance creation executed in {ratio:P} of  standard HtpClient");
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
