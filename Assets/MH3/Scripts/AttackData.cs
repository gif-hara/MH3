using UnityEngine;

namespace MH3
{
    [CreateAssetMenu(fileName = "AttackData", menuName = "MH3/AttackData")]
    public class AttackData : ScriptableObject
    {
        [SerializeField]
        private string colliderName;
        public string ColliderName => colliderName;

        [SerializeField]
        private int power;
        public int Power => power;
    }
}
