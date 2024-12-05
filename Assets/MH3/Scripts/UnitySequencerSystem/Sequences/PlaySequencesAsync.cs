using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class PlaySequencesAsync : Sequence
    {
        [SerializeReference, SubclassSelector]
        private SequencesResolver sequencesResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var sequences = sequencesResolver.Resolve(container);
            var sequencer = new Sequencer(container, sequences);
            return sequencer.PlayAsync(cancellationToken);
        }
    }
}
