using System.Threading;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class ParalysisElementAttackUp : Skill
    {
        public ParalysisElementAttackUp(int level) : base(Define.SkillType.ParalysisElementAttackUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.AbnormalStatusAttack.RegisterAdds(
                "Skill.ParalysisElementAttackUp",
                () =>
                {
                    return owner.SpecController.AbnormalStatusAttackType == Define.AbnormalStatusType.Paralysis
                        ? Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillAbnormalStatusUp.GetFromLevel(Level).Value)
                        : 0;
                }
                );
        }
    }
}
