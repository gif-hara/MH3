using System.Threading;
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

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.Critical.RegisterAdds(
                "Skill.CriticalUp",
                () => TinyServiceLocator.Resolve<MasterData>().SkillCriticalUp.GetFromLevel(Level).Value
                );
        }
    }
}
