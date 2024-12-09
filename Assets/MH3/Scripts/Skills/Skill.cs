namespace MH3.SkillSystems
{
    public abstract class Skill : ISkill
    {
        public abstract Define.SkillType SkillType { get; }

        public int Level { get; private set; }

        public Skill(int level)
        {
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
