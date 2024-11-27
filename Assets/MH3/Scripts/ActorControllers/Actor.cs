using System;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class Actor : MonoBehaviour
    {
        [SerializeField]
        private ActorSpec spec;

        [SerializeField]
        private ScriptableSequences initialState;
        
        public ActorMovementController MovementController { get; private set; }
        
        public ActorStateMachine StateMachine { get; private set; }
        
        public ActorStateProvider StateProvider { get; private set; }

        public ActorSpecController SpecController { get; private set; }

        public ActorAnimationController AnimationController { get; private set; }
        
        void Awake()
        {
            SpecController = new ActorSpecController(spec);
            MovementController = new ActorMovementController();
            StateMachine = new ActorStateMachine(this, initialState);
            StateProvider = new ActorStateProvider(this);
            AnimationController = new ActorAnimationController();
            MovementController.Setup(this);
        }

        private void OnDestroy()
        {
            StateMachine.Dispose();
        }
    }
}
