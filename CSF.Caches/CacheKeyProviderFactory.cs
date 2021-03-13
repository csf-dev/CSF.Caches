//
// CacheKeyProviderFactory.cs
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

namespace CSF
{
    /// <summary>
    /// A factory service which stores a key-prefix and separator character, for the
    /// creation of instances of <see cref="ObjectAggregatingCacheKey"/>.  This allows
    /// consumers to create cache keys from collections of objects without concerning
    /// themselves with the prefix or separator strings.
    /// </summary>
    public class CacheKeyProviderFactory : IGetsCacheKeyProvider
    {
        /// <summary>
        /// Gets the key prefix.
        /// </summary>
        /// <value>The key prefix.</value>
        public string KeyPrefix { get; }

        /// <summary>
        /// Gets the separator.
        /// </summary>
        /// <value>The separator.</value>
        public string Separator { get; }

        /// <summary>
        /// Gets the cache key provider from the specified objects.
        /// </summary>
        /// <returns>The cache key provider.</returns>
        /// <param name="objects">A collection of objects.</param>
        public IGetsCacheKey GetCacheKeyProvider(params object[] objects)
            => new ObjectAggregatingCacheKey(KeyPrefix, Separator, objects);

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKeyProviderFactory"/> class.
        /// </summary>
        /// <param name="keyPrefix">Key prefix.</param>
        /// <param name="separator">Separator.</param>
        public CacheKeyProviderFactory(string keyPrefix, string separator = "|")
        {
            KeyPrefix = keyPrefix ?? throw new ArgumentNullException(nameof(keyPrefix));
            Separator = separator;
        }
    }
}
