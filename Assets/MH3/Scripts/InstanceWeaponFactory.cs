using HK;

namespace MH3
{
    public static class InstanceWeaponFactory
    {
        public static InstanceWeaponData Create(int weaponSpecId)
        {
            var weaponSpec = TinyServiceLocator.Resolve<MasterData>().WeaponSpecs.Get(weaponSpecId);
            var weaponAttack = weaponSpec.GetAttacks().Lottery(x => x.Weight);
            return new InstanceWeaponData(weaponSpecId, weaponAttack.Attack, weaponAttack.RareType);
        }
    }
}
