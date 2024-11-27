using System;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class Actor : MonoBehaviour
    {
        [SerializeField]
        private ScriptableSequences initialState;
        
        public ActorMovementController MovementController { get; private set; }
        
        public ActorStateMachine StateMachine { get; private set; }
        
        public ActorStateProvider StateProvider { get; private set; }
        
        public ActorActionController ActionController { get; private set; }

        void Awake()
        {
            MovementController = new ActorMovementController();
            StateMachine = new ActorStateMachine(this, initialState);
            ActionController = new ActorActionController(this);
            StateProvider = new ActorStateProvider(this);
            MovementController.Setup(this);
        }

        private void OnDestroy()
        {
            StateMachine.Dispose();
        }
    }
}
