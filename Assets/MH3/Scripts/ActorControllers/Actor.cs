using System;
using StandardAssets.Characters.Physics;
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

        [SerializeField]
        private SimpleAnimation simpleAnimation;

        [SerializeField]
        private LocatorHolder locatorHolder;

        [SerializeField]
        private OpenCharacterController openCharacterController;
        
        public ActorMovementController MovementController { get; private set; }
        
        public ActorStateMachine StateMachine { get; private set; }
        
        public ActorStateProvider StateProvider { get; private set; }

        public ActorSpecController SpecController { get; private set; }

        public ActorAnimationController AnimationController { get; private set; }

        public ActorWeaponController WeaponController { get; private set; }

        public ActorAttackController AttackController { get; private set; }

        public LocatorHolder LocatorHolder => locatorHolder;
        
        void Awake()
        {
            SpecController = new ActorSpecController(spec);
            MovementController = new ActorMovementController();
            StateMachine = new ActorStateMachine(this, initialState);
            StateProvider = new ActorStateProvider(this);
            AnimationController = new ActorAnimationController(this, simpleAnimation);
            WeaponController = new ActorWeaponController(this);
            AttackController = new ActorAttackController(this);
            MovementController.Setup(this, openCharacterController);
        }

        private void OnDestroy()
        {
            StateMachine.Dispose();
        }
    }
}
