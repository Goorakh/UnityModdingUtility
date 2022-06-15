using System;
using System.Collections.Generic;

namespace UnityModdingUtility
{
    public class InitializeOnAccessDictionaryArg<TKey, TValue, TArg> : InitializeOnAccessDictionary<TKey, TValue>
    {
        public delegate bool ValueSelectorTryGetArg(TKey key, TArg arg, out TValue value);

        readonly ValueSelectorTryGetArg _valueSelector;

        InitializeOnAccessDictionaryArg(ValueSelectorTryGet selector) : base(selector)
        {
            throw new NotImplementedException();
        }

        public InitializeOnAccessDictionaryArg(ValueSelectorTryGetArg selector)
        {
            _valueSelector = selector;
        }

        public InitializeOnAccessDictionaryArg(Func<TKey, TArg, TValue> selector) : this((TKey key, TArg arg, out TValue value) => { value = selector(key, arg); return true; })
        {
        }

        public InitializeOnAccessDictionaryArg(Func<TArg, TValue> selector) : this((TKey key, TArg arg, out TValue value) => { value = selector(arg); return true; })
        {
        }

        public override TValue this[TKey key]
        {
            get
            {
                return _internalDict[key];
            }
        }

        public TValue this[TKey key, TArg arg]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                    return value;

                throw new KeyNotFoundException($"Key {key} is not in the dictionary and no value was decided");
            }
            set
            {
                this[key] = value;
            }
        }

        public override bool TryGetValue(TKey key, out TValue value)
        {
            return _internalDict.TryGetValue(key, out value);
        }

        public bool TryGetValue(TKey key, TArg arg, out TValue value)
        {
            if (TryGetValue(key, out value))
                return true;

            if (_valueSelector(key, arg, out value))
            {
                _internalDict.Add(key, value);
                return true;
            }

            return false;
        }

        public static implicit operator Dictionary<TKey, TValue>(InitializeOnAccessDictionaryArg<TKey, TValue, TArg> initOnAccessDictionary)
        {
            return initOnAccessDictionary._internalDict;
        }
    }
}
