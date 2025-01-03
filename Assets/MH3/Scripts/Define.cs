namespace MH3
{
    public static class Define
    {
        public enum ActorType
        {
            Player,
            Enemy,
        }

        public enum FlinchType
        {
            None,
            VerySmall,
            Small,
            Large,
        }

        public enum ComparisonType
        {
            Equal,
            NotEqual,
            Greater,
            Less,
            GreaterOrEqual,
            LessOrEqual,
        }

        public enum RareType
        {
            Common,
            Rare,
            Legend,
        }

        public enum RewardType
        {
            InstanceWeapon,
            InstanceSkillCore,
            InstanceArmor,
        }

        public enum SkillType
        {
            /// <summary>攻撃力アップ</summary>
            AttackUp,
            /// <summary>会心率アップ</summary>
            CriticalUp,
            /// <summary>防御力アップ</summary>
            DefenseUp,
            /// <summary>毒属性アップ</summary>
            PoisonUp,
            /// <summary>麻痺属性アップ</summary>
            ParalysisUp,
            /// <summary>崩壊属性アップ</summary>
            CollapseUp,
            /// <summary>火属性アップ</summary>
            FireElementAttackUp,
            /// <summary>水属性アップ</summary>
            WaterElementAttackUp,
            /// <summary>草属性アップ</summary>
            GrassElementAttackUp,
            /// <summary>体力アップ</summary>
            HealthUp,
            /// <summary>回復コマンド回数アップ</summary>
            RecoveryCommandCountUp,
            /// <summary>報酬アップ</summary>
            RewardUp,
            /// <summary>怯み値アップ</summary>
            FlinchDamageUp,
            /// <summary>回復量アップ</summary>
            RecoveryAmountUp,
            /// <summary>ジャストガード成功時会心率アップ</summary>
            SuccessJustGuardCriticalUp,
            /// <summary>最後のコンボ攻撃力アップ</summary>
            LastComboAttackUp,
        }

        public enum ActorParameterType
        {
            Attack,
            Critical,
            Defense,
            PoisonAttack,
            ParalysisAttack,
            CollapseAttack,
            FireElementAttack,
            WaterElementAttack,
            GrassElementAttack,
            Health,
            RecoveryCommandCount,
            Reward,
            FlinchDamage,
            RecoveryAmount,
        }

        public enum SkillLevelValueType
        {
            AttackUp,
            CriticalUp,
            DefenseUp,
            AbnormalStatusUp,
            ElementAttackUp,
            HealthUp,
            RecoveryCommandCountUp,
            RewardUp,
            FlinchDamageUp,
            RecoveryAmountUp,
        }

        public enum AbnormalStatusType
        {
            None,
            Poison,
            Paralysis,
            Collapse,
        }

        public enum ElementType
        {
            None,
            Fire,
            Water,
            Grass,
        }

        public enum GuardResult
        {
            NotGuard,
            SuccessGuard,
            FailedGuard,
            SuccessJustGuard,
        }

        public enum ArmorType
        {
            Head,
            Arms,
            Body,
        }

        public enum WeaponType
        {
            Sword,
            DualSword,
            Blade,
        }

        public enum AvaiableContentsEventTrigger
        {
            Home,
            Battle,
        }
    }
}
