using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class ContainerRegisterEvaluateActorDistance : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;
        
        [SerializeReference, SubclassSelector]
        private ActorResolver targetResolver;
        
        [SerializeReference, SubclassSelector]
        private StringResolver keyResolver;
        
        [SerializeField]
        private Define.ComparisonType comparisonType;
        
        [SerializeReference, SubclassSelector]
        private FloatResolver checkDistanceResolver;
        
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var target = targetResolver.Resolve(container);
            Func<bool> selector = () => comparisonType.Evaluate(Vector3.Distance(actor.transform.position, target.transform.position), checkDistanceResolver.Resolve(container));
            container.RegisterOrReplace(keyResolver.Resolve(container), selector);
            return UniTask.CompletedTask;
        }
    }
}
