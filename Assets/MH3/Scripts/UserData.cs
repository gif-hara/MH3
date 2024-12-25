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
        private int createdInstanceWeaponCount = 1;

        [SerializeField]
        private int createdInstanceSkillCoreCount = 1;

        [SerializeField]
        private int createdInstanceArmorCount = 1;

        [SerializeField]
        private int equippedInstanceWeaponId;
        public int EquippedInstanceWeaponId { get => equippedInstanceWeaponId; set => equippedInstanceWeaponId = value; }

        [SerializeField]
        private int equippedInstanceArmorHeadId;
        public int EquippedInstanceArmorHeadId { get => equippedInstanceArmorHeadId; set => equippedInstanceArmorHeadId = value; }
        
        [SerializeField]
        private int equippedInstanceArmorArmsId;
        public int EquippedInstanceArmorArmsId { get => equippedInstanceArmorArmsId; set => equippedInstanceArmorArmsId = value; }
        
        [SerializeField]
        private int equippedInstanceArmorBodyId;
        public int EquippedInstanceArmorBodyId { get => equippedInstanceArmorBodyId; set => equippedInstanceArmorBodyId = value; }
        
        [SerializeField]
        private AvailableContents availableContents = new();
        public AvailableContents AvailableContents => availableContents;

        public InstanceWeapon GetEquippedInstanceWeapon()
        {
            return instanceWeapons.FirstOrDefault(x => x.InstanceId == equippedInstanceWeaponId);
        }
        
        public InstanceArmor GetEquippedInstanceArmorHead()
        {
            return instanceArmors.FirstOrDefault(x => x.InstanceId == equippedInstanceArmorHeadId);
        }
        
        public InstanceArmor GetEquippedInstanceArmorArms()
        {
            return instanceArmors.FirstOrDefault(x => x.InstanceId == equippedInstanceArmorArmsId);
        }
        
        public InstanceArmor GetEquippedInstanceArmorBody()
        {
            return instanceArmors.FirstOrDefault(x => x.InstanceId == equippedInstanceArmorBodyId);
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
