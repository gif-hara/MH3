using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class TryEnemyBeginQuestEmotion : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver isSuccessKeyResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            var emotionKeys = TinyServiceLocator.Resolve<GameRules>().EnemyBeginQuestEmotions
                .FirstOrDefault(x => actor.SpecController.WeaponSpec.WeaponType == x.WeaponType)
                .EmotionKeys;
            var emotionKey = emotionKeys[UnityEngine.Random.Range(0, emotionKeys.Count)];
            var isSuccess = actor.ActionController.TryEmotion(emotionKey);
            if(isSuccessKeyResolver != null)
            {
                container.RegisterOrReplace(isSuccessKeyResolver.Resolve(container), isSuccess);
            }
            return UniTask.CompletedTask;
        }
    }
}
