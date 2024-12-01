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

        public ActorMovementController MovementController { get; private set; }

        public ActorStateMachine StateMachine { get; private set; }

        public ActorStateProvider StateProvider { get; private set; }

        public ActorSpecController SpecController { get; private set; }

        public ActorAnimationController AnimationController { get; private set; }

        public ActorWeaponController WeaponController { get; private set; }

        public ActorAttackController AttackController { get; private set; }

        public ActorColliderController ColliderController { get; private set; }

        public ActorTimeController TimeController { get; private set; }

        public ActorDodgeController DodgeController { get; private set; }

        public LocatorHolder LocatorHolder => locatorHolder;

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
            actor.ColliderController = new ActorColliderController(actor);
            actor.DodgeController = new ActorDodgeController(actor);
            actor.MovementController.Setup(actor, actor.openCharacterController);
            return actor;
        }

        private void OnDestroy()
        {
            StateMachine.Dispose();
        }
    }
}
