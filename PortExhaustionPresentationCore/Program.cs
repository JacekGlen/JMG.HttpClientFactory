using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace PortExhaustionPresentationFramework
{
    public class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            var exec = new HttpClientExecutor();

            exec.Run(exec.Request, 10).Wait();
            //exec.Run(exec.RequestAndDispose, 100000).Wait();

            Console.WriteLine("Hey, I'm done with " + exec.errorsCount + " errors");
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
        public int errorsCount = 0;

        public async Task Run(Func<Task> method, int iterationsCount = 10)
        {
            errorsCount = 0;
            for (int i = 1; i <= iterationsCount; ++i)
            {
                try
                {
                    await method();
                    Console.WriteLine("Request complete: " + i);
                }
                catch (HttpRequestException ex)
                {
                    errorsCount++;
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
        public async Task Request()
        {
            var client = new HttpClient();
            var result = await client.GetAsync(url);
        }
        #endregion

        #region RequestAndDispose
        public async Task RequestAndDispose()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(url);
            }
        }
        #endregion

        #region Request optimized
        private static HttpClient client = new HttpClient();
        public async Task RequestWithStatic()
        {
            var result = await client.GetAsync(url);
        }
        #endregion
    }
}
