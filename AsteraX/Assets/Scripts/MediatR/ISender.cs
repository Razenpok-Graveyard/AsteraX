﻿using System.Threading;
using Cysharp.Threading.Tasks;

namespace MediatR
{
    /// <summary>
    /// Send a request through the mediator pipeline to be handled by a single handler.
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Asynchronously send a request to a single handler
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
        UniTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously send an object request to a single handler via dynamic dispatch
        /// </summary>
        /// <param name="request">Request object</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A task that represents the send operation. The task result contains the type erased handler response</returns>
        UniTask<object> Send(object request, CancellationToken cancellationToken = default);
    }
}