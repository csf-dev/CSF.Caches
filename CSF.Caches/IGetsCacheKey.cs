//
// IGetsCacheKey.cs
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
    /// An object which can get a <see cref="String"/> cache key, for storing values in a cache.
    /// Please consider implementing <see cref="IEquatable{T}"/> as well as this interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Good implementations of <see cref="IGetsCacheKey"/> are immutable, at least with regard
    /// to the <see cref="GetCacheKey"/> function.  That means that it should be impossible to change
    /// (aka mutate) the object in any way which would mean that it could later generate a different
    /// cache key.  For example, the cache key should be based only on fields/properties which are
    /// read-only.
    /// </para>
    /// </remarks>
    public interface IGetsCacheKey
    {
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
        string GetCacheKey();
    }
}
