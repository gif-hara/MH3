namespace MH3
{
    public static class InstanceWeaponFactory
    {
        public static InstanceWeaponData Create(UserData userData, int weaponSpecId)
        {
            var instanceWeaponData = new InstanceWeaponData(weaponSpecId, 100);
            userData.AddInstanceWeaponData(instanceWeaponData);
            return instanceWeaponData;
        }
    }
}
