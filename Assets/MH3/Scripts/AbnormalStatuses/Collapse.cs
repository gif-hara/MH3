using Cysharp.Threading.Tasks;
using MH3.ActorControllers;

namespace MH3.AbnormalStatusSystems
{
    public sealed class Collapse : AbnormalStatus
    {
        public override async UniTaskVoid Apply(Actor target)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(target.SpecController.CollapseDuration), cancellationToken: target.destroyCancellationToken);
            target.SpecController.RemoveAppliedAbnormalStatus(Define.AbnormalStatusType.Collapse);
        }
    }
}
