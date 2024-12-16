using System;
using System.Linq;
using HK;
using UnityEngine;

namespace MH3
{
    public class Parameter
    {
        private readonly Element.DictionaryList basics = new();
        
        private readonly Element.DictionaryList adds = new();
        
        private readonly Element.DictionaryList multiplies = new();
        
        public float Value
        {
            get
            {
                var basic = basics.List.Sum(x => x.Value);
                var add = adds.List.Sum(x => x.Value);
                var multiply = multiplies.List.Sum(x => x.Value);
                return basic + add + basic * multiply;
            }
        }
        
        public int ValueFloorToInt => Mathf.FloorToInt(Value);

        public void RegisterBasics(string key, float value)
        {
            Register(basics, key, value);
        }
        
        public void RegisterAdds(string key, float value)
        {
            Register(adds, key, value);
        }
        
        public void RegisterMultiplies(string key, float value)
        {
            Register(multiplies, key, value);
        }

        public void ClearAll()
        {
            basics.Clear();
            adds.Clear();
            multiplies.Clear();
        }

        private static void Register(Element.DictionaryList list, string key, float value)
        {
            if (list.TryGetValue(key, out var element))
            {
                element.Value = value;
            }
            else
            {
                list.Add(new Element { Key = key, Value = value });
            }
        }
        
        [Serializable]
        public class Element
        {
            public string Key;
            
            public float Value;
            
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
