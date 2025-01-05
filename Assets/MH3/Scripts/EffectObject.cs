using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MH3
{
    public class EffectObject : MonoBehaviour
    {
        [SerializeField]
        private float lifeTime;

        public async UniTask WaitUntilDeadAsync(CancellationToken scope)
        {
            var newScope = CancellationTokenSource.CreateLinkedTokenSource(scope);
            await UniTask.Delay(TimeSpan.FromSeconds(lifeTime), cancellationToken: newScope.Token);
            newScope.Cancel();
            newScope.Dispose();
        }
    }
}
