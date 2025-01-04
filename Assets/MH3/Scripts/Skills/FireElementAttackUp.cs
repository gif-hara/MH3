using System.Threading;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class FireElementAttackUp : Skill
    {
        public FireElementAttackUp(int level) : base(Define.SkillType.FireElementAttackUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.ElementAttack.RegisterAdds(
                "Skill.FireElementAttackUp",
                () =>
                {
                    return owner.SpecController.ElementAttackType == Define.ElementType.Fire
                        ? Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillElementAttackUp.GetFromLevel(Level).Value)
                        : 0;
                }
                );
        }
    }
}
