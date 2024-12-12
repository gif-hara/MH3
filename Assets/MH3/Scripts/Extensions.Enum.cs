namespace MH3
{
    public static partial class Extensions
    {
        public static bool Evaluate(this Define.ComparisonType self, float a, float b)
        {
            switch (self)
            {
                case Define.ComparisonType.Equal:
                    return a == b;
                case Define.ComparisonType.NotEqual:
                    return a != b;
                case Define.ComparisonType.Greater:
                    return a > b;
                case Define.ComparisonType.Less:
                    return a < b;
                case Define.ComparisonType.GreaterOrEqual:
                    return a >= b;
                case Define.ComparisonType.LessOrEqual:
                    return a <= b;
                default:
                    return false;
            }
        }

        public static string GetName(this Define.SkillType self)
        {
            return self switch
            {
                Define.SkillType.AttackUp => "攻撃力アップ",
                Define.SkillType.CriticalUp => "会心率アップ",
                _ => throw new System.NotImplementedException($"未対応のスキルタイプです {self}"),
            };
        }
    }
}
