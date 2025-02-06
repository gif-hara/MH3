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

        public override async UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var userData = TinyServiceLocator.Resolve<UserData>();
            var availableContentsEvents = TinyServiceLocator.Resolve<MasterData>().AvailableContentsEvents.Get(trigger);
            foreach (var availableContentsEvent in availableContentsEvents)
            {
                if (userData.AvailableContents.NewContents.Contains(availableContentsEvent.NewContentsKey))
                {
                    var sequences = availableContentsEvent.Sequences.Sequences;
                    var sequencer = new Sequencer(container, sequences);
                    await sequencer.PlayAsync(cancellationToken);
                    userData.AvailableContents.RemoveNewContent(availableContentsEvent.NewContentsKey);
                    SaveSystem.Save(TinyServiceLocator.Resolve<SaveData>(), SaveData.Path);
                }
            }
        }
    }
}
