using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3.ActorControllers
{
    public class ActorActionController
    {
        private readonly Actor actor;

        public readonly ReactiveProperty<bool> CanGuard = new(true);

        public readonly ReactiveProperty<bool> IsGuard = new(false);

        private readonly ReactiveProperty<float> beginGuardTime = new(0.0f);

        public readonly ReactiveProperty<bool> JustGuarding = new(false);

        private readonly Subject<(float duration, CancellationDisposable scope)> onBeginDualSwordDodgeMode = new();
        public Observable<(float duration, CancellationDisposable scope)> OnBeginDualSwordDodgeMode => onBeginDualSwordDodgeMode;

        private readonly Subject<(float duration, CancellationDisposable scope)> onBeginBladeEnduranceMode = new();
        public Observable<(float duration, CancellationDisposable scope)> OnBeginBladeEnduranceMode => onBeginBladeEnduranceMode;

        private readonly Subject<Define.RecoveryCommandType> onBeginRecoveryCommand = new();
        public Observable<Define.RecoveryCommandType> OnBeginRecoveryCommand => onBeginRecoveryCommand;

        private CancellationDisposable DualSwordDodgeModeDisposable = null;

        private CancellationDisposable BladeEnduranceModeDisposable = null;

        public string OverrideDodgeAnimationName { get; private set; }

        private CancellationDisposable spearDodgeModeDisposable = null;

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
            Observable.Merge(
                actor.SpecController.OnBuildStatuses,
                actor.SpecController.OnReset
            )
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    @this.OverrideDodgeAnimationName = null;
                    @this.DualSwordDodgeModeDisposable?.Dispose();
                    @this.BladeEnduranceModeDisposable?.Dispose();
                })
                .RegisterTo(actor.destroyCancellationToken);
        }

        public bool TryDodge()
        {
            if (actor.SpecController.IsEventStop.Value)
            {
                return false;
            }
            if (actor.SpecController.Stamina.Value < 0)
            {
                return false;
            }
            return actor.StateMachine.TryChangeState(
                actor.SpecController.DodgeStateSequences,
                containerAction: c =>
                {
                    c.Register("AnimationName", GetDodgeAnimationName());
                });
        }

        public bool TryRecovery()
        {
            var specController = actor.SpecController;
            if (specController.IsEventStop.Value)
            {
                return false;
            }
            if (specController.RecoveryCommandCount.CurrentValue <= 0)
            {
                return false;
            }
            if (specController.RecoveryCommandType == Define.RecoveryCommandType.Recovery)
            {
                if (specController.HitPoint.CurrentValue >= specController.HitPointMaxTotal)
                {
                    return false;
                }
            }
            return actor.StateMachine.TryChangeState(specController.RecoverySequences);
        }

        public bool TrySharpen()
        {
            if (actor.SpecController.IsEventStop.Value)
            {
                return false;
            }
            if (actor.SpecController.Stamina.Value < 0)
            {
                return false;
            }
            return actor.StateMachine.TryChangeState(TinyServiceLocator.Resolve<GameRules>().SharpenStateSequences);
        }

        public bool TryEndurance()
        {
            if (actor.SpecController.IsEventStop.Value)
            {
                return false;
            }
            if (actor.SpecController.Stamina.Value < 0)
            {
                return false;
            }
            return actor.StateMachine.TryChangeState(TinyServiceLocator.Resolve<GameRules>().EnduranceStateSequences);
        }

        public void BeginRecoveryCommand()
        {
            var specController = actor.SpecController;
            if (specController.RecoveryCommandType == Define.RecoveryCommandType.Recovery)
            {
                var amount = TinyServiceLocator.Resolve<GameRules>().RecoveryAmount;
                amount += Mathf.FloorToInt(amount * specController.RecoveryAmountUp.Value);
                specController.AddHitPoint(amount);
            }
            onBeginRecoveryCommand.OnNext(specController.RecoveryCommandType);
        }

        public bool TryEmotion(string emotionName)
        {
            return actor.StateMachine.TryChangeState(
                TinyServiceLocator.Resolve<GameRules>().EmotionStateSequences,
                containerAction: c => c.Register("AttackName", emotionName),
                force: true
                );
        }

        public async UniTask BeginDualSwordDodgeModeAsync()
        {
            try
            {
                var gameRules = TinyServiceLocator.Resolve<GameRules>();
                DualSwordDodgeModeDisposable?.Dispose();
                DualSwordDodgeModeDisposable = new CancellationDisposable();
                OverrideDodgeAnimationName = gameRules.DualSwordDodgeAnimationName;
                onBeginDualSwordDodgeMode.OnNext((gameRules.DualSwordDodgeTime, DualSwordDodgeModeDisposable));
                await UniTask.Delay(TimeSpan.FromSeconds(gameRules.DualSwordDodgeTime), cancellationToken: DualSwordDodgeModeDisposable.Token, cancelImmediately: true);
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
                if (!DualSwordDodgeModeDisposable.IsDisposed)
                {
                    DualSwordDodgeModeDisposable.Dispose();
                }
                DualSwordDodgeModeDisposable = null;
                OverrideDodgeAnimationName = null;
            }
        }

        public async UniTask BeginBladeEnduranceModeAsync()
        {
            try
            {
                var gameRules = TinyServiceLocator.Resolve<GameRules>();
                BladeEnduranceModeDisposable?.Dispose();
                BladeEnduranceModeDisposable = new CancellationDisposable();
                actor.SpecController.SetSuperArmor(1);
                onBeginBladeEnduranceMode.OnNext((gameRules.BladeSuperArmorTime, BladeEnduranceModeDisposable));
                actor.SpecController.SuperArmorCount
                    .Where(x => x <= 0)
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        if (BladeEnduranceModeDisposable != null && !BladeEnduranceModeDisposable.IsDisposed)
                        {
                            BladeEnduranceModeDisposable.Dispose();
                        }
                    })
                    .AddTo(actor.destroyCancellationToken);
                await UniTask.Delay(TimeSpan.FromSeconds(gameRules.BladeSuperArmorTime), cancellationToken: BladeEnduranceModeDisposable.Token, cancelImmediately: true);
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
                if (!BladeEnduranceModeDisposable.IsDisposed)
                {
                    BladeEnduranceModeDisposable.Dispose();
                }
                BladeEnduranceModeDisposable = null;
                actor.SpecController.SetSuperArmor(0);
            }
        }

        public void BeginSpearDodgeMode()
        {
            if (spearDodgeModeDisposable != null)
            {
                return;
            }

            spearDodgeModeDisposable = new CancellationDisposable(CancellationTokenSource.CreateLinkedTokenSource(actor.destroyCancellationToken));
            actor.SpecController.OnEvade
                .Subscribe((this, actor), static (_, t) =>
                {
                    var (@this, actor) = t;
                    var gameRules = TinyServiceLocator.Resolve<GameRules>();
                    var container = new Container();
                    container.Register("Actor", actor);
                    var seruqncer = new Sequencer(container, gameRules.OnEvadeSpearSequences.Sequences);
                    seruqncer.PlayAsync(@this.actor.destroyCancellationToken).Forget();
                })
                .RegisterTo(spearDodgeModeDisposable.Token);
        }

        public void EndSpearDodgeMode()
        {
            spearDodgeModeDisposable?.Dispose();
            spearDodgeModeDisposable = null;
        }

        public string GetDodgeAnimationName()
        {
            return OverrideDodgeAnimationName ?? TinyServiceLocator.Resolve<GameRules>().DefaultDodgeAnimationName;
        }

        public Define.GuardResult GetGuardResult(Vector3 impactPosition)
        {
            if (!IsGuard.Value || (!actor.StateMachine.IsMatchState(actor.SpecController.GuardStateSequences) && !actor.StateMachine.IsMatchState(actor.SpecController.SuccessGuardSequences)))
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
