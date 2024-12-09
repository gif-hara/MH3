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
        }

        public enum SkillType
        {
            AttackUp,
            CriticalUp,
        }
    }
}
