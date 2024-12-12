using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using HK;
using R3;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    public class UIViewAcquireReward
    {
        public static async UniTask OpenAsync(
            HKUIDocument documentPrefab,
            IEnumerable<InstanceWeapon> instanceWeapons,
            IEnumerable<InstanceSkillCore> instanceSkillCores,
            CancellationToken scope
            )
        {
            if (instanceWeapons.Any() == false && instanceSkillCores.Any() == false)
            {
                return;
            }
            var document = Object.Instantiate(documentPrefab);
            var header = document.Q<TMP_Text>("Header");
            var areaInstanceWeapon = document.Q<HKUIDocument>("Area.InstanceWeapon");
            var instanceWeaponSequences = areaInstanceWeapon.Q<SequencesMonoBehaviour>("Sequences");
            var areaInstanceSkillCore = document.Q<HKUIDocument>("Area.InstanceSkillCore");
            var instanceSkillCoreSequences = areaInstanceSkillCore.Q<SequencesMonoBehaviour>("Sequences");
            var inputController = TinyServiceLocator.Resolve<InputController>();
            inputController.PushActionType(InputController.InputActionType.UI);

            areaInstanceWeapon.gameObject.SetActive(true);
            areaInstanceSkillCore.gameObject.SetActive(false);
            foreach (var instanceWeapon in instanceWeapons)
            {
                var container = new Container();
                header.text = "武器獲得！";
                container.Register("InstanceWeapon", instanceWeapon);
                instanceWeaponSequences.PlayAsync(container, scope).Forget();
                await inputController.Actions.UI.Submit.OnPerformedAsObservable().FirstAsync(scope).AsUniTask();
            }

            areaInstanceWeapon.gameObject.SetActive(false);
            areaInstanceSkillCore.gameObject.SetActive(true);
            foreach (var instanceSkillCore in instanceSkillCores)
            {
                var container = new Container();
                header.text = "スキルコア獲得！";
                container.Register("InstanceSkillCore", instanceSkillCore);
                instanceSkillCoreSequences.PlayAsync(container, scope).Forget();
                await inputController.Actions.UI.Submit.OnPerformedAsObservable().FirstAsync(scope).AsUniTask();
            }

            inputController.PopActionType();
            document.DestroySafe();
        }
    }
}
