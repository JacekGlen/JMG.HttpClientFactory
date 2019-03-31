using System;
using System.Collections.Generic;
using System.Text;

namespace JMG.HttpClientFactory
{
    public abstract class HandlerPolicy
    {
        public HandlerPolicy(int poolSize)
        {
            if (poolSize < 1 && poolSize > Constants.MaxUserSockets)
            {
                throw new ArgumentOutOfRangeException();
            }

            PoolSize = poolSize;
        }

        public int PoolSize { get; }

        public abstract IHandlerExpiration StartExpiration();
    }
}
