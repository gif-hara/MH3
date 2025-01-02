using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.ActorControllers;
using R3;
using R3.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MH3
{
    public class UIViewPlayerStatus : UIView
    {
        private readonly HKUIDocument document;

        public UIViewPlayerStatus(HKUIDocument documentPrefab, Actor actor, CancellationToken scope)
            : base(scope)
        {
            var gameRules = TinyServiceLocator.Resolve<GameRules>();
            document = UnityEngine.Object.Instantiate(documentPrefab);
            var hitPointSlider = document.Q<HKUIDocument>("Slider.HitPoint").Q<Slider>("Slider");
            var hitPointSliderDefaultSize = ((RectTransform)hitPointSlider.transform).sizeDelta;
            ((RectTransform)hitPointSlider.transform).sizeDelta = new Vector2(
                gameRules.HitPointSliderAddWidth * actor.SpecController.HitPointMax,
                hitPointSliderDefaultSize.y
                );
            actor.SpecController.HitPoint
                .Subscribe((hitPointSlider, actor), static (_, t) =>
                {
                    var (hitPointSlider, actor) = t;
                    hitPointSlider.value = (float)actor.SpecController.HitPoint.CurrentValue / actor.SpecController.HitPointMax;
                })
                .RegisterTo(scope);
            actor.SpecController.OnBuildStatuses
                .Subscribe((hitPointSlider, hitPointSliderDefaultSize, gameRules, actor), static (_, t) =>
                {
                    var (hitPointSlider, hitPointSliderDefaultSize, gameRules, actor) = t;
                    ((RectTransform)hitPointSlider.transform).sizeDelta = new Vector2(
                        gameRules.HitPointSliderAddWidth * actor.SpecController.HitPointMax,
                        hitPointSliderDefaultSize.y
                        );
                })
                .RegisterTo(scope);
            actor.SpecController.RecoveryCommandCount
                .Subscribe((document, actor), static (x, t) =>
                {
                    var (document, actor) = t;
                    document.Q<HKUIDocument>("RecoveryCommandCount")
                        .Q<TMP_Text>("Value")
                        .text = x.ToString();
                })
                .RegisterTo(scope);

            var dualSwordDodgeModeDocument = document.Q<HKUIDocument>("Area.DualSwordDodgeMode");
            dualSwordDodgeModeDocument.gameObject.SetActive(false);
            actor.ActionController.OnBeginDualSwordDodgeMode
                .Subscribe((actor, dualSwordDodgeModeDocument), static (x, t) =>
                {
                    var (actor, dualSwordDodgeModeDocument) = t;
                    dualSwordDodgeModeDocument.gameObject.SetActive(true);
                    var beginTime = UnityEngine.Time.time;
                    actor.UpdateAsObservable()
                        .TakeUntil(_ => UnityEngine.Time.time - beginTime >= x.duration)
                        .Subscribe(_ =>
                            {
                                dualSwordDodgeModeDocument.Q<Slider>("Slider")
                                    .value = 1.0f - (UnityEngine.Time.time - beginTime) / x.duration;
                            })
                        .RegisterTo(x.scope.Token);
                    x.scope.Token.RegisterWithoutCaptureExecutionContext(() =>
                    {
                        dualSwordDodgeModeDocument.gameObject.SetActive(false);
                    });
                })
                .RegisterTo(scope);

            var bladeEnduranceDocument = document.Q<HKUIDocument>("Area.BladeEnduranceMode");
            bladeEnduranceDocument.gameObject.SetActive(false);
            actor.ActionController.OnBeginBladeEnduranceMode
                .Subscribe((actor, bladeEnduranceDocument), static (x, t) =>
                {
                    var (actor, bladeEnduranceDocument) = t;
                    bladeEnduranceDocument.gameObject.SetActive(true);
                    var beginTime = UnityEngine.Time.time;
                    actor.UpdateAsObservable()
                        .TakeUntil(_ => UnityEngine.Time.time - beginTime >= x.duration)
                        .TakeUntil(actor.SpecController.SuperArmorCount.Where(y => y <= 0))
                        .Subscribe(_ =>
                            {
                                bladeEnduranceDocument.Q<Slider>("Slider")
                                    .value = 1.0f - (UnityEngine.Time.time - beginTime) / x.duration;
                            })
                        .RegisterTo(x.scope.Token);
                    x.scope.Token.RegisterWithoutCaptureExecutionContext(() =>
                    {
                        bladeEnduranceDocument.gameObject.SetActive(false);
                    });
                })
                .RegisterTo(scope);
        }

        protected override void OnDispose()
        {
            if (document != null)
            {
                UnityEngine.Object.Destroy(document.gameObject);
            }
        }
    }
}
