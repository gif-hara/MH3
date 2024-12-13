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

        [SerializeField]
        private float justGuardTime;
        public float JustGuardTime => justGuardTime;

        [SerializeField]
        private string successJustGuardSfxKey;
        public string SuccessJustGuardSfxKey => successJustGuardSfxKey;

        [SerializeField]
        private string successGuardSfxKey;
        public string SuccessGuardSfxKey => successGuardSfxKey;

        [SerializeField]
        private int recoveryAmount;
        public int RecoveryAmount => recoveryAmount;

        [SerializeField]
        private float criticalDamageRate;
        public float CriticalDamageRate => criticalDamageRate;

        [SerializeField]
        private int defenseRate;
        public int DefenseRate => defenseRate;
    }
}
