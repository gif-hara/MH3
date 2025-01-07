using System.Threading;
using HK;
using MH3.ActorControllers;
using R3;

namespace MH3.SkillSystems
{
    public class RecoveryStaminaForCritical : Skill
    {
        public RecoveryStaminaForCritical(int level) : base(Define.SkillType.RecoveryStaminaForCritical, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.OnGiveDamage
                .Where(x => x.IsCritical)
                .Subscribe((owner, this), static (_, t) =>
                {
                    var (owner, @this) = t;
                    owner.SpecController.AddStamina(TinyServiceLocator.Resolve<MasterData>()
                        .SkillRecoveryStaminaForCritical.GetFromLevel(@this.Level).Value);
                })
                .RegisterTo(scope);
        }
    }
}
