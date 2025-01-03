using System;
using System.Collections.Generic;
using HK;
using MH3.ActorControllers;
using Unity.Cinemachine;
using UnityEngine;

namespace MH3
{
    public class GameCameraController : MonoBehaviour
    {
        [SerializeField]
        private CinemachineCamera defaultCinemachineCamera;

        [SerializeField]
        private CinemachineCamera titleCinemachineCamera;

        [SerializeField]
        private List<CinemachineCamera> defeatEnemyCinemachineCameras;

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

        public void Setup(Actor player, Actor enemy)
        {
            defaultCinemachineCamera.Target.TrackingTarget = player.transform;
            defaultCinemachineCamera.Target.LookAtTarget = enemy.LocatorHolder.Get("Root");
            titleCinemachineCamera.Target.TrackingTarget = player.transform;
            titleCinemachineCamera.Target.LookAtTarget = player.LocatorHolder.Get("Root");
            titleCinemachineCamera.gameObject.SetActive(false);
            foreach (var defeatEnemyCinemachineCamera in defeatEnemyCinemachineCameras)
            {
                defeatEnemyCinemachineCamera.Target.TrackingTarget = enemy.transform;
                defeatEnemyCinemachineCamera.Target.LookAtTarget = enemy.LocatorHolder.Get("Root");
            }
        }

        public void BeginTitle()
        {
            titleCinemachineCamera.gameObject.SetActive(true);
        }

        public void EndTitle()
        {
            titleCinemachineCamera.gameObject.SetActive(false);
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
