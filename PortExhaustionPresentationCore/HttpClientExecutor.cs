using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using JMG.HttpClientFactory;

namespace PortExhaustionPresentationFramework
{
    public class HttpClientExecutor
    {
        public int ErrorsCount = 0;
        public bool Executed = false;

        public async Task Run(Func<Task> method, int iterationsCount)
        {
            ErrorsCount = 0;
            Executed = true;

            for (int i = 1; i <= iterationsCount; ++i)
            {
                try
                {
                    await method();
                    Console.WriteLine("Request complete: " + i);
                }
                catch (HttpRequestException ex)
                {
                    ErrorsCount++;
                    Console.WriteLine("Error when executing request " + i + ": " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("\tInner Exception: " + ex.InnerException.Message);
                    }
                }
            }
        }

        private string url = "http://localhost:3070";

        #region Request
        public async Task RequestStandard()
        {
            var client = new HttpClient();
            var result = await client.GetAsync(url);
        }
        #endregion

        #region RequestAndDispose
        public async Task RequestStandardWithDispose()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(url);
            }
        }
        #endregion

        #region Request optimized
        private static HttpClient client = new HttpClient();
        public async Task RequestStaticInstance()
        {
            var result = await client.GetAsync(url);
        }
        #endregion

        #region Request factory
        private HttpClientFactory factory = HttpClientFactory.Default;
        public async Task RequestFactoryDefault()
        {
            var hc = factory.Build();
            var result = await hc.GetAsync(url);
        }
        #endregion

    }
}
