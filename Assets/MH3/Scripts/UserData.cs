using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace MH3
{
    [Serializable]
    public class UserData
    {
        [SerializeField]
        private List<InstanceWeapon> instanceWeapons = new();
        public List<InstanceWeapon> InstanceWeapons => instanceWeapons;

        [SerializeField]
        private List<InstanceSkillCore> instanceSkillCores = new();
        public List<InstanceSkillCore> InstanceSkillCores => instanceSkillCores;
        
        [SerializeField]
        private List<InstanceArmor> instanceArmors = new();
        public List<InstanceArmor> InstanceArmors => instanceArmors;

        [SerializeField]
        private int createdInstanceWeaponCount;

        [SerializeField]
        private int createdInstanceSkillCoreCount;

        [SerializeField]
        private int createdInstanceArmorCount;

        [SerializeField]
        private int equippedInstanceWeaponId;
        public int EquippedInstanceWeaponId { get => equippedInstanceWeaponId; set => equippedInstanceWeaponId = value; }
        
        [SerializeField]
        private AvailableContents availableContents = new();
        public AvailableContents AvailableContents => availableContents;

        public InstanceWeapon GetEquippedInstanceWeapon()
        {
            return instanceWeapons.FirstOrDefault(x => x.InstanceId == equippedInstanceWeaponId);
        }

        public void AddInstanceWeaponData(InstanceWeapon instanceWeaponData)
        {
            instanceWeapons.Add(instanceWeaponData);
        }

        public void RemoveInstanceWeapon(InstanceWeapon instanceWeaponData)
        {
            instanceWeapons.Remove(instanceWeaponData);
        }

        public void AddInstanceSkillCore(InstanceSkillCore instanceSkillCore)
        {
            instanceSkillCores.Add(instanceSkillCore);
        }

        public void RemoveInstanceSkillCore(InstanceSkillCore instanceSkillCore)
        {
            foreach (var instanceWeaponData in instanceWeapons)
            {
                instanceWeaponData.InstanceSkillCoreIds.Remove(instanceSkillCore.InstanceId);
            }
            instanceSkillCores.Remove(instanceSkillCore);
        }
        
        public void AddInstanceArmor(InstanceArmor instanceArmor)
        {
            instanceArmors.Add(instanceArmor);
        }
        
        public void RemoveInstanceArmor(InstanceArmor instanceArmor)
        {
            instanceArmors.Remove(instanceArmor);
        }

        public bool AnyAttachedSkillCore(int instanceSkillCoreId)
        {
            return instanceWeapons.Any(x => x.InstanceSkillCoreIds.Any(y => y == instanceSkillCoreId));
        }

        public int GetAndIncrementCreatedInstanceWeaponCount()
        {
            return createdInstanceWeaponCount++;
        }

        public int GetAndIncrementCreatedInstanceSkillCoreCount()
        {
            return createdInstanceSkillCoreCount++;
        }
        
        public int GetAndIncrementCreatedInstanceArmorCount()
        {
            return createdInstanceArmorCount++;
        }
    }
}
