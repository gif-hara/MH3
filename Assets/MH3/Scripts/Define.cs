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
            AttackUp,
            CriticalUp,
            DefenseUp,
            PoisonUp,
            ParalysisUp,
            CollapseUp,
            FireElementAttackUp,
            WaterElementAttackUp,
            GrassElementAttackUp,
            HealthUp,
            RecoveryCommandCountUp,
            RewardUp,
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
    }
}
