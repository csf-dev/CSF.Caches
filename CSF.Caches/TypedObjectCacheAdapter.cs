//
// TypedObjectCacheAdapter.cs
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
using System.Linq;
using System.Runtime.Caching;
using System.Threading;

namespace CSF
{
    /// <summary>
    /// A type-safe wrapper/adapter around a <see cref="ObjectCache"/> instance.  This implementation reads/writes
    /// data from the underlying cache, providing an alternative API.
    /// </summary>
    public class TypedObjectCacheAdapter<TKey, TValue> : ICachesObjects<TKey, TValue> where TKey : IGetsCacheKey
    {
        readonly ObjectCache cache;
        readonly Func<TKey, TValue, CacheItemPolicy> policyProvider;
        readonly string region;
        readonly ReaderWriterLockSlim syncRoot;

        /// <summary>
        /// Adds a new object to the cache using the specified key.
        /// </summary>
        /// <param name="key">The key for use in caching the value.</param>
        /// <param name="value">The value to cache.</param>
        /// <exception cref="T:CSF.InvalidCacheOperationException">
        /// If an item already exists in the cache with the given key.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="M:CSF.IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        public void Add(TKey key, TValue value)
        {
            var cacheKey = GetCacheKey(key);
            var result = cache.Add(cacheKey, value, GetPolicy(key, value), region);
            if (!result)
                throw new InvalidCacheOperationException($"Cannot add to the cache because an item already exists with the key '{cacheKey}'.");
        }

        /// <summary>
        /// Either adds an item to the cache, or replaces an existing item if it already exists in the cache.
        /// </summary>
        /// <param name="key">The key for the item to add/replace.</param>
        /// <param name="value">The item to add/replace.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="M:CSF.IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        public void AddOrReplace(TKey key, TValue value)
        {
            var cacheKey = GetCacheKey(key);
            cache.Set(cacheKey, value, GetPolicy(key, value), region);
        }

        /// <summary>
        /// Gets a value which indicates whether or not a value is stored for the specified key.
        /// </summary>
        /// <returns><c>true</c> if a value is stored for the key; <c>false</c> otherwise.</returns>
        /// <param name="key">The key for a value.</param>
        public bool Contains(TKey key)
        {
            var cacheKey = GetCacheKey(key);
            return cache.Contains(cacheKey, region);
        }

        /// <summary>
        /// Gets an item from the cache using the specified key.
        /// </summary>
        /// <returns>A value from the cache.</returns>
        /// <param name="key">The key for which the value is to be cached.</param>
        /// <exception cref="T:CSF.InvalidCacheOperationException">If no value is stored within the cache for the specified key.</exception>
        /// <exception cref="T:System.InvalidCastException">If the value stored for the specified key is not of type <typeparamref name="TValue"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="M:CSF.IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        public TValue Get(TKey key)
        {
            var cacheKey = GetCacheKey(key);
            object result = cache.Get(cacheKey, region);
            if(ReferenceEquals(result, null))
                throw new InvalidCacheOperationException($"Cannot get an item with the key '{cacheKey}' because no such item exists in the cache.");
            return (TValue) result;
        }

        /// <summary>
        /// Gets a collection of items which match the specified keys.
        /// Because it is not guaranteed that all the specified <paramref name="keys"/> exist within
        /// the cache, the returned collection may be smaller than the specified collection of keys.
        /// </summary>
        /// <returns>A collection of keys-and-items representing the matching items found in the cache.</returns>
        /// <param name="keys">A collection of keys to retrieve.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// If any item in the <paramref name="keys"/> collection is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// If any item in the <paramref name="keys"/> collection returns <c>null</c> from its
        /// <see cref="M:CSF.IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        public IEnumerable<CacheKeyAndItem<TKey, TValue>> Get(IEnumerable<TKey> keys)
        {
            var cacheKeys = keys
                .Select(x => new { key = x, keyString = GetCacheKey(x)})
                .ToDictionary(k => k.keyString, v => v.key);
            var results = cache.GetValues(cacheKeys.Keys.ToArray(), region);

            return results
                .Where(x => x.Value is TValue)
                .Select(kvp =>
                {
                    var key = cacheKeys[kvp.Key];
                    var value = (TValue)kvp.Value;
                    return new CacheKeyAndItem<TKey, TValue>(key, value);
                })
                .ToArray();
        }


