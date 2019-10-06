using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text;

namespace PortExhaustionPresentationFramework
{
    public class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            var exec = new HttpClientExecutor();

            //exec.RunNewInstancePerRequest(10).Wait();
            //exec.RunNewInstancePerRequest(100000).Wait();
            //exec.RunNewInstancePerRequestWithDispose(10).Wait();
            exec.RunNewInstancePerRequestWithDispose(100000).Wait();
            //            exec.RunSingleInstance(100000).Wait();

            Console.WriteLine("hey I'm done!");
            Console.ReadLine();
        }
    }

    public class Server
    {
        const string Prefix = "http://localhost:3070/";
        static HttpListener listener = null;

        public Server()
        {
            if (!HttpListener.IsSupported)
            {
                throw new Exception("Listener not supported");
            }

            listener = new HttpListener();
            listener.Prefixes.Add(Prefix);
            listener.Start();
            listener.BeginGetContext(Callback, null);
        }

        static void Callback(IAsyncResult ar)
        {
            var context = listener.EndGetContext(ar);
            listener.BeginGetContext(Callback, null);
            var now = DateTime.UtcNow;
            var responseStr = String.Format("<html><body>It's a request!");
            byte[] buffer = Encoding.UTF8.GetBytes(responseStr);
            var response = context.Response;
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            response.StatusCode = 200;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }

    public class HttpClientExecutor
    {
        public async Task RunNewInstancePerRequest(int iterations = 100)
        {
            for (int i = 1; i <= iterations; ++i)
            {
                try
                {
                    var client = new HttpClient();
                    var result = await client.GetAsync("http://localhost:3070");
                    Console.WriteLine("request complete: " + i);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("\tInner Exception: " + ex.InnerException.Message);
                    }
                }
            }

            return;
        }

        public async Task RunNewInstancePerRequestWithDispose(int iterations = 100)
        {
            for (int i = 1; i <= iterations; ++i)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var result = await client.GetAsync("http://localhost:3070");
                        Console.WriteLine("request complete: " + i);
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("\tInner Exception: " + ex.InnerException.Message);
                    }
                }
            }

            return;
        }
        public async Task RunSingleInstance(int iterations = 100)
        {
            var client = new HttpClient();
            for (int i = 1; i <= iterations; ++i)

            {
                try
                {
                    var result = await client.GetAsync("http://localhost:3070");
                    Console.WriteLine("request complete: " + i);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("\tInner Exception: " + ex.InnerException.Message);
                    }
                }
            }

            return;
        }

    }
}
