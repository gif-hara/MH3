using System.Threading;
using HK;
using MH3.ActorControllers;

namespace MH3.SkillSystems
{
    public class AttackUpForConsumeStamina : Skill
    {
        public AttackUpForConsumeStamina(int level) : base(Define.SkillType.AttackUpForConsumeStamina, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.AttackStaminaCost.RegisterAdds(
                "Skill.AttackUpForConsumeStamina",
                () =>
                {
                    var gameRules = TinyServiceLocator.Resolve<GameRules>();
                    var masterData = TinyServiceLocator.Resolve<MasterData>();
                    return gameRules.AttackUpForConsumeStaminaCosts.Get(owner.SpecController.WeaponSpec.WeaponType).NeedStamina
                               * ((float)masterData.SkillAttackUpForConsumeStamina.GetFixedSkillLevel(Level) / masterData.SkillAttackUpForConsumeStamina.List.Count);
                });
            owner.SpecController.Attack.RegisterMultiplies(
                "Skill.AttackUpForConsumeStamina",
                () => TinyServiceLocator.Resolve<MasterData>().SkillAttackUpForConsumeStamina.GetFromLevel(Level).Value);
        }
    }
}
