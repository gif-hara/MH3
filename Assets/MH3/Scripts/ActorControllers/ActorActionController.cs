using System;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
using UnityEngine;

namespace MH3.ActorControllers
{
    public class ActorActionController
    {
        private readonly Actor actor;

        public readonly ReactiveProperty<bool> CanGuard = new(true);

        public readonly ReactiveProperty<bool> IsGuard = new(false);

        private readonly ReactiveProperty<float> beginGuardTime = new(0.0f);

        public readonly ReactiveProperty<bool> JustGuarding = new(false);

        private CancellationDisposable DualSwordDodgeDisposable = null;

        private CancellationDisposable BladeEnduranceDisposable = null;

        public string OverrideDodgeAnimationName { get; private set; }

        public ActorActionController(Actor actor)
        {
            this.actor = actor;
            actor.UpdateAsObservable()
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    if (!@this.CanGuard.Value)
                    {
                        return;
                    }
                    if (
                        @this.IsGuard.Value
                        && !actor.StateMachine.IsMatchState(actor.SpecController.GuardStateSequences)
                        && actor.StateMachine.TryChangeState(actor.SpecController.GuardStateSequences))
                    {
                        @this.beginGuardTime.Value = UnityEngine.Time.time;
                    }
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        public bool TryDodge()
        {
            return actor.StateMachine.TryChangeState(
                actor.SpecController.DodgeStateSequences,
                containerAction: c =>
                {
                    c.Register("DodgeName", "Dodge");
                });
        }

        public async UniTask BeginDualSwordDodgeModeAsync()
        {
            try
            {
                var gameRules = TinyServiceLocator.Resolve<GameRules>();
                DualSwordDodgeDisposable?.Dispose();
                DualSwordDodgeDisposable = new CancellationDisposable();
                OverrideDodgeAnimationName = gameRules.DualSwordDodgeAnimationName;
                await UniTask.Delay(TimeSpan.FromSeconds(gameRules.DualSwordDodgeTime), cancellationToken: DualSwordDodgeDisposable.Token, cancelImmediately: true);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                if (!DualSwordDodgeDisposable.IsDisposed)
                {
                    DualSwordDodgeDisposable.Dispose();
                }
                DualSwordDodgeDisposable = null;
                OverrideDodgeAnimationName = null;
            }
        }

        public async UniTask BeginBladeEnduranceAsync()
        {
            try
            {
                var gameRules = TinyServiceLocator.Resolve<GameRules>();
                BladeEnduranceDisposable?.Dispose();
                BladeEnduranceDisposable = new CancellationDisposable();
                actor.SpecController.SetSuperArmor(1);
                await UniTask.Delay(TimeSpan.FromSeconds(gameRules.BladeSuperArmorTime), cancellationToken: BladeEnduranceDisposable.Token, cancelImmediately: true);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                if (!BladeEnduranceDisposable.IsDisposed)
                {
                    BladeEnduranceDisposable.Dispose();
                }
                BladeEnduranceDisposable = null;
                actor.SpecController.SetSuperArmor(0);
            }
        }

        public string GetDodgeAnimationName()
        {
            return OverrideDodgeAnimationName ?? TinyServiceLocator.Resolve<GameRules>().DefaultDodgeAnimationName;
        }

        public Define.GuardResult GetGuardResult(Vector3 impactPosition)
        {
            if (!IsGuard.Value || !actor.StateMachine.IsMatchState(actor.SpecController.GuardStateSequences))
            {
                return Define.GuardResult.NotGuard;
            }

            var forward = actor.transform.forward;
            forward.y = 0.0f;
            forward.Normalize();
            impactPosition.y = 0.0f;
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            var targetDirection = actor.transform.position - impactPosition;
            targetDirection.y = 0.0f;
            targetDirection.Normalize();
            var guardRange = gameRules.GuardRange;
            var guardAngle = (1.0f - Vector3.Dot(forward, targetDirection) * -1) * 90.0f;
            var successGuard = guardAngle < guardRange / 2.0f;
            if (successGuard)
            {
                var guardTime = UnityEngine.Time.time - beginGuardTime.Value;
                return guardTime <= gameRules.JustGuardTime ? Define.GuardResult.SuccessJustGuard : Define.GuardResult.SuccessGuard;
            }
            else
            {
                return Define.GuardResult.FailedGuard;
            }
        }
    }
}
