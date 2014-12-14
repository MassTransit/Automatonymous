namespace Automatonymous.Events
{
    using System;
    using System.Collections.Concurrent;


    public class PayloadCache
    {
        readonly ConcurrentDictionary<Type, CachedPayload> _cache;

        public PayloadCache(int capacity = 2)
        {
            _cache = new ConcurrentDictionary<Type, CachedPayload>(2, capacity);
        }

        public bool HasPayloadType(Type contextType)
        {
            return _cache.ContainsKey(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            CachedPayload payloadCache;
            if (_cache.TryGetValue(typeof(TPayload), out payloadCache))
            {
                context = payloadCache.GetPayload<TPayload>();
                return true;
            }

            context = default(TPayload);
            return false;
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            CachedPayload cachedPayload;
            if (_cache.TryGetValue(typeof(TPayload), out cachedPayload))
                return cachedPayload.GetPayload<TPayload>();

            try
            {
                cachedPayload = _cache.GetOrAdd(typeof(TPayload), x =>
                {
                    TPayload payload = payloadFactory();
                    if (payload != default(TPayload))
                        return new CachedPayload<TPayload>(payload);

                    throw new PayloadNotFoundException("The payload was not found: " + typeof(TPayload).Name);
                });

                return cachedPayload.GetPayload<TPayload>();
            }
            catch (PayloadNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PayloadFactoryException("The payload factory faulted: " + typeof(TPayload).Name, ex);
            }
        }


        interface CachedPayload
        {
            T GetPayload<T>()
                where T : class;
        }


        class CachedPayload<TPayload> :
            CachedPayload
            where TPayload : class
        {
            readonly TPayload _payload;

            public CachedPayload(TPayload payload)
            {
                _payload = payload;
            }

            public T GetPayload<T>()
                where T : class
            {
                var payload = this as CachedPayload<T>;
                if (payload != null)
                    return payload._payload;

                throw new PayloadException("Payload type mismatch: " + typeof(T).Name);
            }
        }
    }
}