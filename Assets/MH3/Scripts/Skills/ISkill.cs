namespace MH3.SkillSystems
{
    public interface ISkill
    {
        Define.SkillType SkillType { get; }

        int Level { get; }

        void AddLevel(int level);

        int GetAttack();

        int GetCritical();
    }
}
