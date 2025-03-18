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
            var pool = GetPool(key);
            var instance = pool.Get();
            ReturnAsync(pool, instance).Forget();
            return instance;
        }

        public (EffectObject effectObject, ObjectPool<EffectObject> pool) RentManual(string key)
        {
            var pool = GetPool(key);
            var instance = pool.Get();
            return (instance, pool);
        }

        public void Return(EffectObject instance, ObjectPool<EffectObject> pool)
        {
            instance.transform.SetParent(transform);
            pool.Release(instance);
        }

        private ObjectPool<EffectObject> GetPool(string key)
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

            return pool;
        }

        private async UniTask ReturnAsync(ObjectPool<EffectObject> pool, EffectObject instance, CancellationToken cancellationToken = default)
        {
            try
            {
                var scope = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, instance.destroyCancellationToken, cancellationToken);
                if (instance.CanWait())
                {
                    await instance.WaitUntil(scope.Token);
                    Return(instance, pool);
                }
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
