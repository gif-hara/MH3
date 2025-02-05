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
            var staminaSlider = document.Q<HKUIDocument>("Slider.Stamina").Q<Slider>("Slider");
            var staminaSliderDefaultSize = ((RectTransform)staminaSlider.transform).sizeDelta;
            var gameEvents = TinyServiceLocator.Resolve<GameEvents>();
            gameEvents.OnBeginTitle
                .Subscribe(document, static (x, t) =>
                {
                    var document = t;
                    document.gameObject.SetActive(false);
                })
                .RegisterTo(scope);
            gameEvents.OnEndTitle
                .Subscribe(document, static (x, t) =>
                {
                    var document = t;
                    document.gameObject.SetActive(true);
                    document.Q<SimpleAnimation>("Area.Animation").Play("In");
                })
                .RegisterTo(scope);
            gameEvents.OnBeginPauseMenu
                .Subscribe(document, static (x, t) =>
                {
                    var document = t;
                    document.gameObject.SetActive(false);
                })
                .RegisterTo(scope);
            gameEvents.OnEndPauseMenu
                .Subscribe(document, static (x, t) =>
                {
                    var document = t;
                    document.gameObject.SetActive(true);
                })
                .RegisterTo(scope);
            gameEvents.OnBeginAcquireReward
                .Subscribe(document, static (x, t) =>
                {
                    var document = t;
                    document.gameObject.SetActive(false);
                })
                .RegisterTo(scope);
            gameEvents.OnEndAcquireReward
                .Subscribe(document, static (x, t) =>
                {
                    var document = t;
                    document.gameObject.SetActive(true);
                })
                .RegisterTo(scope);
            ((RectTransform)hitPointSlider.transform).sizeDelta = new Vector2(
                gameRules.HitPointSliderAddWidth * actor.SpecController.HitPointMaxTotal,
                hitPointSliderDefaultSize.y
                );
            ((RectTransform)staminaSlider.transform).sizeDelta = new Vector2(
                gameRules.StaminaSliderAddWidth * actor.SpecController.StaminaMaxTotal,
                staminaSliderDefaultSize.y
                );
            actor.SpecController.HitPoint
                .Subscribe((hitPointSlider, actor), static (_, t) =>
                {
                    var (hitPointSlider, actor) = t;
                    hitPointSlider.value = (float)actor.SpecController.HitPoint.CurrentValue / actor.SpecController.HitPointMaxTotal;
                })
                .RegisterTo(scope);
            actor.SpecController.Stamina
                .Subscribe((staminaSlider, actor), static (_, t) =>
                {
                    var (staminaSlider, actor) = t;
                    staminaSlider.value = (float)actor.SpecController.Stamina.CurrentValue / actor.SpecController.StaminaMaxTotal;
                })
                .RegisterTo(scope);
            actor.SpecController.OnBuildStatuses
                .Subscribe((hitPointSlider, hitPointSliderDefaultSize, staminaSlider, staminaSliderDefaultSize, gameRules, actor), static (_, t) =>
                {
                    var (hitPointSlider, hitPointSliderDefaultSize, staminaSlider, staminaSliderDefaultSize, gameRules, actor) = t;
                    ((RectTransform)hitPointSlider.transform).sizeDelta = new Vector2(
                        gameRules.HitPointSliderAddWidth * actor.SpecController.HitPointMaxTotal,
                        hitPointSliderDefaultSize.y
                        );
                    ((RectTransform)staminaSlider.transform).sizeDelta = new Vector2(
                        gameRules.StaminaSliderAddWidth * actor.SpecController.StaminaMaxTotal,
                        staminaSliderDefaultSize.y
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

            var spearDodgeGaugeDocument = document.Q<HKUIDocument>("Area.SpearDodgeGauge");
            spearDodgeGaugeDocument.gameObject.SetActive(actor.SpecController.WeaponSpec.WeaponType == Define.WeaponType.Spear);
            actor.SpecController.OnBuildStatuses
                .Subscribe((actor, spearDodgeGaugeDocument), static (_, t) =>
                {
                    var (actor, spearDodgeGaugeDocument) = t;
                    spearDodgeGaugeDocument.gameObject.SetActive(actor.SpecController.WeaponSpec.WeaponType == Define.WeaponType.Spear);
                })
                .RegisterTo(scope);
            actor.SpecController.SpearDodgeGauge
                .Subscribe((spearDodgeGaugeDocument, actor), static (_, t) =>
                {
                    var (spearDodgeGaugeDocument, actor) = t;
                    var gameRules = TinyServiceLocator.Resolve<GameRules>();
                    spearDodgeGaugeDocument.Q<Slider>("Slider")
                        .value = (float)actor.SpecController.SpearDodgeGauge.CurrentValue / gameRules.SpearDodgeGaugeMax;
                })
                .RegisterTo(scope);
            actor.SpecController.SpearComboLevel
                .Subscribe((spearDodgeGaugeDocument, actor), static (x, t) =>
                {
                    var (spearDodgeGaugeDocument, actor) = t;
                    spearDodgeGaugeDocument.Q<TMP_Text>("Text.ComboLevel")
                        .text = string.Format("Lv.{0}".Localized(), x);
                })
                .RegisterTo(scope);
            var spearComboGaugeThresholdPrefab = spearDodgeGaugeDocument.Q("Prefab.Threshold");
            var spearComboGaugeThresholdParent = spearDodgeGaugeDocument.Q<Transform>("Area.Threshold");
            foreach (var threshold in gameRules.SpearComboLevelThresholds)
            {
                var thresholdObject = Object.Instantiate(spearComboGaugeThresholdPrefab, spearComboGaugeThresholdParent);
                var t = (RectTransform)thresholdObject.transform;
                var rate = threshold / gameRules.SpearDodgeGaugeMax;
                t.anchorMax = new Vector2(rate, 1.0f);
                t.anchorMin = new Vector2(rate, 0.0f);
                t.anchoredPosition = Vector2.zero;
            }
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
