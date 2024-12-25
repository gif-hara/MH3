using System;
using HK;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class Stats
    {
        [SerializeField]
        private Element.DictionaryList elements = new();

        public void AddOrUpdate(string key, float value)
        {
            if (elements.TryGetValue(key, out var element))
            {
                element.Value = value;
            }
            else
            {
                elements.Add(new Element { Key = key, Value = value });
            }
        }

        public float GetOrDefault(string key)
        {
            return elements.TryGetValue(key, out var element) ? element.Value : default;
        }

        public bool Contains(string key)
        {
            return elements.ContainsKey(key);
        }

        public static class Key
        {
            public static string GetQuestClearTime(string questSpecId) => $"QuestClearTime.{questSpecId}";

            public static string GetDefeatEnemyCount(int enemyId) => $"DefeatEnemyCount.{enemyId}";
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
