using System;
using System.Collections.Generic;
using System.Text;

namespace JMG.HttpClientFactory
{
    public interface IHandlerExpiration
    {
        bool IsExpired();
    }
}
