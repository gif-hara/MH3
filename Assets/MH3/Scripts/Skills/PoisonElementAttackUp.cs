using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class PoisonElementAttackUp : Skill
    {
        public PoisonElementAttackUp(int level) : base(Define.SkillType.PoisonElementAttackUp, level)
        {
        }

        public override void Attach(Actor owner)
        {
            owner.SpecController.AbnormalStatusAttack.RegisterAdds(
                "Skill.PoisonElementAttackUp",
                () =>
                {
                    return owner.SpecController.AbnormalStatusAttackType == Define.AbnormalStatusType.Poison
                        ? Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillAbnormalStatusUp.GetFromLevel(Level).Value)
                        : 0;
                }
                );
        }
    }
}
