using System;

namespace JMG.HttpClientFactory
{
    public class SlidingExpirationPolicy : IExpirationPolicy
    {
        private readonly TimeSpan _slidingWindow;
        private DateTime _currentWindowExpiration;

        public SlidingExpirationPolicy(TimeSpan slidingWindow)
        {
            if (slidingWindow.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slidingWindow));
            }

            _slidingWindow = slidingWindow;
            ResetExpirationWindow();
        }

        public bool ShouldRenew()
        {
            var hasExpired = _currentWindowExpiration < DateTime.UtcNow;

            ResetExpirationWindow();

            return hasExpired;
        }

        private void ResetExpirationWindow()
        {
            _currentWindowExpiration = DateTime.UtcNow.Add(_slidingWindow);
        }
    }
}
