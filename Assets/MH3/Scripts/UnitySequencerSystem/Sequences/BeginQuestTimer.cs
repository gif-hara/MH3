using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class BeginQuestTimer : Sequence
    {
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            container.Resolve<GameSceneController>().BeginQuestTimer();
            return UniTask.CompletedTask;
        }
    }
}
