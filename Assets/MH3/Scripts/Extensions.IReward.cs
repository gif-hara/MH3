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
                    userData.AddInstanceSkillCoreData(instanceSkillCore);
                    break;
            }
        }
    }
}
