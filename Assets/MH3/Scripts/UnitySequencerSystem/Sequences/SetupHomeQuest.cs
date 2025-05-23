using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class SetupHomeQuest : Sequence
    {
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            container.Resolve<GameSceneController>().SetupHomeQuestAsync();
            return UniTask.CompletedTask;
        }
    }
}
