//
// ObjectAggregatingCacheKeyTests.cs
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
using NUnit.Framework;

namespace CSF.Tests
{
    [TestFixture, Parallelizable]
    public class ObjectAggregatingCacheKeyTests
    {
        [Test, AutoMoqData]
        public void Equals_returns_false_when_compared_object_is_null(ObjectAggregatingCacheKey sut)
        {
            Assert.That(() => sut.Equals(null), Is.False);
        }

        [Test, AutoMoqData]
        public void Equals_returns_false_when_compared_object_is_of_different_type(ObjectAggregatingCacheKey sut)
        {
            Assert.That(() => sut.Equals(5), Is.False);
        }

        [Test, AutoMoqData]
        public void Equals_returns_false_when_compared_object_has_different_prefix(string prefix1, string prefix2, int item)
        {
            var sut = new ObjectAggregatingCacheKey(prefix1, item);
            var other = new ObjectAggregatingCacheKey(prefix2, item);

            Assert.That(() => sut.Equals(other), Is.False);
        }

        [Test, AutoMoqData]
        public void Equals_returns_false_when_compared_object_has_different_separator(string prefix, string separator1, string separator2, int item)
        {
            var sut = new ObjectAggregatingCacheKey(prefix, separator1, item);
            var other = new ObjectAggregatingCacheKey(prefix, separator2, item);

            Assert.That(() => sut.Equals(other), Is.False);
        }

        [Test, AutoMoqData]
        public void Equals_returns_true_when_compared_object_has_same_item(string prefix, int item)
        {
            var sut = new ObjectAggregatingCacheKey(prefix, item);
            var other = new ObjectAggregatingCacheKey(prefix, item);

            Assert.That(() => sut.Equals(other), Is.True);
        }

        [Test, AutoMoqData]
        public void Equals_returns_true_when_compared_object_has_same_three_items_in_same_order(string prefix, int item1, int item2, int item3)
        {
            var sut = new ObjectAggregatingCacheKey(prefix, item1, item2, item3);
            var other = new ObjectAggregatingCacheKey(prefix, item1, item2, item3);

            Assert.That(() => sut.Equals(other), Is.True);
        }

        [Test, AutoMoqData]
        public void Equals_returns_false_when_compared_object_has_same_items_in_different_order(string prefix, int item1, int item2, int item3)
        {
            var sut = new ObjectAggregatingCacheKey(prefix, item1, item2, item3);
            var other = new ObjectAggregatingCacheKey(prefix, item1, item3, item2);

            Assert.That(() => sut.Equals(other), Is.False);
        }

        [Test, AutoMoqData]
        public void GetHashCode_returns_different_hash_code_for_an_extra_item_even_if_null(string prefix, int item)
        {
            var first = new ObjectAggregatingCacheKey(prefix, item);
            var second = new ObjectAggregatingCacheKey(prefix, item, null);

            Assert.That(first.GetHashCode(), Is.Not.EqualTo(second.GetHashCode()));
        }

        [Test, AutoMoqData]
        public void GetCacheKey_returns_correct_key_for_three_items()
        {
            var sut = new ObjectAggregatingCacheKey("The prefix", 1, 2, 3);
            Assert.That(() => sut.GetCacheKey(), Is.EqualTo("The prefix|1|2|3"));
        }
    }
}
