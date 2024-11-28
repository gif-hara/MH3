using R3;

namespace MH3
{
    public class ActorSpecController
    {
        private readonly ActorSpec spec;

        private readonly ReactiveProperty<int> weaponId = new(0);

        public ActorSpecController(ActorSpec spec)
        {
            this.spec = spec;
            weaponId.Value = spec.WeaponId;
        }

        public float MoveSpeed => spec.MoveSpeed;

        public float RotationSpeed => spec.RotationSpeed;

        public ReadOnlyReactiveProperty<int> WeaponId => weaponId;
    }
}
