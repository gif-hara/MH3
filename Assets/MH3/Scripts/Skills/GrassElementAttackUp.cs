using System.Threading;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class GrassElementAttackUp : Skill
    {
        public GrassElementAttackUp(int level) : base(Define.SkillType.GrassElementAttackUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.ElementAttack.RegisterAdds(
                "Skill.GrassElementAttackUp",
                () =>
                {
                    return owner.SpecController.ElementAttackType == Define.ElementType.Grass
                        ? Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillElementAttackUp.GetFromLevel(Level).Value)
                        : 0;
                }
                );
        }
    }
}
