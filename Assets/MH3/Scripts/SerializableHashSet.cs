using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class SerializableHashSet<T>
    {
        [SerializeField]
        private List<T> list = new();
        
        private readonly HashSet<T> hashSet = new();
        
        public void Add(T item)
        {
            InitializeIfNeed();
            if (hashSet.Add(item))
            {
                list.Add(item);
            }
        }
        
        public bool Contains(T item)
        {
            InitializeIfNeed();
            return hashSet.Contains(item);
        }
        
        public void Clear()
        {
            hashSet.Clear();
            list.Clear();
        }

        public void Remove(T item)
        {
            InitializeIfNeed();
            if (hashSet.Remove(item))
            {
                list.Remove(item);
            }
        }

        private void InitializeIfNeed()
        {
            if (hashSet.Count == 0)
            {
                foreach (var item in list)
                {
                    hashSet.Add(item);
                }
            }
        }
    }
}
