using StandardAssets.Characters.Physics;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class Actor : MonoBehaviour
    {
        [SerializeField]
        private SimpleAnimation simpleAnimation;

        [SerializeField]
        private LocatorHolder locatorHolder;

        [SerializeField]
        private OpenCharacterController openCharacterController;

        [SerializeField]
        private FacialController facialController;

        public ActorMovementController MovementController { get; private set; }

        public ActorStateMachine StateMachine { get; private set; }

        public ActorStateProvider StateProvider { get; private set; }

        public ActorSpecController SpecController { get; private set; }

        public ActorAnimationController AnimationController { get; private set; }

        public ActorWeaponController WeaponController { get; private set; }

        public ActorArmorController ArmorController { get; private set; }

        public ActorAttackController AttackController { get; private set; }

        public ActorColliderController ColliderController { get; private set; }

        public ActorTimeController TimeController { get; private set; }

        public ActorBehaviourController BehaviourController { get; private set; }

        public ActorActionController ActionController { get; private set; }

        public LocatorHolder LocatorHolder => locatorHolder;

        public FacialController FacialController => facialController;

        public Actor Spawn(Vector3 position, Quaternion rotation, MasterData.ActorSpec actorSpec)
        {
            var actor = Instantiate(this, position, rotation);
            actor.SpecController = new ActorSpecController(actor, actorSpec);
            actor.TimeController = new ActorTimeController(actor);
            actor.MovementController = new ActorMovementController();
            actor.StateMachine = new ActorStateMachine(actor, actorSpec.InitialStateSequences);
            actor.StateProvider = new ActorStateProvider(actor);
            actor.AnimationController = new ActorAnimationController(actor, actor.simpleAnimation);
            actor.AttackController = new ActorAttackController(actor);
            actor.WeaponController = new ActorWeaponController(actor);
            actor.ArmorController = new ActorArmorController(actor);
            actor.ColliderController = new ActorColliderController(actor);
            actor.BehaviourController = new ActorBehaviourController(actor);
            actor.ActionController = new ActorActionController(actor);
            actor.MovementController.Setup(actor, actor.openCharacterController);
            actor.SpecController.BeginObserve();
            return actor;
        }

        private void OnDestroy()
        {
            StateMachine.Dispose();
        }
    }
}
