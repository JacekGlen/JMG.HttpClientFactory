using System;
using System.Collections.Generic;
using System.Text;

namespace JMG.HttpClientFactory
{
    public class TemporalHandlerPolicy : HandlerPolicy
    {
        private readonly TimeSpan timeToExpire;

        public TemporalHandlerPolicy(TimeSpan timeToExpire, int poolSize = 1)
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
            return new TemporalHandlerExpiration(timeToExpire);
        }

        private class TemporalHandlerExpiration:IHandlerExpirationMonitor
        {
            private readonly DateTime expiresAt;

            public TemporalHandlerExpiration(TimeSpan timeToExpire)
            {

                this.expiresAt = DateTime.UtcNow.Add(timeToExpire);
            }

            public bool IsExpired()
            {
                return DateTime.UtcNow > expiresAt;
            }
        }
    }
}
