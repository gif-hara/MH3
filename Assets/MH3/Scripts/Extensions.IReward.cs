namespace MH3
{
    public static partial class Extensions
    {
        public static void Acquire(this IReward reward, UserData userData)
        {
            switch (reward)
            {
                case InstanceWeapon instanceWeapon:
                    userData.AddInstanceWeaponData(instanceWeapon);
                    break;
                case InstanceSkillCore instanceSkillCore:
                    userData.AddInstanceSkillCore(instanceSkillCore);
                    break;
                case InstanceArmor instanceArmor:
                    userData.AddInstanceArmor(instanceArmor);
                    break;
            }
        }
    }
}
