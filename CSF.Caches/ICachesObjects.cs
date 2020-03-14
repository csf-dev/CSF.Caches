//
// ICachesObjects.cs
//
// Author:
//       Craig Fowler <craig@csf-dev.com>
//
// Copyright (c) 2020 Craig Fowler
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    /// A type-safe object which caches key/value pairs of objects according to a configured policy.
    /// </summary>
    public interface ICachesObjects<TKey, TValue> where TKey : IGetsCacheKey
    {
        /// <summary>
        /// Adds a new object to the cache using the specified key.
        /// </summary>
        /// <param name="key">The key for use in caching the value.</param>
        /// <param name="value">The value to cache.</param>
        /// <exception cref="InvalidCacheOperationException">
        /// If an item already exists in the cache with the given key.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Adds a new object to the cache using the specified key, or performs no action if an item
        /// already exists in the cache using the specified key.
        /// </summary>
        /// <returns><c>true</c> if a value was added; <c>false</c> if no action was taken.</returns>
        /// <param name="key">The key for use in caching the value.</param>
        /// <param name="value">The value to cache.</param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        bool TryAdd(TKey key, TValue value);

        /// <summary>
        /// Gets an item from the cache using the specified key.  If it does not exist in the cache then
        /// a <paramref name="valueCreator"/> function is used to create a new value, store it in the
        /// cache and then return it.
        /// </summary>
        /// <returns>A value from the cache (which might have just been created if it did not exist already).</returns>
        /// <param name="key">The key for which the value is to be cached.</param>
        /// <param name="valueCreator">A function which creates/gets a value for the key, if it does not already exist in the cache.</param>
        /// <exception cref="InvalidCacheOperationException">
        /// If no value is stored within the cache for the specified
        /// key but the implementation was unable to add the item either. This typically points to a concurrency
        /// problem of some kind, specific to the cache implementation.
        /// </exception>
        /// <exception cref="InvalidCastException">If the value stored for the specified key is not of type <typeparamref name="TValue"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        TValue GetOrAdd(TKey key, Func<TKey, TValue> valueCreator);

        /// <summary>
        /// Gets a value which indicates whether or not a value is stored for the specified key.
        /// </summary>
        /// <returns><c>true</c> if a value is stored for the key; <c>false</c> otherwise.</returns>
        /// <param name="key">The key for a value.</param>
        bool Contains(TKey key);

        /// <summary>
        /// Gets an item from the cache using the specified key.
        /// </summary>
        /// <returns>A value from the cache.</returns>
        /// <param name="key">The key for which the value is to be cached.</param>
        /// <exception cref="InvalidCacheOperationException">If no value is stored within the cache for the specified key.</exception>
        /// <exception cref="InvalidCastException">If the value stored for the specified key is not of type <typeparamref name="TValue"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        TValue Get(TKey key);

        /// <summary>
        /// Gets a collection of items which match the specified keys.
        /// Because it is not guaranteed that all the specified <paramref name="keys"/> exist within
        /// the cache, the returned collection may be smaller than the specified collection of keys.
        /// </summary>
        /// <returns>A collection of keys-and-items representing the matching items found in the cache.</returns>
        /// <param name="keys">A collection of keys to retrieve.</param>
        /// <exception cref="ArgumentNullException">
        /// If any item in the <paramref name="keys"/> collection is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If any item in the <paramref name="keys"/> collection returns <c>null</c> from its
        /// <see cref="IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        IEnumerable<CacheKeyAndItem<TKey, TValue>> Get(IEnumerable<TKey> keys);

        /// <summary>
        /// Gets an item from the cache using the specified key, or returns the default for the data-type if
        /// no item exists with the specified key.
        /// </summary>
        /// <returns>A value from the cache, or the default for <typeparamref name="TValue"/>.</returns>
        /// <param name="key">The key for which the value is to be cached.</param>
        /// <exception cref="InvalidCastException">If the value stored for the specified key is not of type <typeparamref name="TValue"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        TValue TryGet(TKey key);

        /// <summary>
        /// Immediately removes an item from the cache using the specified key.
        /// </summary>
        /// <returns><c>true</c> if an item was found and removed; <c>false</c> if no item existed in the cache with the specified key.</returns>
        /// <param name="key">The key of the item to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        bool Remove(TKey key);

        /// <summary>
        /// Either adds an item to the cache, or replaces an existing item if it already exists in the cache.
        /// </summary>
        /// <param name="key">The key for the item to add/replace.</param>
        /// <param name="value">The item to add/replace.</param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        void AddOrReplace(TKey key, TValue value);
    }
}
