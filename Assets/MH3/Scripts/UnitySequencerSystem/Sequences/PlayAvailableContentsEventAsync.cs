using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class PlayAvailableContentsEventAsync : Sequence
    {
        [SerializeField]
        private Define.AvailableContentsEventTrigger trigger;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            return TinyServiceLocator.Resolve<MasterData>().AvailableContentsEvents.Get(trigger).PlayAsync(cancellationToken);
        }
    }
}
