using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class RewardUp : Skill
    {
        public RewardUp(int level) : base(Define.SkillType.RewardUp, level)
        {
        }

        public override void Attach(Actor owner)
        {
            owner.SpecController.RewardUp.RegisterAdds(
                "Skill.RewardUp",
                () => Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillRewardUp.GetFromLevel(Level).Value)
                );
        }
    }
}
