As of version 2.0.0 of this package, this provides a type-safe wrapper/adapter around an instance of  `System.Runtime.Caching.ObjectCache` ([see the documentation on the Microsoft API website]).  The API provided by the type `CSF.TypedObjectCacheAdapter<TKey,TValue>` provides an easy-to-consume caching API.  Details of cache policy and storage regions (where supported) are moved to the constructor of the cache adapter and are removed from the public interface/API for using the cache.

[see the documentation on the Microsoft API website]: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.caching.objectcache

## Usage
In your application, you should have one or more instances of `ObjectCache`, which are used as the 'backing stores' for the data you wish to cache.  For each object type you wish to cache (key/value pairs), create a single instance of `TypedObjectCacheAdapter<TKey,TValue>`, wrapping the appropriate object cache.

### Constructing & sharing instances
When constructing the cache adapter, you may provide a callback which generates a `CacheItemPolicy` for items which are added to the cache.  You may also optionally choose a cache `region`, assuming the underlying object cache implementation supports it.

A single instance of the cache adapter should have a long-lifetime, usually for the whole lifetime of the application.  Ideally, *use dependency injection to share a single instance* of the cache adapter between any types which need to consume it.

### Cache key types
Because the underlying `ObjectCache` class requires **string** keys to store instances, any class used as a cache key, must implement [the interface `IGetsCacheKey`].

[the interface `IGetsCacheKey`]: https://github.com/csf-dev/CSF.Caches/blob/master/CSF.Caches/IGetsCacheKey.cs

### Further documentation
The API for the cache is documented [in the interface `ICachesObjects`]..

[in the interface `ICachesObjects`]: https://github.com/csf-dev/CSF.Caches/blob/master/CSF.Caches/ICachesObjects.cs

## Open source license
All source files within this project are released as open source software,
under the terms of [the MIT license].

[the MIT license]: http://opensource.org/licenses/MIT
