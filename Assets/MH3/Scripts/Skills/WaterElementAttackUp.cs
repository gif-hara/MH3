using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class WaterElementAttackUp : Skill
    {
        public WaterElementAttackUp(int level) : base(Define.SkillType.WaterElementAttackUp, level)
        {
        }

        public override void Attach(Actor owner)
        {
            owner.SpecController.ElementAttack.RegisterAdds(
                "Skill.WaterElementAttackUp",
                () =>
                {
                    return owner.SpecController.ElementAttackType == Define.ElementType.Water
                        ? Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillElementAttackUp.Get(Level).Value)
                        : 0;
                }
                );
        }
    }
}
