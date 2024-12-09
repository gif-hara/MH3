namespace MH3.SkillSystems
{
    public sealed class AttackUp : Skill
    {
        public override Define.SkillType SkillType => Define.SkillType.AttackUp;

        public AttackUp(int level) : base(level)
        {
        }

        public override int GetAttack()
        {
            return Level * 10;
        }
    }
}
