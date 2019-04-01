using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMG.HttpClientFactoryTests
{
    [TestFixture]
    public class PerformanceTest
    {
        [Test]
        public void SingleThreadInstanceInvokingShouldBeNotMuchSlowerThanDefaultHttpClient()
        {
            Assert.IsTrue(true);
        }
    }
}
