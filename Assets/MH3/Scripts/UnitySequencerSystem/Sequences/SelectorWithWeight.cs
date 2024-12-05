using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class SelectorWithWeight : Sequence
    {
        [SerializeField]
        private List<Element> elements;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var element = elements.Lottery(x => x.Weight);
            var sequences = element.SequenceResolver.Resolve(container);
            var sequencer = new Sequencer(container, sequences);
            return sequencer.PlayAsync(cancellationToken);
        }

        [Serializable]
        public class Element
        {
            [SerializeField]
            private int weight;
            public int Weight => weight;

            [SerializeReference, SubclassSelector]
            private SequencesResolver sequenceResolver;
            public SequencesResolver SequenceResolver => sequenceResolver;
        }
    }
}
