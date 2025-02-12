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
    }
}
