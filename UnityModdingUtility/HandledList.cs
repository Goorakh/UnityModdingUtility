﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityModdingUtility
{
    public class HandledList<T> : IEnumerable<T>
    {
        readonly int _increment;
        Slot[] _slots;

        public HandledList(int initialSize, int increment)
        {
            _slots = new Slot[initialSize];
            _increment = increment;
        }

        void expand()
        {
            Array.Resize(ref _slots, _slots.Length + _increment);
        }

        public int Store(T value)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                ref Slot slot = ref _slots[i];
                if (!slot.Occupied)
                {
                    slot.Set(value);
                    return i;
                }
            }

            int index = _slots.Length;
            expand();
            _slots[index].Set(value);
            return index;
        }

        public void Clear(int slotID)
        {
            if (slotID < 0 || slotID >= _slots.Length)
                throw new ArgumentOutOfRangeException(nameof(slotID));

            _slots[slotID].Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (Slot slot in _slots)
            {
                if (slot.Occupied)
                {
                    yield return slot.Value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        struct Slot
        {
            public T Value;
            public bool Occupied;

            public void Set(T value)
            {
                Value = value;
                Occupied = true;
            }

            public void Clear()
            {
                Value = default;
                Occupied = false;
            }
        }
    }
}
