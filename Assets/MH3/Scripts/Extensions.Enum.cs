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
                Define.SkillType.AttackUp => "攻撃力アップ".Localized(),
                Define.SkillType.CriticalUp => "会心率アップ".Localized(),
                Define.SkillType.DefenseUp => "防御力アップ".Localized(),
                Define.SkillType.PoisonElementAttackUp => "毒属性値アップ".Localized(),
                Define.SkillType.ParalysisElementAttackUp => "麻痺属性値アップ".Localized(),
                Define.SkillType.CollapseElementAttackUp => "崩壊属性値アップ".Localized(),
                Define.SkillType.FireElementAttackUp => "火属性値アップ".Localized(),
                Define.SkillType.WaterElementAttackUp => "水属性値アップ".Localized(),
                Define.SkillType.GrassElementAttackUp => "草属性値アップ".Localized(),
                Define.SkillType.HitPointMaxUp => "体力アップ".Localized(),
                Define.SkillType.RecoveryCommandCountUp => "回復回数アップ".Localized(),
                Define.SkillType.RewardUp => "報酬アップ".Localized(),
                Define.SkillType.FlinchDamageUp => "怯み値アップ".Localized(),
                Define.SkillType.RecoveryAmountUp => "回復量アップ".Localized(),
                Define.SkillType.SuccessJustGuardCriticalUp => "見切り".Localized(),
                Define.SkillType.LastComboAttackUp => "渾身の一撃".Localized(),
                Define.SkillType.StaminaMaxUp => "スタミナアップ".Localized(),
                Define.SkillType.InvokeSharpenAttackUp => "研磨".Localized(),
                Define.SkillType.AttackUpForSuperArmor => "忍耐の力".Localized(),
                Define.SkillType.StaminaRecoveryAmountUp => "スタミナ回復速度アップ".Localized(),
                Define.SkillType.AttackUpForConsumeStamina => "オーバーヒート".Localized(),
                Define.SkillType.RecoveryStaminaForCritical => "会心の呼吸".Localized(),
                Define.SkillType.RecoveryHitPointForAttack => "吸血".Localized(),
                Define.SkillType.CriticalUpForAttackNoMiss => "完璧主義".Localized(),
                Define.SkillType.CriticalDamageUp => "会心の達人".Localized(),
                Define.SkillType.AttackUpForRecoveryCommand => "修羅の薬".Localized(),
                _ => throw new NotImplementedException($"未対応のスキルタイプです {self}"),
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
                Define.SkillLevelValueType.HealthUp => m.SkillHitPointMaxUp,
                Define.SkillLevelValueType.RecoveryCommandCountUp => m.SkillRecoveryCommandCountUp,
                Define.SkillLevelValueType.RewardUp => m.SkillRewardUp,
                Define.SkillLevelValueType.FlinchDamageUp => m.SkillFlinchDamageUp,
                _ => throw new NotImplementedException($"未対応の値です. self: {self}")
            };
        }

        public static string GetName(this Define.AbnormalStatusType self)
        {
            return self switch
            {
                Define.AbnormalStatusType.Poison => "毒".Localized(),
                Define.AbnormalStatusType.Paralysis => "麻痺".Localized(),
                Define.AbnormalStatusType.Collapse => "崩壊".Localized(),
                _ => throw new NotImplementedException($"未対応の異常状態です {self}"),
            };
        }

        public static string GetName(this Define.ElementType self)
        {
            return self switch
            {
                Define.ElementType.Fire => "火".Localized(),
                Define.ElementType.Water => "水".Localized(),
                Define.ElementType.Grass => "草".Localized(),
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

        public static MasterData.TermDescriptionSpec GetTermDescriptionSpec(this Define.WeaponType self)
        {
            return TinyServiceLocator.Resolve<MasterData>().TermDescriptionSpecs.Get($"WeaponType.{self}");
        }

        public static MasterData.TermDescriptionSpec GetTermDescriptionSpec(this Define.SkillType self)
        {
            return TinyServiceLocator.Resolve<MasterData>().TermDescriptionSpecs.Get($"Skill.{self}");
        }
    }
}
