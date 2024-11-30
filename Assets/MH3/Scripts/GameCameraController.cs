using Unity.Cinemachine;
using UnityEngine;

namespace MH3
{
    public class GameCameraController : MonoBehaviour
    {
        [SerializeField]
        private CinemachineCamera defaultCinemachineCamera;

        [SerializeField]
        private Camera controlledCamera;
        public Camera ControlledCamera => controlledCamera;
        
        public void SetTrackingTarget(Transform target)
        {
            defaultCinemachineCamera.Target.TrackingTarget = target;
        }
    }
}
