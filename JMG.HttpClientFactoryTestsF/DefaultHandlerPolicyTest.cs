using System;
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
    public class DefaultHandlerPolicyTest
    {
        [Test]
        public void PolicyNeverExpiries()
        {
            var sut = new DefaultHandlerPolicy();

            for (int i = 0; i < 1000; ++i)
            {
                Assert.IsFalse(sut.HandlerExpired());
            }
        }
    }
}
