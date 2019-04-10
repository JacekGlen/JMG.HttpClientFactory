﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using JMG.HttpClientFactory;

namespace JMG.HttpClientFactoryTestsF
{
    [TestFixture]
    public class VolumeHandlerPolicyTest
    {
        [Test]
        public void PolicyExpiriesAfterNumberOfChecksExceedsVolume()
        {
            var sut = new VolumeHandlerPolicy(5);
            var monitor = sut.StartExpirationMonitor();

            Assert.IsFalse(monitor.IsExpired());
            Assert.IsFalse(monitor.IsExpired());
            Assert.IsFalse(monitor.IsExpired());
            Assert.IsFalse(monitor.IsExpired());
            Assert.IsFalse(monitor.IsExpired());
            Assert.IsTrue(monitor.IsExpired());
        }
    }
}
