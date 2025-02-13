using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.UnitySequencerSystem.Resolvers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class GameCameraBeginQuestStartAsync : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var gameCameraController = TinyServiceLocator.Resolve<GameCameraController>();
            return gameCameraController.BeginQuestStartAsync(actorResolver.Resolve(container), cancellationToken);
        }
    }
}
