using System.Threading;
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

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.Defense.RegisterAdds(
                "Skill.DefenseUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillDefenseUp.GetFromLevel(Level).Value)
                );
        }
    }
}
