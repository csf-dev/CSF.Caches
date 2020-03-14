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
