namespace MH3
{
    public static class InstanceWeaponFactory
    {
        public static InstanceWeaponData Create(int weaponSpecId)
        {
            return new InstanceWeaponData(weaponSpecId, 100);
        }
    }
}
