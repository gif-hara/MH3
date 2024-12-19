using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class AvailableContents
    {
        [SerializeField]
        private SerializableHashSet<string> contents = new();
        
        [SerializeField]
        private List<string> newContents = new();
        
        public List<string> NewContents => newContents;
        
        public void Add(string content)
        {
            contents.Add(content);
            newContents.Add(content);
        }
        
        public bool Contains(string content)
        {
            return contents.Contains(content);
        }
        
        public void ClearNewContents()
        {
            newContents.Clear();
        }
        
        public void RemoveNewContent(string content)
        {
            newContents.Remove(content);
        }
    }
}
