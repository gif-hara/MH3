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
    public class GameCameraSyncTracking : Sequence
    {
        [SerializeReference, SubclassSelector]
        private StringResolver cinemachineCameraNameResolver;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var gameCameraController = TinyServiceLocator.Resolve<GameCameraController>();
            var cinemachineCameraName = cinemachineCameraNameResolver.Resolve(container);
            var cinemachineCamera = gameCameraController.CinemachineCameras.Get(cinemachineCameraName).CinemachineCamera;
            cinemachineCamera.Target.TrackingTarget = gameCameraController.DefaultCinemachineCameraTrackingTarget;
            cinemachineCamera.Target.LookAtTarget = gameCameraController.DefaultCinemachineCameraLookAtTarget;
            return UniTask.CompletedTask;
        }
    }
}
