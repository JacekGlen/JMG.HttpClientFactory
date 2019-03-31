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

        public override IHandlerExpiration StartExpiration()
        {
            throw new NotImplementedException();
        }

        private class VolumeHandlerExpiration : IHandlerExpiration
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
                throw new NotImplementedException();
            }
        }
    }
}
