using UnityEngine;

namespace MH3
{
    [CreateAssetMenu(menuName = "MH3/GameRules")]
    public class GameRules : ScriptableObject
    {
        [SerializeField]
        private float earlyInputTime;
        public float EarlyInputTime => earlyInputTime;
        
        [SerializeField]
        private float guardRange;
        public float GuardRange => guardRange;

        [SerializeField]
        private float guardSuccessDamageRate;
        public float GuardSuccessDamageRate => guardSuccessDamageRate;
    }
}
