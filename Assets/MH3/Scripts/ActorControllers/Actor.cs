using System;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class Actor : MonoBehaviour
    {
        public ActorMovementController MovementController { get; private set; }
        
        public ActorStateController StateController { get; private set; }

        void Awake()
        {
            MovementController = new ActorMovementController();
            StateController = new ActorStateController(this);
            MovementController.Setup(this);
        }

        private void OnDestroy()
        {
            StateController.Dispose();
        }
    }
}
