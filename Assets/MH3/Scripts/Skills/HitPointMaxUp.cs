using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class HitPointMaxUp : Skill
    {
        public HitPointMaxUp(int level) : base(Define.SkillType.HitPointMaxUp, level)
        {
        }

        public override void Attach(Actor owner)
        {
            owner.SpecController.HitPointMax.RegisterAdds(
                "Skill.HitPointMaxUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillHealthUp.Get(Level).Value)
                );
        }
    }
}
