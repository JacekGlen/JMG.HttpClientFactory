using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using JMG.HttpClientFactory;
using System.Threading;

namespace JMG.HttpClientFactory.Tests
{
    [TestFixture]
    public class SlidingHandlerPolicyTest
    {
        [Test]
        public void PolicyExpiriesAfterTimeSpan()
        {
            var sut = new SlidingExpirationPolicy(TimeSpan.FromSeconds(2));

            Assert.IsFalse(sut.ShouldRenew());

            Thread.Sleep(2100);

            Assert.IsTrue(sut.ShouldRenew());
        }

        [Test]
        public void PolicyDoesntExpireIfCallWithingTimeWindow()
        {
            var sut = new SlidingExpirationPolicy(TimeSpan.FromSeconds(2));

            Assert.IsFalse(sut.ShouldRenew());

            Thread.Sleep(1000);
            Assert.IsFalse(sut.ShouldRenew());

            Thread.Sleep(1000);
            Assert.IsFalse(sut.ShouldRenew());

            Thread.Sleep(1000);
            Assert.IsFalse(sut.ShouldRenew());
        }
    }
}
