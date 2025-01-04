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

        public override void Attach(Actor owner)
        {
            owner.SpecController.Attack.RegisterAdds(
                "Skill.FireElementAttackUp",
                () =>
                {
                    return owner.SpecController.ElementAttackType == Define.ElementType.Fire
                        ? Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillElementAttackUp.Get(Level).Value)
                        : 0;
                }
                );
        }
    }
}
