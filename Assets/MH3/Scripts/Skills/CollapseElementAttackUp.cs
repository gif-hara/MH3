using System.Threading;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class CollapseElementAttackUp : Skill
    {
        public CollapseElementAttackUp(int level) : base(Define.SkillType.CollapseElementAttackUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.AbnormalStatusAttack.RegisterAdds(
                "Skill.CollapseElementAttackUp",
                () =>
                {
                    return owner.SpecController.AbnormalStatusAttackType == Define.AbnormalStatusType.Collapse
                        ? Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillAbnormalStatusUp.GetFromLevel(Level).Value)
                        : 0;
                }
                );
        }
    }
}
