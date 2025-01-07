using System.Threading;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;

namespace MH3.SkillSystems
{
    public class RecoveryHitPointForAttack : Skill
    {
        public RecoveryHitPointForAttack(int level) : base(Define.SkillType.RecoveryHitPointForAttack, level)
        {
        }

        public override void Attach(Actor owner, CancellationToken scope)
        {
            owner.SpecController.OnGiveDamage
                .Subscribe((owner, this), static (x, t) =>
                {
                    var (owner, @this) = t;
                    var amount = Mathf.FloorToInt(x.Damage * TinyServiceLocator.Resolve<MasterData>()
                        .SkillRecoveryHitPointForAttack.GetFromLevel(@this.Level).Value);
                    owner.SpecController.AddHitPoint(amount);
                })
                .RegisterTo(scope);
        }
    }
}
