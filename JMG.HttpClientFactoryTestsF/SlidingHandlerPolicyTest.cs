using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using JMG.HttpClientFactory;
using System.Threading;

namespace JMG.HttpClientFactoryTestsF
{
    [TestFixture]
    public class SlidingHandlerPolicyTest
    {
        [Test]
        public void PolicyExpiriesAfterTimeSpan()
        {
            var sut = new SlidingHandlerPolicy(TimeSpan.FromSeconds(2));
            var monitor = sut.StartExpirationMonitor();

            Assert.IsFalse(monitor.IsExpired());

            Thread.Sleep(2100);
            Assert.IsTrue(monitor.IsExpired());
        }
    }
}
