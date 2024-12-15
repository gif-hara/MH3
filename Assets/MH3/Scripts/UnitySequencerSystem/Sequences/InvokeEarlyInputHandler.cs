using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class InvokeEarlyInputHandler : Sequence
    {
        [SerializeReference, SubclassSelector]
        private SequencesResolver sequencesResolver;

        [SerializeReference, SubclassSelector]
        private FloatResolver inputTimeResolver;

        [SerializeReference, SubclassSelector]
        private BooleanResolver isSuccessResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var inputTime = inputTimeResolver == null ? TinyServiceLocator.Resolve<GameRules>().EarlyInputTime : inputTimeResolver.Resolve(container);
            var scope = new CancellationDisposable();
            EarlyInputHandler.Invoke(() =>
            {
                var sequences = sequencesResolver.Resolve(container);
                var sequencer = new Sequencer(container, sequences);
                sequencer.PlayAsync(scope.Token).Forget();
                var isSuccess = isSuccessResolver.Resolve(container);
                if (isSuccess)
                {
                    scope.Dispose();
                }
                return isSuccess;
            }, inputTime, scope.Token);
            return UniTask.CompletedTask;
        }
    }
}
