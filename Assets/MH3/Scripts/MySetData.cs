using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace MH3
{
    [Serializable]
    public class MySetData
    {
        public int instanceWeaponId;
        
        public int instanceArmorHeadId;
        
        public int instanceArmorArmsId;
        
        public int instanceArmorBodyId;
        
        public List<int> instanceSkillCoreIds = new();

        public bool IsValid => instanceWeaponId != 0;

        public void Reset()
        {
            instanceWeaponId = 0;
            instanceArmorHeadId = 0;
            instanceArmorArmsId = 0;
            instanceArmorBodyId = 0;
            instanceSkillCoreIds.Clear();
        }
        
        public void Set(UserData userData, int instanceWeaponId, int instanceArmorHeadId, int instanceArmorArmsId, int instanceArmorBodyId)
        {
            this.instanceWeaponId = instanceWeaponId;
            this.instanceArmorHeadId = instanceArmorHeadId;
            this.instanceArmorArmsId = instanceArmorArmsId;
            this.instanceArmorBodyId = instanceArmorBodyId;
            var instanceWeapon = userData.GetInstanceWeapon(instanceWeaponId);
            this.instanceSkillCoreIds.Clear();
            this.instanceSkillCoreIds.AddRange(instanceWeapon.InstanceSkillCoreIds);
        }
        
        public void Equip(UserData userData)
        {
            userData.EquippedInstanceWeaponId = instanceWeaponId;
            userData.EquippedInstanceArmorHeadId = instanceArmorHeadId;
            userData.EquippedInstanceArmorArmsId = instanceArmorArmsId;
            userData.EquippedInstanceArmorBodyId = instanceArmorBodyId;
            var instanceWeapon = userData.GetInstanceWeapon(instanceWeaponId);
            instanceWeapon.AddRangeInstanceSkillCoreIds(instanceSkillCoreIds);
        }
    }
}
