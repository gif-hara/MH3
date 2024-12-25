using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH3
{
    [CreateAssetMenu(menuName = "MH3/ArmorModelData")]
    public class ArmorModelData : ScriptableObject
    {
        [SerializeField]
        private List<Element> elements;
        public List<Element> Elements => elements;

        [Serializable]
        public class Element
        {
            [SerializeField]
            private string locatorName;
            public string LocatorName => locatorName;

            [SerializeField]
            private Armor modelPrefab;
            public Armor ModelPrefab => modelPrefab;
        }
    }
}
