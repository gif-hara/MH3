using System.Threading;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class InvokeSharpenAttackUp : Skill
    {
        public InvokeSharpenAttackUp(int level) : base(Define.SkillType.InvokeSharpenAttackUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.Attack.RegisterAdds(
                "Skill.InvokeSharpenAttackUp",
                () => Mathf.Clamp(
                    Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillInvokeSharpenAttackUp.GetFromLevel(Level).Value) * owner.SpecController.InvokeSharpenCount,
                    0,
                    TinyServiceLocator.Resolve<GameRules>().SkillInvokeSharpenAttackUpAttackMax
                    )
                );
        }
    }
}
