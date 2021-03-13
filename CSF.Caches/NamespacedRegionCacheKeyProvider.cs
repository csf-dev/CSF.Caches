//
// NamespacedCacheKeyProvider.cs
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
using System.Collections.Concurrent;

namespace CSF
{
    /// <summary>
    /// Implementation of <see cref="IGetsRegionBasedCacheKeys"/> which uses a map/dictionary
    /// to associate region names <see cref="Guid"/> strings (plus an additional Guid string for
    /// null region names).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The choice to use <see cref="Guid"/> instead of just the region name alone is to make it
    /// more difficult for an attacker to generate cache collisions by using crafted data.
    /// Because the Guids are typically generated using the application lifetime and are ephemeral
    /// then they will be very difficult to predict (without significantly greater access to the
    /// computer running this process).
    /// </para>
    /// <para>
    /// Also be aware that - because the map of region-names to region-keys (as well as the null
    /// region key) is held using in-process memory - this approach will be of limited use with
    /// out-of-process cache implementations.  In other words, this is a 'cheap way to support
    /// cache regions' for <see cref="System.Runtime.Caching.MemoryCache"/> but not a particularly
    /// scaleable one.
    /// </para>
    /// <para>
    /// Conceivably the problem described above could be solved by additionally storing the
    /// <see cref="RegionNamesToKeys"/> and <see cref="NullRegionKey"/> within th out-of-process
    /// cache, at well-known keys.  These would then be retrieved when the process starts/restarts,
    /// and passed into the constructor of this class.  This would allow an implementation to restart
    /// without 'losing' the association between region names and their keys.
    /// </para>
    /// </remarks>
    public class NamespacedRegionCacheKeyProvider : IGetsRegionBasedCacheKeys
    {
        /// <summary>
        /// Gets a map of the region names and their corresponding region keys.
        /// The keys are used to add unpredictable data into the namespace which is
        /// prepended to the original cache key, making collisions less likely.
        /// </summary>
        /// <value>The region-names/region-keys map.</value>
        public ConcurrentDictionary<string, string> RegionNamesToKeys { get; }

        /// <summary>
        /// Gets a region key representing the null region.
        /// </summary>
        /// <value>The null region-key.</value>
        public string NullRegionKey { get; }

        /// <summary>
        /// Gets the cache key for the specified original key and region name.
        /// </summary>
        /// <returns>The modified cache key.</returns>
        /// <param name="key">The original cache key.</param>
        /// <param name="regionName">The region name.</param>
        public string GetCacheKey(string key, string regionName)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return $"[{nameof(NamespacedRegionCacheKeyProvider)}(Region {regionName ?? "<null>"}, Key '{GetRegionKey(regionName)}')] {key}";
        }

        string GetRegionKey(string regionName)
        {
            if (regionName == null) return NullRegionKey;
            return RegionNamesToKeys.GetOrAdd(regionName, name => Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespacedRegionCacheKeyProvider"/> class.
        /// This will generate a new <see cref="RegionNamesToKeys"/> collection and a new <see cref="NullRegionKey"/>.
        /// </summary>
        public NamespacedRegionCacheKeyProvider()
        {
            RegionNamesToKeys = new ConcurrentDictionary<string, string>();
            NullRegionKey = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespacedRegionCacheKeyProvider"/> class.
        /// This will use an existing <paramref name="regionNamesToKeys"/> and <paramref name="nullRegionKey"/>.
        /// </summary>
        /// <param name="regionNamesToKeys">The region-names/region-keys map.</param>
        /// <param name="nullRegionKey">The null region-key.</param>
        public NamespacedRegionCacheKeyProvider(ConcurrentDictionary<string, string> regionNamesToKeys, string nullRegionKey)
        {
            RegionNamesToKeys = regionNamesToKeys ?? throw new ArgumentNullException(nameof(regionNamesToKeys));
            NullRegionKey = nullRegionKey ?? throw new ArgumentNullException(nameof(nullRegionKey));
        }
    }
}
