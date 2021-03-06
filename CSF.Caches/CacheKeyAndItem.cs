﻿//
// CacheKeyAndItem.cs
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
namespace CSF
{
    /// <summary>
    /// Represents a cached item and its key.
    /// </summary>
    public class CacheKeyAndItem<TKey,TValue> where TKey : IGetsCacheKey
    {
        /// <summary>
        /// The cache key.
        /// </summary>
        /// <value>The key.</value>
        public TKey Key { get; }

        /// <summary>
        /// The cache item
        /// </summary>
        /// <value>The item.</value>
        public TValue Item { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKeyAndItem{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="item">Item.</param>
        public CacheKeyAndItem(TKey key, TValue item)
        {
            Key = key;
            Item = item;
        }
    }
}
