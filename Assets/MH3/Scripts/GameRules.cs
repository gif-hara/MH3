using UnityEngine;
using UnitySequencerSystem;

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

        [SerializeField]
        private float poisonInterval;
        public float PoisonInterval => poisonInterval;

        [SerializeField]
        private int poisonDamage;
        public int PoisonDamage => poisonDamage;

        [SerializeField]
        private ScriptableSequences paralysisBeginSequence;
        public ScriptableSequences ParalysisBeginSequence => paralysisBeginSequence;

        [SerializeField]
        private ScriptableSequences paralysisEndSequence;
        public ScriptableSequences ParalysisEndSequence => paralysisEndSequence;
    }
}
