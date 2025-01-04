using System.Threading;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class SuccessJustGuardCriticalUp : Skill
    {
        public SuccessJustGuardCriticalUp(int level) : base(Define.SkillType.SuccessJustGuardCriticalUp, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            var successJustGuardTime = -9999.0f;
            owner.ActionController.JustGuarding
                .Where(x => x)
                .Subscribe(_ => successJustGuardTime = UnityEngine.Time.time)
                .RegisterTo(scope);
            owner.SpecController.Critical.RegisterAdds(
                "Skill.SuccessJustGuardCriticalUp",
                () =>
                {
                    var diffTime = UnityEngine.Time.time - successJustGuardTime;
                    if (diffTime > TinyServiceLocator.Resolve<GameRules>().SkillSuccessJustGuardCriticalUpDuration)
                    {
                        return 0;
                    }
                    return Mathf.FloorToInt(TinyServiceLocator.Resolve<MasterData>().SkillSuccessJustGuardCriticalUp.GetFromLevel(Level).Value);
                }
                );
        }
    }
}
