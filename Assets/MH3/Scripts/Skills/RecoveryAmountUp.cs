using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class RecoveryAmountUp : Skill
    {
        public RecoveryAmountUp(int level) : base(Define.SkillType.RecoveryAmountUp, level)
        {
        }

        public override void Attach(Actor owner)
        {
            owner.SpecController.RecoveryAmountUp.RegisterAdds(
                "Skill.RecoveryAmountUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillRecoveryAmountUp.GetFromLevel(Level).Value)
                );
        }
    }
}
