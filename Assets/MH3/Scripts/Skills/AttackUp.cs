using System.Threading;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class AttackUp : Skill
    {
        public AttackUp(int level) : base(Define.SkillType.AttackUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.Attack.RegisterAdds(
                "Skill.AttackUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillAttackUp.GetFromLevel(Level).Value)
                );
        }
    }
}
