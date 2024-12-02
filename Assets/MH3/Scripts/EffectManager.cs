using System;
using System.Collections.Generic;
using HK;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace MH3
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField]
        private Element.DictionaryList elements;
        
        private readonly Dictionary<string, ObjectPool<EffectObject>> pools = new();

        public EffectObject Rent(string key)
        {
            if(pools.TryGetValue(key, out var pool))
            {
                return pool.Get();
            }
            else
            {
                var element = elements.Get(key);
                pool = new ObjectPool<EffectObject>(
                    () => Object.Instantiate(element.Prefab),
                    x => x.gameObject.SetActive(true),
                    x => x.gameObject.SetActive(false),
                    x => Object.Destroy(x.gameObject)
                    );
                pools.Add(key, pool);
                return pool.Get();
            }
        }

        [Serializable]
        public class Element
        {
            [SerializeField]
            private string key;
            public string Key => key;
            
            [SerializeField]
            private EffectObject prefab;
            public EffectObject Prefab => prefab;

            [Serializable]
            public class DictionaryList : DictionaryList<string, Element>
            {
                public DictionaryList() : base(x => x.Key)
                {
                }
            }
        }
    }
}
