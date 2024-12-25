using System;
using System.Collections.Generic;
using HK;
using MH3.ProjectileControllers;
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

        [SerializeField]
        private float collapseDamageRate;
        public float CollapseDamageRate => collapseDamageRate;

        [SerializeField]
        private ElementProjectile.DictionaryList elementProjectiles;
        public ElementProjectile.DictionaryList ElementProjectiles => elementProjectiles;

        [SerializeField]
        private string defaultDodgeAnimationName;
        public string DefaultDodgeAnimationName => defaultDodgeAnimationName;

        [SerializeField]
        private string dualSwordDodgeAnimationName;
        public string DualSwordDodgeAnimationName => dualSwordDodgeAnimationName;

        [SerializeField]
        private float dualSwordDodgeTime;
        public float DualSwordDodgeTime => dualSwordDodgeTime;

        [SerializeField]
        private float superArmorDamageRate;
        public float SuperArmorDamageRate => superArmorDamageRate;

        [SerializeField]
        private string superArmorHitSfxKey;
        public string SuperArmorHitSfxKey => superArmorHitSfxKey;

        [SerializeField]
        private float bladeSuperArmorTime;
        public float BladeSuperArmorTime => bladeSuperArmorTime;

        [SerializeField]
        private int rewardOptionNumber;
        public int RewardOptionNumber => rewardOptionNumber;

        [SerializeField]
        private List<int> initialWeaponIds;
        public List<int> InitialWeaponIds => initialWeaponIds;

        [SerializeField]
        private string criticalHitSfxKey;
        public string CriticalHitSfxKey => criticalHitSfxKey;

        [SerializeField]
        private string criticalHitEffectKey;
        public string CriticalHitEffectKey => criticalHitEffectKey;

        [Serializable]
        public class ElementProjectile
        {
            [SerializeField]
            private Define.ElementType elementType;
            public Define.ElementType ElementType => elementType;

            [SerializeField]
            private Projectile projectilePrefab;
            public Projectile ProjectilePrefab => projectilePrefab;

            [SerializeField]
            private string attackSpecKey;
            public string AttackSpecKey => attackSpecKey;

            [Serializable]
            public class DictionaryList : DictionaryList<Define.ElementType, ElementProjectile>
            {
                public DictionaryList() : base(x => x.ElementType)
                {
                }
            }
        }
    }
}
