//
// NamespacedRegionCacheKeyProviderTests.cs
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
    [TestFixture,Parallelizable]
    public class NamespacedRegionCacheKeyProviderTests
    {
        [Test, AutoMoqData]
        public void GetCacheKey_used_twice_with_same_parameters_returns_the_same_result(NamespacedRegionCacheKeyProvider sut,
                                                                                        string key, string region)
        {
            var firstKey = sut.GetCacheKey(key, region);
            var secondKey = sut.GetCacheKey(key, region);
            Assert.That(secondKey, Is.EqualTo(firstKey), "Keys should be equal (although their exact values will be indeterminate)");
        }

        [Test, AutoMoqData]
        public void GetCacheKey_used_twice_with_same_original_key_and_null_region_returns_the_same_result(NamespacedRegionCacheKeyProvider sut,
                                                                                                          string key)
        {
            var firstKey = sut.GetCacheKey(key, null);
            var secondKey = sut.GetCacheKey(key, null);
            Assert.That(secondKey, Is.EqualTo(firstKey), "Keys should be equal (although their exact values will be indeterminate)");
        }

        [Test, AutoMoqData]
        public void GetCacheKey_used_twice_on_different_instances_with_same_parameters_returns_different_result(NamespacedRegionCacheKeyProvider sut1,
                                                                                                                NamespacedRegionCacheKeyProvider sut2,
                                                                                                                string key,
                                                                                                                string region)
        {
            var firstKey = sut1.GetCacheKey(key, region);
            var secondKey = sut2.GetCacheKey(key, region);
            Assert.That(secondKey, Is.Not.EqualTo(firstKey), "Keys should not be equal (although their exact values will be indeterminate)");
        }

        [Test, AutoMoqData]
        public void GetCacheKey_used_twice_on_different_instances_with_same_original_key_and_null_region_returns_different_result(NamespacedRegionCacheKeyProvider sut1,
                                                                                                                                  NamespacedRegionCacheKeyProvider sut2,
                                                                                                                                  string key)
        {
            var firstKey = sut1.GetCacheKey(key, null);
            var secondKey = sut2.GetCacheKey(key, null);
            Assert.That(secondKey, Is.Not.EqualTo(firstKey), "Keys should not be equal (although their exact values will be indeterminate)");
        }

        [Test, AutoMoqData]
        public void GetCacheKey_used_twice_on_different_instances_with_same_parameters_returns_the_same_result_if_second_is_constructed_using_values_from_first(NamespacedRegionCacheKeyProvider sut,
                                                                                                                                                                string key,
                                                                                                                                                                string region)
        {
            var firstKey = sut.GetCacheKey(key, region);
            var sut2 = new NamespacedRegionCacheKeyProvider(sut.RegionNamesToKeys, sut.NullRegionKey);
            var secondKey = sut2.GetCacheKey(key, region);
            Assert.That(secondKey, Is.EqualTo(firstKey), "Keys should be equal (although their exact values will be indeterminate)");
        }

    }
}
