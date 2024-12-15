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
                Define.SkillType.PoisonUp => "毒属性値アップ",
                Define.SkillType.ParalysisUp => "麻痺属性値アップ",
                Define.SkillType.CollapseUp => "崩壊属性値アップ",
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
                Define.SkillLevelValueType.AbnormalStatusUp => m.SkillAbnormalStatusUp,
                Define.SkillLevelValueType.ElementAttackUp => m.SkillElementAttackUp,
                Define.SkillLevelValueType.HealthUp => m.SkillHealthUp,
                Define.SkillLevelValueType.RecoveryCommandCountUp => m.SkillRecoveryCommandCountUp,
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

        public static string GetName(this Define.ElementType self)
        {
            return self switch
            {
                Define.ElementType.Fire => "火",
                Define.ElementType.Water => "水",
                Define.ElementType.Grass => "草",
                _ => throw new NotImplementedException($"未対応の属性です {self}"),
            };
        }

        public static Define.ActorParameterType ToActorParameterType(this Define.ElementType self)
        {
            return self switch
            {
                Define.ElementType.Fire => Define.ActorParameterType.FireElementAttack,
                Define.ElementType.Water => Define.ActorParameterType.WaterElementAttack,
                Define.ElementType.Grass => Define.ActorParameterType.GrassElementAttack,
                _ => throw new NotImplementedException($"未対応の属性です {self}"),
            };
        }

        public static Define.ActorParameterType ToActorParameterType(this Define.AbnormalStatusType self)
        {
            return self switch
            {
                Define.AbnormalStatusType.Poison => Define.ActorParameterType.PoisonAttack,
                Define.AbnormalStatusType.Paralysis => Define.ActorParameterType.ParalysisAttack,
                Define.AbnormalStatusType.Collapse => Define.ActorParameterType.CollapseAttack,
                _ => throw new NotImplementedException($"未対応の異常状態です {self}"),
            };
        }
    }
}
