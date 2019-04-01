using System;
using System.Collections.Generic;
using System.Text;

namespace JMG.HttpClientFactory
{
    public class DefaultHandlerPolicy : HandlerPolicy
    {
        public DefaultHandlerPolicy(int poolSize = 1)
            : base(poolSize)
        {
        }

        public override IHandlerExpirationMonitor StartExpirationMonitor()
        {
            return new DefaultHandlerExpiration();
        }

        private class DefaultHandlerExpiration : IHandlerExpirationMonitor
        {
            public bool IsExpired()
            {
                return false;
            }
        }
    }
}
