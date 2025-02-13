using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class GameCameraForceDefault : Sequence
    {
        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var gameCameraController = TinyServiceLocator.Resolve<GameCameraController>();
            gameCameraController.ForceDefaultCinemachineCamera();
            return UniTask.CompletedTask;
        }
    }
}
