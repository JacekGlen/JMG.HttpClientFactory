using System;
using System.Collections.Generic;
using System.Text;

namespace JMG.HttpClientFactory
{
    public class SlidingHandlerPolicy : HandlerPolicy
    {
        private readonly TimeSpan timeToExpire;

        public SlidingHandlerPolicy(TimeSpan timeToExpire, int poolSize = 1)
            : base(poolSize)
        {
            if (timeToExpire.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.timeToExpire = timeToExpire;
        }

        public override IHandlerExpirationMonitor StartExpirationMonitor()
        {
            return new SlidingHandlerExpiration(timeToExpire);
        }

        private class SlidingHandlerExpiration : IHandlerExpirationMonitor
        {
            private DateTime expiresAt;
            private readonly TimeSpan timeToExpire;

            public SlidingHandlerExpiration(TimeSpan timeToExpire)
            {
                this.timeToExpire = timeToExpire;
                this.expiresAt = DateTime.UtcNow.Add(timeToExpire);
            }

            public bool IsExpired()
            {
                if (DateTime.UtcNow > expiresAt)
                {
                    return true;
                }
                else
                {
                    this.expiresAt = DateTime.UtcNow.Add(timeToExpire);
                    return false;
                }
            }
        }
    }
}
