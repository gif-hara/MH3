using System;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3.ContainerEvaluators
{
    [Serializable]
    public class Constant : IContainerEvaluator
    {
        [SerializeReference, SubclassSelector]
        private BooleanResolver resolver;

        public bool Evaluate(Container container)
        {
            return resolver.Resolve(container);
        }
    }
}
