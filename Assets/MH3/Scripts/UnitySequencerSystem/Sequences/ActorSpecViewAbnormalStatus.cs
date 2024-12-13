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
    public class ActorSpecViewAbnormalStatus : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private TMP_Text text;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            if (actor.SpecController.AbnormalStatusAttackType == Define.AbnormalStatusType.None)
            {
                text.text = "なし";
            }
            else
            {
                text.text = $"[{actor.SpecController.AbnormalStatusAttackType.GetName()}] {actor.SpecController.AbnormalStatusAttackTotal}";
            }
            return UniTask.CompletedTask;
        }
    }
}
