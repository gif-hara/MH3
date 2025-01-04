using System.Threading;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class LastComboAttackUp : Skill
    {
        public LastComboAttackUp(int level) : base(Define.SkillType.LastComboAttackUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.Attack.RegisterAdds(
                "Skill.LastComboAttackUp",
                () =>
                {
                    return owner.AttackController.HasNextCombo
                        ? 0
                        : Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillLastComboAttackUp.GetFromLevel(Level).Value);
                }
                );
        }
    }
}
