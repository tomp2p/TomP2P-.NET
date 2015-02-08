﻿using System;
using System.Collections.Concurrent;
using System.Threading;

namespace TomP2P.Connection.Windows.Netty
{
    public class DefaultAttributeMap : IAttributeMap
    {
        private ConcurrentDictionary<AttributeKey, DefaultAttribute> _attributes;

        /// <summary>
        /// Gets the attribute for the given attribute key.
        /// This method will never return null, but may return 
        /// an attribute which does not have a value set yet.

        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public IAttribute<T> Attr<T>(AttributeKey<T> key)
        {
            if (key == null)
            {
                throw new NullReferenceException("key");
            }

            var attributes = _attributes;
            if (attributes == null)
            {
                attributes = new ConcurrentDictionary<AttributeKey, DefaultAttribute>();

                // atomically: if _attributes == null -> replace it with attributes
                Interlocked.CompareExchange(ref _attributes, attributes, null);
            }

            var attr = attributes.GetOrAdd(key, new DefaultAttribute(key));
            return (IAttribute<T>) attr;
        }

        private sealed class DefaultAttribute<T> : DefaultAttribute, IAttribute<T>
        {
            private readonly AttributeKey<T> _key;
            private T _value;
            private readonly object _lock = new object();

            internal DefaultAttribute(AttributeKey<T> key)
                : base(key)
            {
                _key = key;
            }

            public AttributeKey<T> Key
            {
                get { return _key; }
            }

            public T Get()
            {
                lock (_lock)
                {
                    return _value;
                }
            }

            public void Set(T value)
            {
                lock (_lock)
                {
                    _value = value;
                }
            }
        }

        // TODO finish implementation of DefaultAttribute
        private class DefaultAttribute
        {
            internal DefaultAttribute(AttributeKey key)
            {
                // ignore
            }
        }
    }
}