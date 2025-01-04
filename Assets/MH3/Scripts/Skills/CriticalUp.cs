using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class CriticalUp : Skill
    {
        public CriticalUp(int level) : base(Define.SkillType.CriticalUp, level)
        {
        }

        public override void Attach(Actor owner)
        {
            owner.SpecController.Critical.RegisterAdds(
                "Skill.CriticalUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillCriticalUp.Get(Level).Value)
                );
        }
    }
}
