using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using HK;
using R3;
using UnityEngine;

namespace MH3
{
    public class EffectObject : MonoBehaviour
    {
        [SerializeField]
        private float lifeTime;

        public UniTask WaitUntil(CancellationToken scope)
        {
            return UniTask.WhenAny(
                UniTask.Delay(TimeSpan.FromSeconds(lifeTime), cancellationToken: scope),
                TinyServiceLocator.Resolve<GameEvents>().OnTransitioned.FirstAsync().AsUniTask()
            );
        }
    }
}
