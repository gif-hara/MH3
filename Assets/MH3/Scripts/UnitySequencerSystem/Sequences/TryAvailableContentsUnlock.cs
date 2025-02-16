using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class TryAvailableContentsUnlock : Sequence
    {
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            TinyServiceLocator.Resolve<UserData>().TryAvailableContentsUnlock();
            return UniTask.CompletedTask;
        }
    }
}
