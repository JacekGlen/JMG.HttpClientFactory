using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JMG.HttpClientFactory.Benchmark
{
//    [SimpleJob(RuntimeMoniker.Net461)]
//    [SimpleJob(RuntimeMoniker.Net472, baseline: true)]
  //  [SimpleJob(RuntimeMoniker.Net48)]

    //[SimpleJob(RuntimeMoniker.NetCoreApp20)]
    //[SimpleJob(RuntimeMoniker.NetCoreApp21)]
    [SimpleJob(RuntimeMoniker.NetCoreApp30, launchCount: 20, warmupCount: 1, invocationCount: 10 )]
    //[SimpleJob(RuntimeMoniker.Mono)]
    //[ShortRunJob()]
    //[RPlotExporter]
    public class HttpClientTest
    {
        private string url = "http://localhost:3070";
        private HttpClient client = new HttpClient();
        private HttpClientFactory factory = HttpClientFactory.Default;

        public HttpClientTest()
        {
            factory.Setup((handler) => new CustomHttpClient(handler, "http://localhost:3070/", "secret"), () => new CustomHttpClientHandler(), new CountExpirationPolicy(100));
        }

        [Benchmark]
        public async Task<string> StaticInstance()
        {
            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        [Benchmark]
        public async Task<string> FactoryDefault()
        {
            var client = factory.Build();
            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        [Benchmark]
        public async Task<string> FactoryDefaultWithDispose()
        {
            var result = "";
            using (var client = factory.Build())
            {
                var response = await client.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
            }
            return result;
        }

        [Benchmark]
        public async Task<string> FactoryCustom()
        {
            var client = factory.Build<CustomHttpClient>();
            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        [Benchmark]
        public async Task<string> FactoryCustomWithDispose()
        {
            var result = "";
            using (client = factory.Build<CustomHttpClient>())
            {
                var response = await client.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
            }
            return result;
        }

        [Benchmark]
        public async Task<string> Standard()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        [Benchmark]
        public async Task<string> StandardWithDispose()
        {
            var result = "";
            using (client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
            }
            return result;
        }
    }
}
