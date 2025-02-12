using System;
using System.Collections.Generic;

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
        
        public void Set(int instanceWeaponId, int instanceArmorHeadId, int instanceArmorArmsId, int instanceArmorBodyId, List<int> instanceSkillCoreIds)
        {
            this.instanceWeaponId = instanceWeaponId;
            this.instanceArmorHeadId = instanceArmorHeadId;
            this.instanceArmorArmsId = instanceArmorArmsId;
            this.instanceArmorBodyId = instanceArmorBodyId;
            this.instanceSkillCoreIds.Clear();
            this.instanceSkillCoreIds.AddRange(instanceSkillCoreIds);
        }
    }
}
