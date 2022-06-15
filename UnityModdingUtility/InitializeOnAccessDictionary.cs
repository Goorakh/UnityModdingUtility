﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace UnityModdingUtility
{
    public class InitializeOnAccessDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        public delegate bool ValueSelectorTryGet(TKey key, out TValue value);

        protected Dictionary<TKey, TValue> _internalDict;
        readonly ValueSelectorTryGet _valueSelector;

        protected object _syncRoot;

        protected InitializeOnAccessDictionary()
        {
            _internalDict = new Dictionary<TKey, TValue>();
        }

        public InitializeOnAccessDictionary(ValueSelectorTryGet selector) : this()
        {
            _valueSelector = selector;
        }

        public InitializeOnAccessDictionary(Func<TKey, TValue> selector) : this((TKey key, out TValue value) => { value = selector(key); return true; })
        {
        }

        public InitializeOnAccessDictionary(Func<TValue> selector) : this((TKey key, out TValue value) => { value = selector(); return true; })
        {
        }

        public virtual TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                    return value;

                throw new KeyNotFoundException($"Key {key} is not in the dictionary and no value was decided");
            }
            set
            {
                _internalDict[key] = value;
            }
        }

        public ICollection<TKey> Keys => _internalDict.Keys;
        public ICollection<TValue> Values => _internalDict.Values;
        public int Count => _internalDict.Count;
        public bool IsReadOnly => false;

        ICollection IDictionary.Keys => _internalDict.Keys;
        ICollection IDictionary.Values => _internalDict.Values;
        public bool IsFixedSize => false;
        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);

                return _syncRoot;
            }
        }
        public bool IsSynchronized => false;

        public object this[object key] { get => this[(TKey)key]; set => this[(TKey)key] = (TValue)value; }

        public void Add(TKey key, TValue value)
        {
            _internalDict.Add(key, value);
        }

        public void Clear()
        {
            _internalDict.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return _internalDict.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _internalDict.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            return _internalDict.Remove(key);
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            if (_internalDict.TryGetValue(key, out value))
                return true;

            if (_valueSelector(key, out value))
            {
                _internalDict.Add(key, value);
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object key)
        {
            return key is TKey tkey && ContainsKey(tkey);
        }

        public void Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _internalDict.GetEnumerator();
        }

        public void Remove(object key)
        {
            Remove((TKey)key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void SetTo(IDictionary<TKey, TValue> dict)
        {
            _internalDict = new Dictionary<TKey, TValue>(dict);
        }

        public static implicit operator Dictionary<TKey, TValue>(InitializeOnAccessDictionary<TKey, TValue> initOnAccessDictionary)
        {
            return initOnAccessDictionary._internalDict;
        }
    }
}
