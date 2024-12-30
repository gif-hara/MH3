using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class PopTimeScaleRoot : Sequence
    {
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            HK.Time.Root.PopTimeScale();
            return UniTask.CompletedTask;
        }
    }
}
