using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class GameCameraBeginImpulseSource : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver impulseNameResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var gameCameraController = TinyServiceLocator.Resolve<GameCameraController>();
            gameCameraController.BeginImpulseSource(impulseNameResolver.Resolve(container));
            return UniTask.CompletedTask;
        }
    }
}
