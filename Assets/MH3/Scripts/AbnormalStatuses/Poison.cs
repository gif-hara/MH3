using System;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.AbnormalStatusSystems
{
    public sealed class Poison : AbnormalStatus
    {
        public override async UniTaskVoid Apply(Actor target)
        {
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            var count = Mathf.FloorToInt(target.SpecController.PoisonDuration / gameRules.PoisonInterval);
            var effectManager = TinyServiceLocator.Resolve<EffectManager>();
            var (effectObject, pool) = effectManager.RentManual("AbnormalStatus.Poison.1");
            effectObject.transform.SetParent(target.LocatorHolder.Get("Poison"));
            effectObject.transform.localPosition = Vector3.zero;
            effectObject.transform.localRotation = Quaternion.identity;
            for (var i = 0; i < count; i++)
            {
                if(target.SpecController.IsDead)
                {
                    break;
                }
                await UniTask.Delay(TimeSpan.FromSeconds(gameRules.PoisonInterval), cancellationToken: target.destroyCancellationToken);
                var damage = Mathf.FloorToInt(target.SpecController.HitPointMaxTotal * gameRules.PoisonDamageRate);
                target.SpecController.TakeDamageFromPoison(damage);
            }
            target.SpecController.RemoveAppliedAbnormalStatus(Define.AbnormalStatusType.Poison);
            effectManager.Return(effectObject, pool);
        }
    }
}
