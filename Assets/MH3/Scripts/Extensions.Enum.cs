using System;
using HK;

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
                Define.SkillType.DefenseUp => "防御力アップ",
                Define.SkillType.AbnormalStatusUp => "状態異常値アップ",
                Define.SkillType.FireElementAttackUp => "火属性値アップ",
                Define.SkillType.WaterElementAttackUp => "水属性値アップ",
                Define.SkillType.GrassElementAttackUp => "草属性値アップ",
                Define.SkillType.HealthUp => "体力アップ",
                Define.SkillType.RecoveryCommandCountUp => "回復回数アップ",
                Define.SkillType.RewardUp => "報酬アップ",
                _ => throw new System.NotImplementedException($"未対応のスキルタイプです {self}"),
            };
        }

        public static MasterData.SkillLevelValue.DictionaryList GetSkillLevelValue(this Define.SkillLevelValueType self)
        {
            var m = TinyServiceLocator.Resolve<MasterData>();
            return self switch
            {
                Define.SkillLevelValueType.AttackUp => m.SkillAttackUp,
                Define.SkillLevelValueType.CriticalUp => m.SkillCriticalUp,
                Define.SkillLevelValueType.DefenseUp => m.SkillDefenseUp,
                _ => throw new NotImplementedException($"未対応の値です. self: {self}")
            };
        }

        public static string GetName(this Define.AbnormalStatusType self)
        {
            return self switch
            {
                Define.AbnormalStatusType.Poison => "毒",
                Define.AbnormalStatusType.Paralysis => "麻痺",
                Define.AbnormalStatusType.Collapse => "崩壊",
                _ => throw new NotImplementedException($"未対応の異常状態です {self}"),
            };
        }
    }
}
