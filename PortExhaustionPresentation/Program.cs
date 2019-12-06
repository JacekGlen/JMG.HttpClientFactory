using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using CommandLine;

namespace PortExhaustionPresentationFramework
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var server = new DummyServer();
            var exec = new HttpClientExecutor();

            //Parser.Default.ParseArguments<StandardOptions, StandardDisposeOptions, StaticOptions, FactoryDefaultOptions>(args)
            //    .WithParsed<StandardOptions>(async opts => await exec.Run(exec.RequestStandard, opts.Iterations))
            //    .WithParsed<StandardDisposeOptions>(async opts => await exec.Run(exec.RequestStandardWithDispose, opts.Iterations))
            //    .WithParsed<StaticOptions>(async opts => await exec.Run(exec.RequestStaticInstance, opts.Iterations))
            //    .WithParsed<FactoryDefaultOptions>(async opts => await exec.Run(exec.RequestFactoryDefault, opts.Iterations));

            var task = Parser.Default.ParseArguments<StandardOptions, StandardDisposeOptions, StaticOptions, FactoryDefaultOptions>(args)
                .MapResult(
                    async (StandardOptions opts) => { await exec.Run(exec.RequestStandard, opts.Iterations); },
                    async (StandardDisposeOptions opts) => { await exec.Run(exec.RequestStandardWithDispose, opts.Iterations); },
                    async (StaticOptions opts) => { await exec.Run(exec.RequestStaticInstance, opts.Iterations); },
                    async (FactoryDefaultOptions opts) => { await exec.Run(exec.RequestFactoryDefault, opts.Iterations); },
                    errors => Task.CompletedTask
                );
            await task;

                //.WithParsed<StandardOptions>(async opts => await exec.Run(exec.RequestStandard, opts.Iterations))
                //.WithParsed<StandardDisposeOptions>(async opts => await exec.Run(exec.RequestStandardWithDispose, opts.Iterations))
                //.WithParsed<StaticOptions>(async opts => await exec.Run(exec.RequestStaticInstance, opts.Iterations))
                //.WithParsed<FactoryDefaultOptions>(async opts => await exec.Run(exec.RequestFactoryDefault, opts.Iterations));


            if (exec.Executed)
            {
                Console.WriteLine();
                Console.WriteLine("Hey, I'm done with " + exec.ErrorsCount + " errors");
            }

            server.Close();
        }
    }

    [Verb("standard", HelpText = "Creates a new instance of HttpClient for each request")]
    class StandardOptions : Options
    {
    }

    [Verb("standardDispose", HelpText = "Creates a new instance of HttpClient for each request and disposes it immediately")]
    class StandardDisposeOptions : Options
    {
    }

    [Verb("static", HelpText = "Re-uses a single instace of HttpClient for all requests")]
    class StaticOptions : Options
    {
    }

    [Verb("factory", HelpText = "Uses HttpClientFactory to create a new instance of HttpClient for each request using ")]
    class FactoryDefaultOptions : Options
    {
    }

    public class Options
    {
        [Option('i', "iterations",
            Default = 100000,
            HelpText = "Number of times to repeat the operation")]
        public int Iterations { get; set; }
    }

}
