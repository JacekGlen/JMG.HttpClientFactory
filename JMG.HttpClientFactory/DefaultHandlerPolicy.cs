using System;
using System.Collections.Generic;
using System.Text;

namespace JMG.HttpClientFactory
{
    public class DefaultHandlerPolicy : IExpirationPolicy
    {
        public bool HandlerExpired()
        {
            return false;
        }
    }
}
