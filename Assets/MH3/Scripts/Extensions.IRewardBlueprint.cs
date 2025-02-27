using System;
using HK;

namespace MH3
{
    public static partial class Extensions
    {
        public static string GetName(this IRewardBlueprint self)
        {
            return self.Type switch
            {
                Define.RewardType.InstanceWeapon =>
                    TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(self.Id).LocalizedName,
                Define.RewardType.InstanceSkillCore =>
                    TinyServiceLocator.Resolve<MasterData>().SkillCoreSpecs.Get(self.Id).LocalizedName,
                Define.RewardType.InstanceArmor =>
                    TinyServiceLocator.Resolve<MasterData>().ArmorSpecs.Get(self.Id).LocalizedName,
                _ => throw new ArgumentOutOfRangeException($"未対応のタイプです {self.Type}")
            };
        }

        public static string GetSeenKey(this IRewardBlueprint self)
        {
            return self.Type switch
            {
                Define.RewardType.InstanceWeapon => AvailableContents.Key.GetSeenWeapon(self.Id),
                Define.RewardType.InstanceSkillCore => AvailableContents.Key.GetSeenSkillCore(self.Id),
                Define.RewardType.InstanceArmor => AvailableContents.Key.GetSeenArmor(self.Id),
                _ => throw new ArgumentOutOfRangeException($"未対応のタイプです {self.Type}")
            };
        }
    }
}
