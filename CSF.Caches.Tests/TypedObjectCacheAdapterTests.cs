//
// TypedObjectCacheAdapterTests.cs
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
using System.Linq;
using System.Runtime.Caching;
using NUnit.Framework;

namespace CSF.Tests
{
    [TestFixture,Parallelizable]
    public class TypedObjectCacheAdapterTests
    {
        [Test, AutoMoqData]
        public void Add_then_get_returns_the_correct_object(Person person)
        {
            var sut = GetSut();
            sut.Add(person.GetKey(), person);
            var result = sut.Get(person.GetKey());

            Assert.That(result, Is.EqualTo(person));
        }

        [Test, AutoMoqData]
        public void Add_throws_InvalidCacheOperationException_if_used_twice_for_the_same_key(Person person)
        {
            var sut = GetSut();
            sut.Add(person.GetKey(), person);
            
            Assert.That(() => sut.Add(person.GetKey(), person), Throws.InstanceOf<InvalidCacheOperationException>());
        }

        [Test, AutoMoqData]
        public void TryAdd_returns_true_if_used_once(Person person)
        {
            var sut = GetSut();
            Assert.That(() => sut.TryAdd(person.GetKey(), person), Is.True);
        }

        [Test, AutoMoqData]
        public void TryAdd_returns_true_if_used_twice_for_the_same_key(Person person)
        {
            var sut = GetSut();
            sut.TryAdd(person.GetKey(), person);
            Assert.That(() => sut.TryAdd(person.GetKey(), person), Is.False);
        }

        [Test, AutoMoqData]
        public void GetOrAdd_uses_factory_if_item_not_in_cache(Person person)
        {
            var sut = GetSut();
            var factoryUsed = false;
            Person GetPerson(PersonKey key) { factoryUsed = true; return person; };

            sut.GetOrAdd(person.GetKey(), GetPerson);
            Assert.That(factoryUsed, Is.True);
        }

        [Test, AutoMoqData]
        public void GetOrAdd_does_not_use_factory_if_item_is_in_cache(Person person)
        {
            var sut = GetSut();
            var factoryUsed = false;
            Person GetPerson(PersonKey key) { factoryUsed = true; return person; };

            sut.Add(person.GetKey(), person);
            sut.GetOrAdd(person.GetKey(), GetPerson);
            Assert.That(factoryUsed, Is.False);
        }

        [Test, AutoMoqData]
        public void Contains_returns_false_if_item_not_in_cache(Person person)
        {
            var sut = GetSut();
            Assert.That(() => sut.Contains(person.GetKey()), Is.False);
        }

        [Test, AutoMoqData]
        public void Contains_returns_true_if_item_is_in_cache(Person person)
        {
            var sut = GetSut();
            sut.Add(person.GetKey(), person);
            Assert.That(() => sut.Contains(person.GetKey()), Is.True);
        }

        [Test, AutoMoqData]
        public void Get_returns_item_if_it_is_in_cache(Person person)
        {
            var sut = GetSut();
            sut.Add(person.GetKey(), person);
            Assert.That(() => sut.Get(person.GetKey()), Is.EqualTo(person));
        }

        [Test, AutoMoqData]
        public void Get_throws_InvalidCacheOperationException_if_it_is_not_in_cache(Person person)
        {
            var sut = GetSut();
            Assert.That(() => sut.Get(person.GetKey()), Throws.InstanceOf<InvalidCacheOperationException>());
        }

        [Test, AutoMoqData]
        public void TryGet_returns_true_item_if_it_is_in_cache(Person person)
        {
            var sut = GetSut();
            sut.Add(person.GetKey(), person);
            Person result = null;
            Assert.That(() => sut.TryGet(person.GetKey(), out result), Is.True);
            Assert.That(result, Is.SameAs(person));
        }

        [Test, AutoMoqData]
        public void TryGet_returns_false_if_it_is_not_in_cache(Person person)
        {
            var sut = GetSut();
            Person result = null;
            Assert.That(() => sut.TryGet(person.GetKey(), out result), Is.False);
            Assert.That(result, Is.Not.SameAs(person));
        }

        [Test, AutoMoqData]
        public void Get_multiple_can_return_two_items_from_the_cache(Person person1, Person person2)
        {
            var sut = GetSut();
            sut.Add(person1.GetKey(), person1);
            sut.Add(person2.GetKey(), person2);
            Assert.That(() => sut.Get(new[] { person1.GetKey(), person2.GetKey() })?.Select(x => x.Item).ToList(), Is.EqualTo(new[] { person1, person2 }));
        }

        [Test, AutoMoqData]
        public void Remove_returns_true_if_an_item_was_removed(Person person)
        {
            var sut = GetSut();
            sut.Add(person.GetKey(), person);
            Assert.That(() => sut.Remove(person.GetKey()), Is.True);
        }

        [Test, AutoMoqData]
        public void Remove_returns_false_if_no_item_was_removed(Person person)
        {
            var sut = GetSut();
            Assert.That(() => sut.Remove(person.GetKey()), Is.False);
        }

        [Test, AutoMoqData]
        public void AddOrReplace_can_replace_an_object(Person person1, Person person2)
        {
            var sut = GetSut();
            sut.Add(person1.GetKey(), person1);
            sut.AddOrReplace(person1.GetKey(), person2);
            Assert.That(() => sut.Get(person1.GetKey()), Is.EqualTo(person2));
        }

        ICachesObjects<PersonKey, Person> GetSut(ObjectCache cache = null)
            => new TypedObjectCacheAdapter<PersonKey, Person>(cache ?? new MemoryCache(Guid.NewGuid().ToString()));

        #region contained types

        public class Person : IEquatable<Person>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }

            public override bool Equals(object obj)
                => Equals(obj as Person);

            public override int GetHashCode()
            {
                return Id.GetHashCode()
                     ^ Name.GetHashCode()
                     ^ Age.GetHashCode();
            }

            public bool Equals(Person other)
            {
                if (ReferenceEquals(other, null)) return false;
                if (ReferenceEquals(other, this)) return true;

                return other.Id == Id
                    && other.Name == Name
                    && other.Age == Age;
            }

            public PersonKey GetKey() => new PersonKey { Id = Id };
        }

        public class PersonKey : IGetsCacheKey
        {
            public Guid Id { get; set; }

            public string GetCacheKey() => Id.ToString();
        }

        #endregion
    }
}