        /// <summary>
        /// <para>
        /// Gets an item from the cache using the specified key.  If it does not exist in the cache then
        /// a <paramref name="valueCreator"/> function is used to create a new value, store it in the
        /// cache and then return it.
        /// </para>
        /// </summary>
        /// <returns>A value from the cache (which might have just been created if it did not exist already).</returns>
        /// <param name="key">The key for which the value is to be cached.</param>
        /// <param name="valueCreator">A function which creates/gets a value for the key, if it does not already exist in the cache.</param>
        /// <exception cref="T:CSF.InvalidCacheOperationException">
        /// If no value is stored within the cache for the specified
        /// key but the implementation was unable to add the item either. This typically points to a concurrency
        /// problem of some kind, specific to the cache implementation.
        /// </exception>
        /// <exception cref="T:System.InvalidCastException">If the value stored for the specified key is not of type <typeparamref name="TValue"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="M:CSF.IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueCreator)
        {
            var cacheKey = GetCacheKey(key);

            try
            {
                syncRoot.EnterUpgradeableReadLock();

                if (cache.Contains(cacheKey, region))
                    return (TValue)cache.Get(cacheKey, region);

                syncRoot.EnterWriteLock();

                var newItem = valueCreator(key);
                var addResult = cache.Add(cacheKey, newItem, GetPolicy(key, newItem), region);
                if (!addResult)
                    throw new InvalidCacheOperationException($"{nameof(GetOrAdd)} did not find an item in the cache for key '{cacheKey}' but was unable to add one either.  Possible concurrent change to underlying cache?");

                return newItem;
            }
            finally
            {
                if (syncRoot.IsUpgradeableReadLockHeld)
                    syncRoot.ExitUpgradeableReadLock();
                if (syncRoot.IsWriteLockHeld)
                    syncRoot.ExitWriteLock();
            }
        }

        /// <summary>
        /// Immediately removes an item from the cache using the specified key.
        /// </summary>
        /// <returns><c>true</c> if an item was found and removed; <c>false</c> if no item existed in the cache with the specified key.</returns>
        /// <param name="key">The key of the item to remove.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="M:CSF.IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        public bool Remove(TKey key)
        {
            var cacheKey = GetCacheKey(key);
            var result = cache.Remove(cacheKey, region);
            return !ReferenceEquals(result, null);
        }

        /// <summary>
        /// Adds a new object to the cache using the specified key, or performs no action if an item
        /// already exists in the cache using the specified key.
        /// </summary>
        /// <returns><c>true</c> if a value was added; <c>false</c> if no action was taken.</returns>
        /// <param name="key">The key for use in caching the value.</param>
        /// <param name="value">The value to cache.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="M:CSF.IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        public bool TryAdd(TKey key, TValue value)
        {
            var cacheKey = GetCacheKey(key);
            return cache.Add(cacheKey, value, GetPolicy(key, value), region);
        }

        /// <summary>
        /// Gets an item from the cache using the specified key, or returns the default for the data-type if
        /// no item exists with the specified key.
        /// </summary>
        /// <returns>A value from the cache, or the default for <typeparamref name="TValue"/>.</returns>
        /// <param name="key">The key for which the value is to be cached.</param>
        /// <exception cref="T:System.InvalidCastException">If the value stored for the specified key is not of type <typeparamref name="TValue"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// If the <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// If the <paramref name="key"/> returns <c>null</c> from its <see cref="M:CSF.IGetsCacheKey.GetCacheKey"/> method.
        /// </exception>
        public TValue TryGet(TKey key)
        {
            var cacheKey = GetCacheKey(key);
            object result = cache.Get(cacheKey, region);
            return ReferenceEquals(result, null) ? default(TValue) : (TValue)result;
        }

        /// <summary>
        /// Gets and sanitises a cache key string.
        /// </summary>
        /// <returns>The cache key string.</returns>
        /// <param name="key">A cache key object.</param>
        string GetCacheKey(TKey key)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException(nameof(key), "A cache key object must not be be null.");
            var cacheKey = key.GetCacheKey();
            if (cacheKey == null)
                throw new ArgumentException($"A cache key's {nameof(IGetsCacheKey.GetCacheKey)} must not return null.");

            return cacheKey;
        }

        /// <summary>
        /// Gets a cache item policy for a key/value pair.
        /// </summary>
        /// <returns>The cache item policy.</returns>
        /// <param name="key">The item key.</param>
        /// <param name="value">The item value.</param>
        CacheItemPolicy GetPolicy(TKey key, TValue value)
            => policyProvider(key, value);

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedObjectCacheAdapter{TKey, TValue}"/> class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Because of the mechanism by which <see cref="GetOrAdd(TKey, Func{TKey, TValue})"/> is implemented, and
        /// because this functionality is not supported in the underlying <see cref="ObjectCache"/> type, an instance
        /// of this type should be long-lived, and shared between consumers.  Whilst an underlying object cache is
        /// itself thread-safe, it cannot create an item to add to the cache using a delayed mechanism (as with
        /// get-or-add).  This means that additional synchronisation logic exists within this class.  Inconsistent
        /// behaviour could occur if multiple instances of <see cref="TypedObjectCacheAdapter{TKey, TValue}"/> access
        /// the same underlying cache, for the same object type.
        /// </para>
        /// </remarks>
        /// <param name="cache">An underlying object cache instance, acting as the data store for this instance.</param>
        /// <param name="policyProvider">
        /// An optional function which determines the cache policy for each item that is added to the cache.
        /// If not provided then a default policy will be used, that adds items to the cache with no expiration/eviction.
        /// </param>
        /// <param name="region">An optional cache region-name.</param>
        public TypedObjectCacheAdapter(ObjectCache cache,
                                       Func<TKey,TValue,CacheItemPolicy> policyProvider = null,
                                       string region = null)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.policyProvider = policyProvider ?? ((k,v) => new CacheItemPolicy());
            this.region = region;

            syncRoot = new ReaderWriterLockSlim();
        }
    }
}
