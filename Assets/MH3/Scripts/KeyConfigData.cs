using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class KeyConfigData
    {
        [SerializeField]
        private List<Element> elements = new();
        
        public void AddOrReplace(string name, string json)
        {
            var index = elements.FindIndex(e => e.Name == name);
            if (index == -1)
            {
                elements.Add(new Element(name, json));
            }
            else
            {
                elements[index] = new Element(name, json);
            }
        }
        
        [Serializable]
        public class Element
        {
            [SerializeField]
            private string name;
            public string Name => name;

            [SerializeField]
            private string json;
            public string Json => json;
            
            public Element(string name, string json)
            {
                this.name = name;
                this.json = json;
            }
        }
    }
}
