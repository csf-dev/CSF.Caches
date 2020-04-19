//
// ObjectAggregatingCacheKey.cs
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

namespace CSF
{
    /// <summary>
    /// A general-purpose implementation of <see cref="IGetsCacheKey"/> which derives a key from
    /// a number of objects.  Each object is converted to a string and joined into the created
    /// cache key, with a separator value.
    /// </summary>
    public sealed class ObjectAggregatingCacheKey : IGetsCacheKey, IEquatable<ObjectAggregatingCacheKey>
    {
        const string defaultSeparator = "|";

        /// <summary>
        /// Gets the default separator string, which is used to join-together the cache keys from each of the 
        /// </summary>
        /// <value>The default separator.</value>
        public string DefaultSeparator => defaultSeparator;

        /// <summary>
        /// Gets the prefix for this cache key, which is used to
        /// differentiate it from other cache keys which might otherwise be the same.
        /// </summary>
        /// <value>The key prefix.</value>
        public string Prefix { get; }

        /// <summary>
        /// Gets a separator string which is used to separate the parts of the key.
        /// </summary>
        /// <value>The separator string.</value>
        public string Separator { get; }

        /// <summary>
        /// Gets a collection of the object values which will be used to make-up this cache key.
        /// </summary>
        /// <value>The cache key objects.</value>
        public IReadOnlyList<object> Objects { get; }

        /// <summary>
        /// Gets the string cache key representation.  This string should be unique instances of the current type which are considered equal.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A cache key is a representation of the current object instance, for use in a cache (which only
        /// accepts strings as keys).  So, the returned value should follow the same sorts of rules
        /// as equality.  Two instances of the current object which are not equal must return two
        /// different cache keys.  Conversely two different instances which are equal should always return
        /// the same cache key.
        /// </para>
        /// </remarks>
        /// <returns>The cache key.</returns>
        public string GetCacheKey() => $"{Prefix}{Separator}{String.Join(Separator, Objects.Select(GetObjectKey))}";

        /// <summary>
        /// Gets the cache key for a single object instance.
        /// </summary>
        /// <returns>The object's cache key.</returns>
        /// <param name="obj">The object for which to get a cache key.</param>
        string GetObjectKey(object obj)
        {
            if (ReferenceEquals(obj, null))
                return "<null>";

            if(obj is IGetsCacheKey cacheKeyProvider)
                return cacheKeyProvider.GetCacheKey();

            return obj.ToString();
        }

        /// <summary>
        /// Determines whether the specified <see cref="ObjectAggregatingCacheKey"/> is equal
        /// to the current <see cref="ObjectAggregatingCacheKey"/>.
        /// </summary>
        /// <param name="other">The <see cref="ObjectAggregatingCacheKey"/> to compare with the current <see cref="ObjectAggregatingCacheKey"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="ObjectAggregatingCacheKey"/> is equal to the current
        /// <see cref="ObjectAggregatingCacheKey"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(ObjectAggregatingCacheKey other)
        {
            if (ReferenceEquals(other, this)) return true;
            if (ReferenceEquals(other, null)) return false;

            return (Prefix == other.Prefix
                    && Separator == other.Separator
                    && Objects.SequenceEqual(other.Objects));
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="ObjectAggregatingCacheKey"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="ObjectAggregatingCacheKey"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="ObjectAggregatingCacheKey"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => Equals(obj as ObjectAggregatingCacheKey);

        /// <summary>
        /// Serves as a hash function for a <see cref="ObjectAggregatingCacheKey"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            var parts = new List<object> { Prefix, Separator };
            parts.AddRange(Objects);

            unchecked
            {
                // 37 is an arbitrary prime number that I am using to multiply the accumulate
                // before each additional part is combined in.  This means that the hash is
                // certain to change with each part that is added.
                // 
                // 13 is an arbitrary prime number that I will use to represent null.
                // I'm using a nonzero value to ensure that a null will always have an impact
                // upon the hash code.
                return parts.Aggregate(0, (acc, next) => (acc * 37) ^ ((next == null)? 13 : next.GetHashCode()));
            }
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="ObjectAggregatingCacheKey"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that represents the current <see cref="ObjectAggregatingCacheKey"/>.</returns>
        public override string ToString() => $"[{nameof(ObjectAggregatingCacheKey)}: \"{GetCacheKey()}\"]";

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectAggregatingCacheKey"/> class.
        /// </summary>
        /// <param name="prefix">The prefix for cache keys generated by this instance.</param>
        /// <param name="objects">A collection of objects which make up this key.</param>
        public ObjectAggregatingCacheKey(string prefix, params object[] objects) : this(prefix, defaultSeparator, objects) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectAggregatingCacheKey"/> class.
        /// </summary>
        /// <param name="prefix">The prefix for cache keys generated by this instance.</param>
        /// <param name="separator">A separator string to break up the parts of the cache key.</param>
        /// <param name="objects">A collection of objects which make up this key.</param>
        public ObjectAggregatingCacheKey(string prefix, string separator, params object[] objects)
        {
            Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            Separator = separator ?? throw new ArgumentNullException(nameof(separator));
            Objects = objects ?? throw new ArgumentNullException(nameof(objects));
        }
    }
}
