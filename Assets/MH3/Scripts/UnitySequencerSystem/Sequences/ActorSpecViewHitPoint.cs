using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MH3.UnitySequencerSystem.Resolvers;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class ActorSpecViewHitPoint : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            text.text = actor.SpecController.HitPointMaxTotal.ToString();
            return UniTask.CompletedTask;
        }
    }
}
