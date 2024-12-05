using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class Never : Sequence
    {
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            return UniTask.Never(cancellationToken);
        }
    }
}
