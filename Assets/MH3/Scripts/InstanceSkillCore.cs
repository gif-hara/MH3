using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class InstanceSkillCore
    {
        [SerializeField]
        private int slot;
        public int Slot => slot;

        [SerializeField]
        private List<InstanceSkill> skills;
        public List<InstanceSkill> Skills => skills;

        public InstanceSkillCore(int slot, List<InstanceSkill> skills)
        {
            this.slot = slot;
            this.skills = skills;
        }
    }
}
