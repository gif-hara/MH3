namespace MH3
{
    public abstract class Skill : ISkill
    {
        public Define.SkillType SkillType { get; }

        public int Level { get; private set; }

        public Skill(Define.SkillType skillType, int level)
        {
            SkillType = skillType;
            Level = level;
        }

        public void AddLevel(int level)
        {
            Level += level;
        }

        public virtual int GetAttack()
        {
            return 0;
        }

        public virtual int GetCritical()
        {
            return 0;
        }
    }
}
