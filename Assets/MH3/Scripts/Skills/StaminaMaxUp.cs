using System.Threading;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class StaminaMaxUp : Skill
    {
        public StaminaMaxUp(int level) : base(Define.SkillType.StaminaMaxUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.StaminaMax.RegisterAdds(
                "Skill.StaminaMaxUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillStaminaMaxUp.GetFromLevel(Level).Value)
                );
        }
    }
}
