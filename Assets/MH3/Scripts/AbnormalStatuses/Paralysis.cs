using System;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using UnityEngine;

namespace MH3.AbnormalStatusSystems
{
    public sealed class Paralysis : AbnormalStatus
    {
        public override async UniTaskVoid Apply(Actor target)
        {
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            target.StateMachine.TryChangeState(gameRules.ParalysisBeginSequence, true);
            await UniTask.Delay(TimeSpan.FromSeconds(target.SpecController.ParalysisDuration), cancellationToken: target.destroyCancellationToken);
            if (target.SpecController.IsDead)
            {
                return;
            }
            target.StateMachine.TryChangeState(gameRules.ParalysisEndSequence, true);
            target.SpecController.RemoveAppliedAbnormalStatus(Define.AbnormalStatusType.Paralysis);
        }
    }
}
