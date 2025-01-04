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

        public override void Attach(Actor owner)
        {
            owner.SpecController.Attack.RegisterAdds(
                "Skill.GrassElementAttackUp",
                () =>
                {
                    return owner.SpecController.ElementAttackType == Define.ElementType.Grass
                        ? Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillElementAttackUp.Get(Level).Value)
                        : 0;
                }
                );
        }
    }
}
