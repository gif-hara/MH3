using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MH3
{
    public class UIViewOptionsSound : IUIViewOptions
    {
        private HKUIDocument document;

        private Element masterVolume;

        private Element bgmVolume;

        private Element sfxVolume;

        public UIViewOptionsSound(HKUIDocument documentPrefab)
        {
            document = UnityEngine.Object.Instantiate(documentPrefab);
            masterVolume = new Element(document, "MasterVolume");
            bgmVolume = new Element(document, "BgmVolume");
            sfxVolume = new Element(document, "SfxVolume");
            var saveData = TinyServiceLocator.Resolve<SaveData>();
            masterVolume.slider.value = saveData.SystemData.MasterVolume;
            bgmVolume.slider.value = saveData.SystemData.BgmVolume;
            sfxVolume.slider.value = saveData.SystemData.SfxVolume;
        }

        public void Dispose()
        {
            document.DestroySafe();
        }

        public async UniTask ActivateAsync(CancellationToken scope)
        {
            var saveData = TinyServiceLocator.Resolve<SaveData>();
            var audioManager = TinyServiceLocator.Resolve<AudioManager>();
            var inputController = TinyServiceLocator.Resolve<InputController>();
            var selectables = new[]
            {
                masterVolume.selectable,
                bgmVolume.selectable,
                sfxVolume.selectable
            };
            selectables.SetNavigationVertical();
            masterVolume.slider.value = saveData.SystemData.MasterVolume;
            bgmVolume.slider.value = saveData.SystemData.BgmVolume;
            sfxVolume.slider.value = saveData.SystemData.SfxVolume;
            masterVolume.selectable.OnSelectAsObservable()
                .Subscribe(_ => UIViewTips.SetTip("マスター音量を変更します。".Localized()))
                .RegisterTo(scope);
            bgmVolume.selectable.OnSelectAsObservable()
                .Subscribe(_ => UIViewTips.SetTip("BGMの音量を変更します。".Localized()))
                .RegisterTo(scope);
            sfxVolume.selectable.OnSelectAsObservable()
                .Subscribe(_ => UIViewTips.SetTip("効果音の音量を変更します。".Localized()))
                .RegisterTo(scope);
            inputController.Actions.UI.Navigate.OnPerformedAsObservable()
                .Subscribe(context =>
                {
                    var value = context.ReadValue<Vector2>();
                    if (value.x == 0)
                    {
                        return;
                    }
                    var addValue = value.x > 0 ? 0.1f : -0.1f;
                    switch (EventSystem.current.currentSelectedGameObject)
                    {
                        case var x when x == masterVolume.selectable.gameObject:
                            saveData.SystemData.MasterVolume = Mathf.Clamp(saveData.SystemData.MasterVolume + addValue, 0, 1);
                            masterVolume.slider.value = saveData.SystemData.MasterVolume;
                            audioManager.SetVolumeMaster(saveData.SystemData.MasterVolume);
                            SaveSystem.Save(saveData, SaveData.Path);
                            break;
                        case var x when x == bgmVolume.selectable.gameObject:
                            saveData.SystemData.BgmVolume = Mathf.Clamp(saveData.SystemData.BgmVolume + addValue, 0, 1);
                            bgmVolume.slider.value = saveData.SystemData.BgmVolume;
                            audioManager.SetVolumeBgm(saveData.SystemData.BgmVolume);
                            SaveSystem.Save(saveData, SaveData.Path);
                            break;
                        case var x when x == sfxVolume.selectable.gameObject:
                            saveData.SystemData.SfxVolume = Mathf.Clamp(saveData.SystemData.SfxVolume + addValue, 0, 1);
                            sfxVolume.slider.value = saveData.SystemData.SfxVolume;
                            audioManager.SetVolumeSfx(saveData.SystemData.SfxVolume);
                            SaveSystem.Save(saveData, SaveData.Path);
                            break;
                    }
                })
                .RegisterTo(scope);
            masterVolume.selectable.Select();
            await inputController.Actions.UI.Cancel
                    .OnPerformedAsObservable()
                    .FirstAsync();
            foreach (var selectable in selectables)
            {
                selectable.navigation = new Navigation
                {
                    mode = Navigation.Mode.None
                };
            }
        }

        public class Element
        {
            public readonly HKUIDocument rootDocument;

            public readonly Selectable selectable;

            public readonly Slider slider;

            public Element(HKUIDocument document, string rootName)
            {
                rootDocument = document.Q<HKUIDocument>(rootName);
                selectable = rootDocument.Q<HKUIDocument>("Area.Button").Q<Selectable>("Button");
                slider = rootDocument.Q<HKUIDocument>("Area.Slider").Q<HKUIDocument>("Element.Slider").Q<Slider>("Slider");
            }
        }
    }
}
