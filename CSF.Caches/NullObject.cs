//
// NullObject.cs
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
    /// This is a special object (used as a singleton) which represents <c>null</c> in the cache.
    /// </summary>
    [Serializable]
    internal sealed class NullObject : IEquatable<NullObject>
    {
        /// <summary>
        /// Determines whether the specified <see cref="NullObject"/> is equal to the current <see cref="NullObject"/>.
        /// </summary>
        /// <param name="other">The <see cref="NullObject"/> to compare with the current <see cref="NullObject"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="NullObject"/> is equal to the current
        /// <see cref="NullObject"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(NullObject other) => other is NullObject;

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="NullObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="NullObject"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="NullObject"/>;
        /// otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is NullObject;

        /// <summary>
        /// Serves as a hash function for a <see cref="NullObject"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode() => 0;

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="NullObject"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that represents the current <see cref="NullObject"/>.</returns>
        public override string ToString() => $"[{nameof(NullObject)}]";

        /// <summary>
        /// Initializes a new instance of the <see cref="NullObject"/> class.
        /// </summary>
        private NullObject() { }

        /// <summary>
        /// Initializes the <see cref="NullObject"/> class.
        /// </summary>
        static NullObject()
        {
            Instance = new NullObject();
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="NullObject"/>.
        /// </summary>
        /// <value>The instance.</value>
        internal static NullObject Instance { get; private set; }
    }
}
