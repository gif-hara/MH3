using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class TakeUntilDisposeScopeObservable : Sequence
    {
        [SerializeField]
        private ScriptableSequences subscribeSequence;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            cancellationToken.RegisterWithoutCaptureExecutionContext(async () =>
            {
                using (var disposable = new CancellationDisposable())
                {
                    var sequencer = new Sequencer(container, subscribeSequence.Sequences);
                    await sequencer.PlayAsync(disposable.Token);
                }
            });
            return UniTask.CompletedTask;
        }
    }
}
