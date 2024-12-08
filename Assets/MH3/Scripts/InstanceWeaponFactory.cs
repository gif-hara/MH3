using HK;

namespace MH3
{
    public static class InstanceWeaponFactory
    {
        public static InstanceWeapon Create(UserData userData, int weaponSpecId)
        {
            var weaponSpec = TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(weaponSpecId);
            var weaponAttack = weaponSpec.GetAttacks().Lottery(x => x.Weight);
            var weaponCritical = weaponSpec.GetCriticals().Lottery(x => x.Weight);
            var weaponSkillSlot = weaponSpec.GetSkillSlots().Lottery(x => x.Weight);
            return new InstanceWeapon(
                userData.GetAndIncrementCreatedInstanceWeaponCount(),
                weaponSpecId,
                weaponAttack.Attack,
                weaponAttack.RareType,
                weaponCritical.Critical,
                weaponCritical.RareType,
                weaponSkillSlot.SkillSlot,
                weaponSkillSlot.RareType
                );
        }
    }
}
