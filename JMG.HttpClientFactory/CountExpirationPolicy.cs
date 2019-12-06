using System;
using System.Collections.Generic;
using System.Text;

namespace JMG.HttpClientFactory
{
    public class CountExpirationPolicy : IExpirationPolicy
    {
        private readonly long _maxCallsPerInstance;
        private long _currentCount;

        public CountExpirationPolicy(long maxCallsPerInstance)
        {
            if (maxCallsPerInstance <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCallsPerInstance));
            }

            _maxCallsPerInstance = maxCallsPerInstance;
        }

        public bool ShouldRenew()
        {
            _currentCount++;
            var hasExpired = _currentCount > _maxCallsPerInstance;

            if (hasExpired)
            {
                ResetCount();
            }

            return hasExpired;
        }

        private void ResetCount()
        {
            _currentCount = 0;
        }
    }
}
