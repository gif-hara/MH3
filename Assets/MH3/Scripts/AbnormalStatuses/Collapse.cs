using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;

namespace MH3.AbnormalStatusSystems
{
    public sealed class Collapse : AbnormalStatus
    {
        public override async UniTaskVoid Apply(Actor target)
        {
            var effectManager = TinyServiceLocator.Resolve<EffectManager>();
            var (effectObject, pool) = effectManager.RentManual("AbnormalStatus.Collapse.1");
            var parent = target.LocatorHolder.Get("Spine");
            effectObject.transform.SetParent(parent);
            effectObject.transform.localPosition = UnityEngine.Vector3.zero;
            effectObject.transform.localRotation = UnityEngine.Quaternion.identity;
            effectObject.transform.localScale = UnityEngine.Vector3.one;
            await UniTask.Delay(System.TimeSpan.FromSeconds(target.SpecController.CollapseDuration), cancellationToken: target.destroyCancellationToken);
            target.SpecController.RemoveAppliedAbnormalStatus(Define.AbnormalStatusType.Collapse);
            effectManager.Return(effectObject, pool);
        }
    }
}
