using System.Collections.Generic;

namespace MH3
{
    public class UserData
    {
        private List<InstanceWeaponData> instanceWeaponDataList = new();
        public List<InstanceWeaponData> InstanceWeaponDataList => instanceWeaponDataList;

        public void AddInstanceWeaponData(InstanceWeaponData instanceWeaponData)
        {
            instanceWeaponDataList.Add(instanceWeaponData);
        }
    }
}
