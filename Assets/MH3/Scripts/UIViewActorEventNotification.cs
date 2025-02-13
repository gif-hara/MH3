using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using UnityEngine;

namespace MH3
{
    public class UIViewActorEventNotification
    {
        private readonly HKUIDocument document;

        public static UIViewActorEventNotification Open(
            HKUIDocument documentPrefab,
            Actor actor,
            CancellationToken scope
            )
        {
            return new UIViewActorEventNotification(documentPrefab, actor, scope);
        }

        public UIViewActorEventNotification(HKUIDocument documentPrefab, Actor actor, CancellationToken scope)
        {
            document = Object.Instantiate(documentPrefab);
            var elementParent = document.Q<Transform>("Area.Elements");
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                document.DestroySafe();
            });
            Observable.Merge(
                actor.SpecController.OnEvade.Select(_ => "Element.Evade"),
                actor.SpecController.OnGuard.Where(x => x == Define.GuardResult.SuccessGuard).Select(_ => "Element.SuccessGuard"),
                actor.SpecController.OnGuard.Where(x => x == Define.GuardResult.SuccessJustGuard).Select(_ => "Element.SuccessJustGuard"),
                actor.SpecController.OnInvokeSuperArmor.Where(_ => actor.SpecController.WeaponSpec.WeaponType == Define.WeaponType.Blade).Select(_ => "Element.SuperArmor.Blade"),
                actor.SpecController.OnInvokeSuperArmor.Where(_ => actor.SpecController.WeaponSpec.WeaponType == Define.WeaponType.Shield).Select(_ => "Element.SuperArmor.Shield"),
                actor.SpecController.SpearComboLevel.Chunk(2, 1).Where(x => x[0] < x[1]).Select(_ => "Element.SpearComboLevelUp")
                )
                .Subscribe((document, elementParent), (x, t) =>
                {
                    var (document, elementParent) = t;
                    Object.Instantiate(document.Q<HKUIDocument>(x), elementParent);
                })
                .RegisterTo(document.destroyCancellationToken);
        }
    }
}
