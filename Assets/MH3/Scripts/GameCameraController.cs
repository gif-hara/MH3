using System;
using HK;
using Unity.Cinemachine;
using UnityEngine;

namespace MH3
{
    public class GameCameraController : MonoBehaviour
    {
        [SerializeField]
        private CinemachineCamera defaultCinemachineCamera;

        [SerializeField]
        private CinemachineCameraElement.DictionaryList cinemachineCameras;
        public CinemachineCameraElement.DictionaryList CinemachineCameras => cinemachineCameras;

        [SerializeField]
        private ImpulseSourceElement.DictionaryList impulseSources;
        public ImpulseSourceElement.DictionaryList ImpulseSources => impulseSources;

        [SerializeField]
        private Camera controlledCamera;
        public Camera ControlledCamera => controlledCamera;

        public Transform DefaultCinemachineCameraTrackingTarget => defaultCinemachineCamera.Target.TrackingTarget;

        public Transform DefaultCinemachineCameraLookAtTarget => defaultCinemachineCamera.Target.LookAtTarget;

        public void SetTrackingTarget(Transform tracking, Transform lookAt)
        {
            defaultCinemachineCamera.Target.TrackingTarget = tracking;
            defaultCinemachineCamera.Target.LookAtTarget = lookAt;
        }

        public void BeginImpulseSource(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            impulseSources.Get(name).ImpulseSource.GenerateImpulse();
        }

        [Serializable]
        public class CinemachineCameraElement
        {
            [SerializeField]
            private CinemachineCamera cinemachineCamera;
            public CinemachineCamera CinemachineCamera => cinemachineCamera;

            [Serializable]
            public class DictionaryList : DictionaryList<string, CinemachineCameraElement>
            {
                public DictionaryList() : base(x => x.cinemachineCamera.name)
                {
                }
            }
        }

        [Serializable]
        public class ImpulseSourceElement
        {
            [SerializeField]
            private CinemachineImpulseSource impulseSource;
            public CinemachineImpulseSource ImpulseSource => impulseSource;

            [Serializable]
            public class DictionaryList : DictionaryList<string, ImpulseSourceElement>
            {
                public DictionaryList() : base(x => x.impulseSource.name)
                {
                }
            }
        }
    }
}
