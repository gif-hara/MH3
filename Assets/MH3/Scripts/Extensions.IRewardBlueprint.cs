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
                    TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(self.Id).Name,
                Define.RewardType.InstanceSkillCore =>
                    TinyServiceLocator.Resolve<MasterData>().SkillCoreSpecs.Get(self.Id).Name,
                Define.RewardType.InstanceArmor =>
                    TinyServiceLocator.Resolve<MasterData>().ArmorSpecs.Get(self.Id).Name,
                _ => throw new ArgumentOutOfRangeException($"未対応のタイプです {self.Type}")
            };
        }

        public static string GetAvailableContentsAcquireKey(this IRewardBlueprint self)
        {
            return self.Type switch
            {
                Define.RewardType.InstanceWeapon => AvailableContents.Key.GetAcquireWeapon(self.Id),
                Define.RewardType.InstanceSkillCore => AvailableContents.Key.GetAcquireSkillCore(self.Id),
                Define.RewardType.InstanceArmor => AvailableContents.Key.GetAcquireArmor(self.Id),
                _ => throw new ArgumentOutOfRangeException($"未対応のタイプです {self.Type}")
            };
        }
    }
}
