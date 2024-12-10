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
    public class GameCameraSetActiveCinemachineCamera : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver cinemachineCameraNameResolver;

        [SerializeReference, SubclassSelector]
        private BooleanResolver isActiveResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var gameCameraController = TinyServiceLocator.Resolve<GameCameraController>();
            var cinemachineCameraName = cinemachineCameraNameResolver.Resolve(container);
            var isActive = isActiveResolver.Resolve(container);
            gameCameraController.CinemachineCameras.Get(cinemachineCameraName).CinemachineCamera.gameObject.SetActive(isActive);
            return UniTask.CompletedTask;
        }
    }
}
