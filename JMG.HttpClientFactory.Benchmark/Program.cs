using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;

namespace JMG.HttpClientFactory.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new DummyServer();

            var a = ManualConfig
                .Create(DefaultConfig.Instance)
                .With(ConfigOptions.DisableOptimizationsValidator);
           

            BenchmarkRunner.Run<HttpClientTest>(a);
            Console.ReadLine();

            server.Close();
        }
    }
}
