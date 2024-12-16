using System;
using System.Linq;
using HK;
using R3;
using UnityEngine;

namespace MH3
{
    public sealed class Parameter
    {
        private readonly Element.DictionaryList basics = new();

        private readonly Element.DictionaryList adds = new();

        private readonly Element.DictionaryList multiplies = new();

        private readonly Subject<float> onValueChanged = new();
        public Observable<float> OnValueChanged => onValueChanged;

        public float Value
        {
            get
            {
                var basic = basics.List.Sum(x => x.ValueSelector());
                var add = adds.List.Sum(x => x.ValueSelector());
                var multiply = multiplies.List.Sum(x => x.ValueSelector());
                return basic + add + basic * multiply;
            }
        }

        public int ValueFloorToInt => Mathf.FloorToInt(Value);

        public void RegisterBasics(string key, Func<float> valueSelector)
        {
            Register(basics, key, valueSelector);
        }

        public void RegisterAdds(string key, Func<float> valueSelector)
        {
            Register(adds, key, valueSelector);
        }

        public void RegisterMultiplies(string key, Func<float> valueSelector)
        {
            Register(multiplies, key, valueSelector);
        }

        public void ClearAll()
        {
            basics.Clear();
            adds.Clear();
            multiplies.Clear();
            onValueChanged.OnNext(Value);
        }

        private void Register(Element.DictionaryList list, string key, Func<float> valueSelector)
        {
            if (list.TryGetValue(key, out var element))
            {
                element.ValueSelector = valueSelector;
            }
            else
            {
                list.Add(new Element { Key = key, ValueSelector = valueSelector });
            }

            onValueChanged.OnNext(Value);
        }

        [Serializable]
        public class Element
        {
            public string Key;

            public Func<float> ValueSelector;

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
