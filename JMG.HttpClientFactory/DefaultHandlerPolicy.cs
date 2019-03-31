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

        public override IHandlerExpiration StartExpiration()
        {
            return new DefaultHandlerExpiration();
        }

        private class DefaultHandlerExpiration : IHandlerExpiration
        {
            public bool IsExpired()
            {
                return false;
            }
        }
    }
}
