using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class UserData
    {
        [SerializeField]
        private List<InstanceWeapon> instanceWeaponDataList = new();
        public List<InstanceWeapon> InstanceWeaponDataList => instanceWeaponDataList;

        [SerializeField]
        private List<InstanceSkillCore> instanceSkillCoreList = new();
        public List<InstanceSkillCore> InstanceSkillCoreList => instanceSkillCoreList;

        [SerializeField]
        private int createdInstanceWeaponCount;

        [SerializeField]
        private int createdInstanceSkillCoreCount;

        [SerializeField]
        private int equippedInstanceWeaponId;
        public int EquippedInstanceWeaponId { get => equippedInstanceWeaponId; set => equippedInstanceWeaponId = value; }

        public void AddInstanceWeaponData(InstanceWeapon instanceWeaponData)
        {
            instanceWeaponDataList.Add(instanceWeaponData);
        }

        public void AddInstanceSkillCoreData(InstanceSkillCore instanceSkillCore)
        {
            instanceSkillCoreList.Add(instanceSkillCore);
        }

        public int GetAndIncrementCreatedInstanceWeaponCount()
        {
            return createdInstanceWeaponCount++;
        }

        public int GetAndIncrementCreatedInstanceSkillCoreCount()
        {
            return createdInstanceSkillCoreCount++;
        }
    }
}
