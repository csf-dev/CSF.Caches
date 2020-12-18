//
// NamespacedRegionCacheDecorator.cs
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

namespace CSF
{
    /// <summary>
    /// A wrapper/decorator for an <see cref="ObjectCache"/> which adds 'cache region'
    /// support to those which do not already support it.  This is achieved by prepending
    /// a namespace to the cache key in order to avoid key collisions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Please note that this class is not a perfect solution to adding region support to a cache
    /// implementation that does not already support it.  Please read the full remarks from
    /// <see cref="NamespacedRegionCacheKeyProvider"/> (even if you are not using that precise
    /// implementation of <see cref="IGetsRegionBasedCacheKeys"/>).
    /// </para>
    /// <para>
    /// This class adds region support by using an <see cref="IGetsRegionBasedCacheKeys"/> instance
    /// to combine the cache key and the region name for every supported operation, transforming
    /// this into a modified cache key.  The modified cache key is then used with the same operation
    /// in the wrapped <see cref="ObjectCache"/> implementation, but with the region name always
    /// specified as <see langword="null"/>.
    /// </para>
    /// <para>
    /// This is probably suitable only for very simple &amp; small in-process caching scenarios,
    /// such as those which use <see cref="MemoryCache"/> or for proof-of-concept/prototyping purposes.
    /// It's expected that caching requirements could outstrip the capabilities of this class and
    /// that an alternative implementation will be sought, with native region support.
    /// </para>
    /// </remarks>
    public class NamespacedRegionCacheDecorator : ObjectCache
    {
        readonly ObjectCache wrapped;
        readonly IGetsRegionBasedCacheKeys cacheKeyProvider;

        /// <summary>
        /// Throws <see cref="NotSupportedException"/>.  Use the <see cref="Get(string, string)"/> or
        /// <see cref="Set(string, object, CacheItemPolicy, string)"/> methods instead.
        /// </summary>
        /// <param name="key">The cache key.</param>
        public override object this[string key]
        {
            get => throw new NotSupportedException($"Get & set via indexer is not supported in this implementation; use {nameof(Get)} or {nameof(Set)} methods instead.");
            set => throw new NotSupportedException($"Get & set via indexer is not supported in this implementation; use {nameof(Get)} or {nameof(Set)} methods instead.");
        }

        /// <summary>When overridden in a derived class, gets a description of the features that a cache implementation provides.</summary>
        /// <returns>A bitwise combination of flags that indicate the default capabilities of a cache implementation.</returns>
        public override DefaultCacheCapabilities DefaultCacheCapabilities
            => wrapped.DefaultCacheCapabilities | DefaultCacheCapabilities.CacheRegions;

        /// <summary>Gets the name of a specific <see cref="T:System.Runtime.Caching.ObjectCache" /> instance.</summary>
        /// <returns>The name of a specific cache instance.</returns>
        public override string Name => wrapped.Name;

        /// <summary>When overridden in a derived class, inserts the specified <see cref="T:System.Runtime.Caching.CacheItem" /> object into the cache, specifying information about how the entry will be evicted.</summary>
        /// <param name="value">The object to insert.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        /// <returns>If a cache entry with the same key exists, the specified cache entry; otherwise, <see langword="null" />.</returns>
        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
            => wrapped.AddOrGetExisting(GetItem(value), policy);

        /// <summary>When overridden in a derived class, inserts a cache entry into the cache, by using a key, an object for the cache entry, an absolute expiration value, and an optional region to add the cache into.</summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is <see langword="null" />.</param>
        /// <returns>If a cache entry with the same key exists, the specified cache entry's value; otherwise, <see langword="null" />.</returns>
        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
            => wrapped.AddOrGetExisting(GetKey(key, regionName), value, absoluteExpiration);

        /// <summary>When overridden in a derived class, inserts a cache entry into the cache, specifying a key and a value for the cache entry, and information about how the entry will be evicted.</summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is <see langword="null" />.</param>
        /// <returns>If a cache entry with the same key exists, the specified cache entry's value; otherwise, <see langword="null" />.</returns>
        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
            => wrapped.AddOrGetExisting(GetKey(key, regionName), value, policy);

        /// <summary>When overridden in a derived class, checks whether the cache entry already exists in the cache.</summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="regionName">Optional. A named region in the cache where the cache can be found, if regions are implemented. The default value for the optional parameter is <see langword="null" />.</param>
        /// <returns>
        /// <see langword="true" /> if the cache contains a cache entry with the same key value as <paramref name="key" />; otherwise, <see langword="false" />.</returns>
        public override bool Contains(string key, string regionName = null)
            => wrapped.Contains(GetKey(key, regionName));

