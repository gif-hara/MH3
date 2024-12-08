namespace MH3
{
    public interface ISkill
    {
        Define.Skilltype SkillType { get; }

        int Level { get; }

        void AddLevel(int level);

        int GetAttack();

        int GetCritical();
    }
}
