using System;
using UnityEngine;

namespace MH3
{
    [Serializable]
    public class InstanceSkill
    {
        [SerializeField]
        private Define.SkillType skillType;
        public Define.SkillType SkillType => skillType;

        [SerializeField]
        private int level;
        public int Level => level;

        [SerializeField]
        private Define.RareType rareType;
        public Define.RareType RareType => rareType;

        public InstanceSkill(Define.SkillType skillType, int level, Define.RareType rareType)
        {
            this.skillType = skillType;
            this.level = level;
            this.rareType = rareType;
        }
    }
}