        /// <summary>When overridden in a derived class, creates a <see cref="T:System.Runtime.Caching.CacheEntryChangeMonitor" /> object that can trigger events in response to changes to specified cache entries.</summary>
        /// <param name="keys">The unique identifiers for cache entries to monitor.</param>
        /// <param name="regionName">Optional. A named region in the cache where the cache keys in the <paramref name="keys" /> parameter exist, if regions are implemented. The default value for the optional parameter is <see langword="null" />.</param>
        /// <returns>A change monitor that monitors cache entries in the cache.</returns>
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
            => wrapped.CreateCacheEntryChangeMonitor(keys.Select(x => GetKey(x, regionName)).ToList());

        /// <summary>When overridden in a derived class, gets the specified cache entry from the cache as an object.</summary>
        /// <param name="key">A unique identifier for the cache entry to get.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry was added, if regions are implemented. The default value for the optional parameter is <see langword="null" />.</param>
        /// <returns>The cache entry that is identified by <paramref name="key" />.</returns>
        public override object Get(string key, string regionName = null)
            => wrapped.Get(GetKey(key, regionName));

        /// <summary>When overridden in a derived class, gets the specified cache entry from the cache as a <see cref="T:System.Runtime.Caching.CacheItem" /> instance.</summary>
        /// <param name="key">A unique identifier for the cache entry to get.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache was added, if regions are implemented. Because regions are not implemented in .NET Framework 4, the default is <see langword="null" />.</param>
        /// <returns>The cache entry that is identified by <paramref name="key" />.</returns>
        public override CacheItem GetCacheItem(string key, string regionName = null)
            => wrapped.GetCacheItem(GetKey(key, regionName));

        /// <summary>
        /// Returns the count of items in the cache if the <paramref name="regionName"/> is <see langword="null"/>.
        /// Throws <see cref="NotSupportedException"/> if the region name is non-null.
        /// </summary>
        /// <returns>The count of items.</returns>
        /// <param name="regionName">The region name.</param>
        public override long GetCount(string regionName = null)
        {
            if (regionName != null) throw new NotSupportedException($"{nameof(GetCount)} is not supported with a non-null region name.");
            return wrapped.GetCount();
        }

        /// <summary>When overridden in a derived class, gets a set of cache entries that correspond to the specified keys.</summary>
        /// <param name="keys">A collection of unique identifiers for the cache entries to get.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry or entries were added, if regions are implemented. The default value for the optional parameter is <see langword="null" />.</param>
        /// <returns>A dictionary of key/value pairs that represent cache entries.</returns>
        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
            => wrapped.GetValues(keys.Select(x => GetKey(x, regionName)).ToList());

        /// <summary>When overridden in a derived class, removes the cache entry from the cache.</summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry was added, if regions are implemented. The default value for the optional parameter is <see langword="null" />.</param>
        /// <returns>An object that represents the value of the removed cache entry that was specified by the key, or <see langword="null" /> if the specified entry was not found.</returns>
        public override object Remove(string key, string regionName = null)
            => wrapped.Remove(GetKey(key, regionName));

        /// <summary>When overridden in a derived class, inserts the cache entry into the cache as a <see cref="T:System.Runtime.Caching.CacheItem" /> instance, specifying information about how the entry will be evicted.</summary>
        /// <param name="item">The cache item to add.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        public override void Set(CacheItem item, CacheItemPolicy policy)
            => wrapped.Set(GetItem(item), policy);

        /// <summary>When overridden in a derived class, inserts a cache entry into the cache, specifying time-based expiration details.</summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is <see langword="null" />.</param>
        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
            => wrapped.Set(GetKey(key, regionName), value, absoluteExpiration);

        /// <summary>When overridden in a derived class, inserts a cache entry into the cache.</summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is <see langword="null" />.</param>
        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
            => wrapped.Set(GetKey(key, regionName), value, policy);

        /// <summary>
        /// Throws <see cref="NotSupportedException"/>.  This method is not supported in this implementation.
        /// </summary>
        /// <returns>Nothing (because this method always throws).</returns>
        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => throw new NotSupportedException($"{nameof(GetEnumerator)} is not supported in this implementation.");

        string GetKey(string key, string region) => cacheKeyProvider.GetCacheKey(key, region);

        CacheItem GetItem(CacheItem item) => new CacheItem(GetKey(item.Key, item.RegionName), item.Value);

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespacedRegionCacheDecorator"/> class.
        /// </summary>
        /// <param name="wrapped">The wrapped cache, which will be enhanced with region support.</param>
        /// <param name="cacheKeyProvider">An optional cache key provider.  If none is specified then a
        /// new instance of <see cref="NamespacedRegionCacheKeyProvider"/> will be used.</param>
        public NamespacedRegionCacheDecorator(ObjectCache wrapped, IGetsRegionBasedCacheKeys cacheKeyProvider = null)
        {
            this.wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            this.cacheKeyProvider = cacheKeyProvider ?? new NamespacedRegionCacheKeyProvider();
        }
    }
}
