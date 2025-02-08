using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
            if (!pools.TryGetValue(key, out var pool))
            {
                var element = elements.Get(key);
                pool = new ObjectPool<EffectObject>(
                    () => Object.Instantiate(element.Prefab),
                    x => x.gameObject.SetActive(true),
                    x => x.gameObject.SetActive(false),
                    x => Object.Destroy(x.gameObject)
                );
                pools.Add(key, pool);
            }

            var instance = pool.Get();
            ReturnAsync(pool, instance).Forget();
            return instance;
        }

        private async UniTask ReturnAsync(ObjectPool<EffectObject> pool, EffectObject instance)
        {
            try
            {
                var scope = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, instance.destroyCancellationToken);
                await instance.WaitUntil(scope.Token);
                instance.transform.SetParent(null);
                pool.Release(instance);
            }
            catch (OperationCanceledException)
            {
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
