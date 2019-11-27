using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace JMG.HttpClientFactory.Benchmark
{
    public class DummyServer
    {
        const string Prefix = "http://localhost:3070/";
        static HttpListener listener = null;

        public DummyServer()
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

        public void Close()
        {
            listener.Close();
        }

        static void Callback(IAsyncResult ar)
        {
            var context = listener.EndGetContext(ar);
            listener.BeginGetContext(Callback, null);
            var now = DateTime.UtcNow;
            var responseStr = "{ \"message\": \"Dick Laurent Is Dead\" }";
            byte[] buffer = Encoding.UTF8.GetBytes(responseStr);
            var response = context.Response;
            response.ContentType = "text/json";
            response.ContentLength64 = buffer.Length;
            response.StatusCode = 200;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            context.Response.Close();
        }
    }
}
