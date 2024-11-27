using System;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class Actor : MonoBehaviour
    {
        public ActorMovementController MovementController { get; private set; }
        
        public ActorStateMachine StateMachine { get; private set; }
        
        public ActorStateProvider StateProvider { get; private set; }

        void Awake()
        {
            MovementController = new ActorMovementController();
            StateMachine = new ActorStateMachine(this);
            StateProvider = new ActorStateProvider(this);
            MovementController.Setup(this);
        }

        private void OnDestroy()
        {
            StateMachine.Dispose();
        }
    }
}
