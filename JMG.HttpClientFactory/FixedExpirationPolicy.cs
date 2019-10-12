using System;
using System.Collections.Generic;
using System.Text;

namespace JMG.HttpClientFactory
{
    public class FixedExpirationPolicy : IExpirationPolicy
    {
        private readonly TimeSpan _singleInstanceTime;
        private DateTime _currentWindowExpiration;

        public FixedExpirationPolicy(TimeSpan instanceTime)
        {
            if (instanceTime.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            _singleInstanceTime = instanceTime;
        }

        public bool HandlerExpired()
        {
            var hasExpired = _currentWindowExpiration < DateTime.UtcNow;

            if (hasExpired)
            {
                ResetExpirationWindow();
            }

            return hasExpired;
        }

        private void ResetExpirationWindow()
        {
            _currentWindowExpiration = DateTime.UtcNow.Add(_singleInstanceTime);
        }
    }
}
