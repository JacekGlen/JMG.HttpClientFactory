using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;


namespace JMG.HttpClientFactory
{
    public class InstanceManager
    {
        private readonly Func<HttpMessageHandler, HttpClient> _clientBuilder;
        private readonly Func<HttpMessageHandler> _handlerBuilder;
        private readonly IExpirationPolicy _hanndlerPolicy;
        private HttpMessageHandler _handler;

        FieldInfo _disposeHandlerField;
        FieldInfo _disposedField;

        public InstanceManager(Func<HttpMessageHandler, HttpClient> clientBuilder, Func<HttpMessageHandler> handlerBuilder, IExpirationPolicy policy)
        {
            if (clientBuilder == null)
            {
                throw new ArgumentNullException(nameof(clientBuilder));
            }

            if (handlerBuilder == null)
            {
                throw new ArgumentNullException(nameof(handlerBuilder));
            }

            _clientBuilder = clientBuilder;
            _handlerBuilder = handlerBuilder;
            _hanndlerPolicy = policy;

            _handler = handlerBuilder();
            SetFieldSelectors(_handler);
        }

        public HttpClient Build()
        {
            if (_hanndlerPolicy.ShouldRenew() || IsHandlerDisposed())
            {
                _handler = _handlerBuilder();
            }

            var httpClient = _clientBuilder(_handler);

            _disposeHandlerField.SetValue(httpClient, false);

            return httpClient;
        }

        private bool IsHandlerDisposed()
        {
            if (_disposedField == null) return false;

            var isDisposed = _disposedField.GetValue(_handler);

            if (isDisposed is bool)
            {
                return (bool)isDisposed;
            }

            return false;
        }

        public static InstanceManager Default
        {
            get
            {
                return new InstanceManager((handler) => new HttpClient(handler, false), () => new HttpClientHandler(), new DefaultHandlerPolicy());
            }
        }

        private void SetFieldSelectors(HttpMessageHandler handler)
        {
            var privateFieldFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            //.NET Framework and .NET Core use different namig convention for private fields
            var httpMessageInvokerType = typeof(HttpMessageInvoker);
            _disposeHandlerField =
                httpMessageInvokerType.GetField("disposeHandler", privateFieldFlags) ??
                httpMessageInvokerType.GetField("_disposeHandler", privateFieldFlags);


            var httpMessageHandlerType = handler.GetType();
            _disposedField =
                httpMessageHandlerType.GetField("disposed", privateFieldFlags) ??
                httpMessageHandlerType.GetField("_disposed", privateFieldFlags);
        }
    }
}

