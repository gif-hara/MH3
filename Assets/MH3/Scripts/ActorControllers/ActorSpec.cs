using UnityEngine;

namespace MH3
{
    [CreateAssetMenu(menuName = "MH3/ActorSpec")]
    public class ActorSpec : ScriptableObject
    {
        [SerializeField]
        private float moveSpeed;
        public float MoveSpeed => moveSpeed;

        [SerializeField]
        private float rotationSpeed;
        public float RotationSpeed => rotationSpeed;

        [SerializeField]
        private int weaponId;
        public int WeaponId => weaponId;
    }
}
