using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Razensoft.Mapper
{
    public static partial class MapperExtensions
    {
        /// <summary>
        /// Maps the list of <typeparamref name="TSource"/> into an observable collection of
        /// <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source objects.</typeparam>
        /// <typeparam name="TDestination">The type of the destination objects.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <param name="source">The source objects.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An observable collection of <typeparamref name="TDestination"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="mapper"/> or <paramref name="source"/> is
        /// <c>null</c>.</exception>
        public static async Task<ObservableCollection<TDestination>> MapObservableCollectionAsync<TSource, TDestination>(
            this IAsyncMapper<TSource, TDestination> mapper,
            List<TSource> source,
            CancellationToken cancellationToken = default)
            where TDestination : new()
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceCount = source.Count;
            var tasks = new Task[sourceCount];
            var destination = new ObservableCollection<TDestination>();
            for (var i = 0; i < sourceCount; ++i)
            {
                var sourceItem = source[i];
                var destinationItem = new TDestination();
                destination.Insert(i, destinationItem);
                tasks[i] = mapper.MapAsync(sourceItem, destinationItem, cancellationToken);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return destination;
        }

        /// <summary>
        /// Maps the collection of <typeparamref name="TSource"/> into an observable collection of
        /// <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source objects.</typeparam>
        /// <typeparam name="TDestination">The type of the destination objects.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <param name="source">The source objects.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An observable collection of <typeparamref name="TDestination"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="mapper"/> or <paramref name="source"/> is
        /// <c>null</c>.</exception>
        public static async Task<ObservableCollection<TDestination>> MapObservableCollectionAsync<TSource, TDestination>(
            this IAsyncMapper<TSource, TDestination> mapper,
            Collection<TSource> source,
            CancellationToken cancellationToken = default)
            where TDestination : new()
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceCount = source.Count;
            var tasks = new Task[sourceCount];
            var destination = new ObservableCollection<TDestination>();
            for (var i = 0; i < sourceCount; ++i)
            {
                var sourceItem = source[i];
                var destinationItem = new TDestination();
                destination.Insert(i, destinationItem);
                tasks[i] = mapper.MapAsync(sourceItem, destinationItem, cancellationToken);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return destination;
        }

        /// <summary>
        /// Maps the array of <typeparamref name="TSource"/> into an observable collection of
        /// <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source objects.</typeparam>
        /// <typeparam name="TDestination">The type of the destination objects.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <param name="source">The source objects.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An observable collection of <typeparamref name="TDestination"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="mapper"/> or <paramref name="source"/> is
        /// <c>null</c>.</exception>
        public static async Task<ObservableCollection<TDestination>> MapObservableCollectionAsync<TSource, TDestination>(
            this IAsyncMapper<TSource, TDestination> mapper,
            TSource[] source,
            CancellationToken cancellationToken = default)
            where TDestination : new()
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceCount = source.Length;
            var tasks = new Task[sourceCount];
            var destination = new ObservableCollection<TDestination>();
            for (var i = 0; i < sourceCount; ++i)
            {
                var sourceItem = source[i];
                var destinationItem = new TDestination();
                destination.Insert(i, destinationItem);
                tasks[i] = mapper.MapAsync(sourceItem, destinationItem, cancellationToken);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return destination;
        }

        /// <summary>
        /// Maps the enumerable of <typeparamref name="TSource"/> into an observable collection of
        /// <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source objects.</typeparam>
        /// <typeparam name="TDestination">The type of the destination objects.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <param name="source">The source objects.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An observable collection of <typeparamref name="TDestination"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="mapper"/> or <paramref name="source"/> is
        /// <c>null</c>.</exception>
        public static async Task<ObservableCollection<TDestination>> MapObservableCollectionAsync<TSource, TDestination>(
            this IAsyncMapper<TSource, TDestination> mapper,
            IEnumerable<TSource> source,
            CancellationToken cancellationToken = default)
            where TDestination : new()
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceCount = source.Count();
            var tasks = new Task[sourceCount];
            var destination = new ObservableCollection<TDestination>();
            var i = 0;
            foreach (var sourceItem in source)
            {
                var destinationItem = new TDestination();
                destination.Insert(i, destinationItem);
                tasks[i] = mapper.MapAsync(sourceItem, destinationItem, cancellationToken);
                ++i;
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return destination;
        }

        /// <summary>
        /// Maps the list of <typeparamref name="TSource"/> into an observable collection of
        /// <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source objects.</typeparam>
        /// <typeparam name="TDestination">The type of the destination objects.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <param name="source">The source objects.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An observable collection of <typeparamref name="TDestination"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="mapper"/> or <paramref name="source"/> is
        /// <c>null</c>.</exception>
        public static async Task<ObservableCollection<TDestination>> MapObservableCollectionAsync<TSource, TDestination>(
            this IAsyncImmutableMapper<TSource, TDestination> mapper,
            List<TSource> source,
            CancellationToken cancellationToken = default)
            where TDestination : new()
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceCount = source.Count;
            var tasks = new Task<TDestination>[sourceCount];
            var destination = new ObservableCollection<TDestination>();
            for (var i = 0; i < sourceCount; ++i)
            {
                var sourceItem = source[i];
                tasks[i] = mapper.MapAsync(sourceItem, cancellationToken);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            for (var i = 0; i < tasks.Length; ++i)
            {
#pragma warning disable VSTHRD103 // Call async methods when in an async method.
                destination.Insert(i, tasks[i].Result);
#pragma warning restore VSTHRD103 // Call async methods when in an async method.
            }

            return destination;
        }

        /// <summary>
        /// Maps the collection of <typeparamref name="TSource"/> into an observable collection of
        /// <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source objects.</typeparam>
        /// <typeparam name="TDestination">The type of the destination objects.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <param name="source">The source objects.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An observable collection of <typeparamref name="TDestination"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="mapper"/> or <paramref name="source"/> is
        /// <c>null</c>.</exception>
        public static async Task<ObservableCollection<TDestination>> MapObservableCollectionAsync<TSource, TDestination>(
            this IAsyncImmutableMapper<TSource, TDestination> mapper,
            Collection<TSource> source,
            CancellationToken cancellationToken = default)
            where TDestination : new()
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceCount = source.Count;
            var tasks = new Task<TDestination>[sourceCount];
            var destination = new ObservableCollection<TDestination>();
            for (var i = 0; i < sourceCount; ++i)
            {
                var sourceItem = source[i];
                tasks[i] = mapper.MapAsync(sourceItem, cancellationToken);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            for (var i = 0; i < tasks.Length; ++i)
            {
#pragma warning disable VSTHRD103 // Call async methods when in an async method.
                destination.Insert(i, tasks[i].Result);
#pragma warning restore VSTHRD103 // Call async methods when in an async method.
            }

            return destination;
        }

        /// <summary>
        /// Maps the array of <typeparamref name="TSource"/> into an observable collection of
        /// <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source objects.</typeparam>
        /// <typeparam name="TDestination">The type of the destination objects.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <param name="source">The source objects.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An observable collection of <typeparamref name="TDestination"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="mapper"/> or <paramref name="source"/> is
        /// <c>null</c>.</exception>
        public static async Task<ObservableCollection<TDestination>> MapObservableCollectionAsync<TSource, TDestination>(
            this IAsyncImmutableMapper<TSource, TDestination> mapper,
            TSource[] source,
            CancellationToken cancellationToken = default)
            where TDestination : new()
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceCount = source.Length;
            var tasks = new Task<TDestination>[sourceCount];
            var destination = new ObservableCollection<TDestination>();
            for (var i = 0; i < sourceCount; ++i)
            {
                var sourceItem = source[i];
                tasks[i] = mapper.MapAsync(sourceItem, cancellationToken);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            for (var i = 0; i < tasks.Length; ++i)
            {
#pragma warning disable VSTHRD103 // Call async methods when in an async method.
                destination.Insert(i, tasks[i].Result);
#pragma warning restore VSTHRD103 // Call async methods when in an async method.
            }

            return destination;
        }

        /// <summary>
        /// Maps the enumerable of <typeparamref name="TSource"/> into an observable collection of
        /// <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source objects.</typeparam>
        /// <typeparam name="TDestination">The type of the destination objects.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <param name="source">The source objects.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An observable collection of <typeparamref name="TDestination"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="mapper"/> or <paramref name="source"/> is
        /// <c>null</c>.</exception>
        public static async Task<ObservableCollection<TDestination>> MapObservableCollectionAsync<TSource, TDestination>(
            this IAsyncImmutableMapper<TSource, TDestination> mapper,
            IEnumerable<TSource> source,
            CancellationToken cancellationToken = default)
            where TDestination : new()
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceCount = source.Count();
            var tasks = new Task<TDestination>[sourceCount];
            var destination = new ObservableCollection<TDestination>();
            var i = 0;
            foreach (var sourceItem in source)
            {
                tasks[i] = mapper.MapAsync(sourceItem, cancellationToken);
                ++i;
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            for (var j = 0; j < tasks.Length; ++j)
            {
#pragma warning disable VSTHRD103 // Call async methods when in an async method.
                destination.Insert(j, tasks[j].Result);
#pragma warning restore VSTHRD103 // Call async methods when in an async method.
            }

            return destination;
        }
    }
}