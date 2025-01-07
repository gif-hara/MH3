using System.Threading;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class CriticalUpForAttackNoMiss : Skill
    {
        private int count = 0;

        public CriticalUpForAttackNoMiss(int level) : base(Define.SkillType.CriticalUpForAttackNoMiss, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.OnGiveDamage
                .Subscribe(this, static (_, @this) =>
                {
                    @this.count++;
                })
                .RegisterTo(scope);
            owner.SpecController.OnTakeDamage
                .Subscribe(this, static (_, @this) =>
                {
                    @this.count = 0;
                })
                .RegisterTo(scope);
            owner.SpecController.Critical.RegisterAdds(
                "Skill.CriticalUpForAttackNoMiss",
                () => Mathf.Clamp(
                    TinyServiceLocator.Resolve<MasterData>().SkillCriticalUpForAttackNoMiss.GetFromLevel(Level).Value * count,
                    0.0f,
                    TinyServiceLocator.Resolve<GameRules>().SkillCriticalUpForAttackNoMissLimit
                    )
                );
        }
    }
}
