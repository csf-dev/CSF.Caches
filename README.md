# Archived: Use MS Extensions Caching instead

This library is archived and no longer maintained.
The reason is that Microsoft have released [Microsoft.Extensions.Caching.Abstractions] which provides far superior support and a very similar API for caching objects in a simple manner.
In particular, former users of this library should consider migrating to [Microsoft.Extensions.Caching.Memory.IMemoryCache].

[Microsoft.Extensions.Caching.Memory.IMemoryCache]: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.imemorycache
[Microsoft.Extensions.Caching.Abstractions]: https://www.nuget.org/packages/Microsoft.Extensions.Caching.Abstractions/

---

## Typed cache adapter
This package provides `ICachesObjects<TKey,TValue>`, which is a type-safe cache implementation.
The implementation `TypedObjectCacheAdapter<TKey,TValue>` is an [adapter] for instances of [`System.Runtime.Caching.ObjectCache`].

The primary motivations for this cache interface are:
* Type safety
* Simplification of the API exposed to consumers

The simplification is achieved by configuring the caching policy and storage regions from the constructor, rather than requiring consumers to pass this information directly.

[adapter]: https://en.wikipedia.org/wiki/Adapter_pattern
[`System.Runtime.Caching.ObjectCache`]: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.caching.objectcache

## Namespaced-region cache decorator
The class `NamespacedRegionCacheDecorator` is [decorator] for [`System.Runtime.Caching.ObjectCache`] which may be used to add **region** support to `ObjectCache` implementations that do not otherwise support them.
This is achieved cheaply by prepending *namespaced strings* to the cache keys in the underlying implementation.
The namespace identifiers make use of `Guid` keys, making them very difficult to predict and thus reasonably safe from key-collision attacks.  Please see the remarks upon the class itself for more information.

[decorator]: https://en.wikipedia.org/wiki/Decorator_pattern

## `TypedObjectCacheAdapter` usage
In your application, you should have one or more instances of `ObjectCache`, which are used as the 'backing stores' for the data you wish to cache.  For each object type you wish to cache (key/value pairs), create a single instance of `TypedObjectCacheAdapter<TKey,TValue>`, wrapping the appropriate object cache.

### Constructing & sharing instances
When constructing the cache adapter, you may provide a callback which generates a `CacheItemPolicy` for items which are added to the cache.  You may also optionally choose a cache `region`, assuming the underlying object cache implementation supports it.

A single instance of the cache adapter should have a long-lifetime, usually for the whole lifetime of the application.  Ideally, *use dependency injection to share a single instance* of the cache adapter between any types which need to consume it.

### Cache key types
Because the underlying `ObjectCache` class requires **string** keys to store instances, any class used as a cache key, must implement [the interface `IGetsCacheKey`].

[the interface `IGetsCacheKey`]: https://github.com/csf-dev/CSF.Caches/blob/master/CSF.Caches/IGetsCacheKey.cs

### Further documentation
The API for the cache is documented [in the interface `ICachesObjects<TKey,TValue>`].

[in the interface `ICachesObjects<TKey,TValue>`]: https://github.com/csf-dev/CSF.Caches/blob/master/CSF.Caches/ICachesObjects.cs

## `NamespacedRegionCacheDecorator` usage
To use this class, simply construct it wrapping an existing object cache instance.
You should either keep this object as a singleton within your application or you must ensure that you manually construct and keep the accompanying instance of `IGetsRegionBasedCacheKeys` (for example `NamespacedRegionCacheKeyProvider`).  This key-provider object maintains the map of region names to their Guid keys.  If that is lost, then the cached items will all become unreachable, because their true keys in the underlying cache will be lost.

## Open source license
All source files within this project are released as open source software,
under the terms of [the MIT license].

[the MIT license]: http://opensource.org/licenses/MIT
