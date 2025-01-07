using System.Threading;
using HK;
using MH3.ActorControllers;

namespace MH3.SkillSystems
{
    public class CriticalDamageUp : Skill
    {
        public CriticalDamageUp(int level) : base(Define.SkillType.CriticalDamageUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.CriticalDamageRate.RegisterAdds(
                "Skill.CriticalDamageUp",
                () => TinyServiceLocator.Resolve<MasterData>().SkillCriticalDamageUp.GetFromLevel(Level).Value
                );
        }
    }
}
