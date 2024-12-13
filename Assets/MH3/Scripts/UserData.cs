using System;
using System.Collections.Generic;
using System.Linq;
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

        public InstanceWeapon GetEquippedInstanceWeapon()
        {
            return instanceWeaponDataList.FirstOrDefault(x => x.InstanceId == equippedInstanceWeaponId);
        }

        public void AddInstanceWeaponData(InstanceWeapon instanceWeaponData)
        {
            instanceWeaponDataList.Add(instanceWeaponData);
        }

        public void RemoveInstanceWeaponData(InstanceWeapon instanceWeaponData)
        {
            instanceWeaponDataList.Remove(instanceWeaponData);
        }

        public void AddInstanceSkillCoreData(InstanceSkillCore instanceSkillCore)
        {
            instanceSkillCoreList.Add(instanceSkillCore);
        }

        public void RemoveInstanceSkillCoreData(InstanceSkillCore instanceSkillCore)
        {
            foreach (var instanceWeaponData in instanceWeaponDataList)
            {
                instanceWeaponData.InstanceSkillCoreIds.Remove(instanceSkillCore.InstanceId);
            }
            instanceSkillCoreList.Remove(instanceSkillCore);
        }

        public bool AnyAttachedSkillCore(int instanceSkillCoreId)
        {
            return instanceWeaponDataList.Any(x => x.InstanceSkillCoreIds.Any(y => y == instanceSkillCoreId));
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
