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
    public class ActorSpecViewElement : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            if (actor.SpecController.ElementAttackType == Define.ElementType.None)
            {
                text.text = "なし".Localized();
            }
            else
            {
                text.text = $"[{actor.SpecController.ElementAttackType.GetName()}] {actor.SpecController.ElementAttackTotal}";
            }
            return UniTask.CompletedTask;
        }
    }
}
