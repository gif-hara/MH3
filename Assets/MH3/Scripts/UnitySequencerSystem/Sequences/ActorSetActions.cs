using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.ActorControllers;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class ActorSetActions : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private List<Element> elements;
        
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            actor.ActionController.ClearAction();
            foreach (var element in elements)
            {
                actor.ActionController.SetAction(element.type, element.sequence);
            }
            return UniTask.CompletedTask;
        }

        [Serializable]
        public class Element
        {
            public ActorActionController.ActionType type;
            
            public ScriptableSequences sequence;
        }
    }
}
