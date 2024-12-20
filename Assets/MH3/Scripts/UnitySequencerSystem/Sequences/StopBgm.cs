using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class StopBgm : Sequence
    {
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            TinyServiceLocator.Resolve<AudioManager>().StopBgm();
            return UniTask.CompletedTask;
        }
    }
}
