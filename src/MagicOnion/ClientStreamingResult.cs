﻿using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MagicOnion
{
    /// <summary>
    /// Wrapped AsyncClientStreamingCall.
    /// </summary>
    public struct ClientStreamingResult<TRequest, TResponse> : IDisposable
    {
        readonly AsyncClientStreamingCall<byte[], byte[]> inner;
        readonly MarshallingClientStreamWriter<TRequest> requestStream;
        readonly Marshaller<TResponse> responseMarshaller;

        public ClientStreamingResult(AsyncClientStreamingCall<byte[], byte[]> inner, Marshaller<TRequest> requestMarshaller, Marshaller<TResponse> responseMarshaller)
        {
            this.inner = inner;
            this.requestStream = new MarshallingClientStreamWriter<TRequest>(inner.RequestStream, requestMarshaller);
            this.responseMarshaller = responseMarshaller;
        }

        async Task<TResponse> Deserialize()
        {
            var bytes = await inner.ResponseAsync.ConfigureAwait(false);
            return responseMarshaller.Deserializer(bytes);
        }

        /// <summary>
        /// Asynchronous call result.
        /// </summary>
        public Task<TResponse> ResponseAsync
        {
            get
            {
                return Deserialize();
            }
        }

        /// <summary>
        /// Asynchronous access to response headers.
        /// </summary>
        public Task<Metadata> ResponseHeadersAsync
        {
            get
            {
                return this.inner.ResponseHeadersAsync;
            }
        }

        /// <summary>
        /// Async stream to send streaming requests.
        /// </summary>
        public IClientStreamWriter<TRequest> RequestStream
        {
            get
            {
                return this.requestStream;
            }
        }

        /// <summary>
        /// Allows awaiting this object directly.
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter<TResponse> GetAwaiter()
        {
            return ResponseAsync.GetAwaiter();
        }

        /// <summary>
        /// Gets the call status if the call has already finished.
        /// Throws InvalidOperationException otherwise.
        /// </summary>
        public Status GetStatus()
        {
            return this.inner.GetStatus();
        }

        /// <summary>
        /// Gets the call trailing metadata if the call has already finished.
        /// Throws InvalidOperationException otherwise.
        /// </summary>
        public Metadata GetTrailers()
        {
            return this.inner.GetTrailers();
        }

        /// <summary>
        /// Provides means to cleanup after the call.
        /// If the call has already finished normally (request stream has been completed and call result has been received), doesn't do anything.
        /// Otherwise, requests cancellation of the call which should terminate all pending async operations associated with the call.
        /// As a result, all resources being used by the call should be released eventually.
        /// </summary>
        /// <remarks>
        /// Normally, there is no need for you to dispose the call unless you want to utilize the
        /// "Cancel" semantics of invoking <c>Dispose</c>.
        /// </remarks>
        public void Dispose()
        {
            this.inner.Dispose();
        }
    }
}