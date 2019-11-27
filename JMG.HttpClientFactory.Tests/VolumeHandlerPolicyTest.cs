using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using JMG.HttpClientFactory;

namespace JMG.HttpClientFactory.Tests
{
    [TestFixture]
    public class VolumeHandlerPolicyTest
    {
        [Test]
        public void PolicyExpiriesAfterNumberOfChecksExceedsVolume()
        {
            var sut = new CountExpirationPolicy(5);

            Assert.IsFalse(sut.ShouldRenew());
            Assert.IsFalse(sut.ShouldRenew());
            Assert.IsFalse(sut.ShouldRenew());
            Assert.IsFalse(sut.ShouldRenew());
            Assert.IsFalse(sut.ShouldRenew());
            Assert.IsTrue(sut.ShouldRenew());
            Assert.IsFalse(sut.ShouldRenew());
        }
    }
}
