using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class RecoveryCommandCountUp : Skill
    {
        public RecoveryCommandCountUp(int level) : base(Define.SkillType.RecoveryCommandCountUp, level)
        {
        }

        public override void Attach(Actor owner)
        {
            owner.SpecController.RecoveryCommandCountMax.RegisterAdds(
                "Skill.RecoveryCommandCountUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillRecoveryCommandCountUp.Get(Level).Value)
                );
        }
    }
}
