using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class SetColorGraphic : Sequence
    {
        [SerializeReference, SubclassSelector]
        private GraphicResolver targetResolver;
        
        [SerializeReference, SubclassSelector]
        private ColorResolver colorResolver;
        
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var target = targetResolver.Resolve(container);
            target.color = colorResolver.Resolve(container);
            return UniTask.CompletedTask;
        }
    }
}
