using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class InstanceWeaponViewSkill : Sequence
    {
        [SerializeField]
        private string instanceWeaponKey;

        [SerializeField]
        private RectTransform parent;

        [SerializeField]
        private HKUIDocument labelDocumentPrefab;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var instanceWeapon = container.Resolve<InstanceWeapon>(instanceWeaponKey);
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                UnityEngine.Object.Destroy(child.gameObject);
            }
            var userData = TinyServiceLocator.Resolve<UserData>();
            var skills = instanceWeapon.InstanceSkillCoreIds
                .Select(x => userData.InstanceSkillCores.Find(y => y.InstanceId == x))
                .SelectMany(x => x.Skills)
                .GroupBy(x => x.SkillType);
            foreach (var i in skills.OrderBy(x => x.Key))
            {
                var label = UnityEngine.Object.Instantiate(labelDocumentPrefab, parent);
                label.Q<TMP_Text>("Header").text = i.Key.GetName();
                label.Q<TMP_Text>("Value").text = $"Lv.{i.Sum(x => x.Level)}";
            }
            return UniTask.CompletedTask;
        }
    }
}
