using Unity.Cinemachine;
using UnityEngine;

namespace MH3
{
    public class GameCameraController : MonoBehaviour
    {
        [SerializeField]
        private CinemachineCamera defaultCinemachineCamera;
        
        public void SetTrackingTarget(Transform target)
        {
            defaultCinemachineCamera.Target.TrackingTarget = target;
        }
    }
}
