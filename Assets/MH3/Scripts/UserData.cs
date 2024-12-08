using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class UserData
    {
        [SerializeField]
        private List<InstanceWeaponData> instanceWeaponDataList = new();
        public List<InstanceWeaponData> InstanceWeaponDataList => instanceWeaponDataList;

        [SerializeField]
        private List<InstanceSkillCore> instanceSkillCoreList = new();
        public List<InstanceSkillCore> InstanceSkillCoreList => instanceSkillCoreList;

        [SerializeField]
        private int createdInstanceWeaponCount;

        [SerializeField]
        private int createdInstanceSkillCoreCount;

        public void AddInstanceWeaponData(InstanceWeaponData instanceWeaponData)
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
