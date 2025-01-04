using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class FlinchDamageUp : Skill
    {
        public FlinchDamageUp(int level) : base(Define.SkillType.FlinchDamageUp, level)
        {
        }

        public override void Attach(Actor owner)
        {
            owner.SpecController.FlinchDamageRate.RegisterAdds(
                "Skill.FlinchDamageUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillFlinchDamageUp.Get(Level).Value)
                );
        }
    }
}
