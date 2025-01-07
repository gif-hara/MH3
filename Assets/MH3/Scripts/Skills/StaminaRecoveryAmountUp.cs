using System.Threading;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class StaminaRecoveryAmountUp : Skill
    {
        public StaminaRecoveryAmountUp(int level) : base(Define.SkillType.StaminaRecoveryAmountUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.StaminaRecoveryAmount.RegisterAdds(
                "Skill.StaminaRecoveryAmountUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillStaminaRecoveryAmountUp.GetFromLevel(Level).Value)
                );
        }
    }
}
