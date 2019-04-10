using System;
using System.Collections.Generic;
using System.Text;

namespace JMG.HttpClientFactory
{
    public class VolumeHandlerPolicy : HandlerPolicy
    {
        private readonly long maxCallsPerInstance;

        public VolumeHandlerPolicy(long maxCallsPerInstance, int poolSize = 1)
            : base(poolSize)
        {
            if (maxCallsPerInstance <= 0)
            {
                throw new ArgumentException();
            }

            this.maxCallsPerInstance = maxCallsPerInstance;
        }

        public override IHandlerExpirationMonitor StartExpirationMonitor()
        {
            return new VolumeHandlerExpiration(this.maxCallsPerInstance);
        }

        private class VolumeHandlerExpiration : IHandlerExpirationMonitor
        {
            private readonly long _maxCallsPerInstance;
            private long _callsCount;

            public VolumeHandlerExpiration(long maxCallsPerInstance)
            {
                _maxCallsPerInstance = maxCallsPerInstance;
                _callsCount = 0;
            }

            public bool IsExpired()
            {
                return ++_callsCount > _maxCallsPerInstance;
            }
        }
    }
}
