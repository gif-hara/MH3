using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;

namespace MH3.AbnormalStatusSystems
{
    public sealed class Poison : AbnormalStatus
    {
        public override async UniTaskVoid Apply(Actor target)
        {
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            var count = Mathf.FloorToInt(target.SpecController.PoisonDuration / gameRules.PoisonInterval);
            var disposable = Observable.Interval(System.TimeSpan.FromSeconds(gameRules.PoisonInterval))
                .Take(count)
                .TakeUntil(target.SpecController.OnDead)
                .Subscribe((target, gameRules), static (_, t) =>
                {
                    var (target, gameRules) = t;
                    var damage = Mathf.FloorToInt(target.SpecController.HitPointMaxTotal * gameRules.PoisonDamageRate);
                    target.SpecController.TakeDamageFromPoison(damage);
                })
                .RegisterTo(target.destroyCancellationToken);
            await UniTask.WaitUntilCanceled(disposable.Token);
            target.SpecController.RemoveAppliedAbnormalStatus(Define.AbnormalStatusType.Poison);
        }
    }
}
