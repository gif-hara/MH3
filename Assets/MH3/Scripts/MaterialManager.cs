using System;
using HK;
using UnityEngine;

namespace MH3
{
    public class MaterialManager : MonoBehaviour
    {
        [SerializeField]
        private Element.DictionaryList elements;

        public Material Get(string key)
        {
            return elements.Get(key).Material;
        }

        [Serializable]
        public class Element
        {
            [SerializeField]
            private Material material;
            public Material Material => material;

            [Serializable]
            public class DictionaryList : DictionaryList<string, Element>
            {
                public DictionaryList() : base(x => x.Material.name)
                {
                }
            }
        }
    }
}
