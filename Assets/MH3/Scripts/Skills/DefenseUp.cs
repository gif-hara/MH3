using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class DefenseUp : Skill
    {
        public DefenseUp(int level) : base(Define.SkillType.DefenseUp, level)
        {
        }

        public override void Attach(Actor owner)
        {
            owner.SpecController.Attack.RegisterAdds(
                "Skill.DefenseUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillDefenseUp.Get(Level).Value)
                );
        }
    }
}
