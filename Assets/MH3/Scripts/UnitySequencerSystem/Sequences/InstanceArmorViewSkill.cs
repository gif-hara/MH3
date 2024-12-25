using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class InstanceArmorViewSkill : Sequence
    {
        [SerializeField]
        private string instanceArmorKey;

        [SerializeField]
        private RectTransform parent;

        [SerializeField]
        private HKUIDocument skillDocumentPrefab;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var instanceArmor = container.Resolve<InstanceArmor>(instanceArmorKey);
            for (int i = 0; i < parent.childCount; i++)
            {
                UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
            }
            var skills = instanceArmor.Skills.GroupBy(x => x.SkillType);
            foreach (var skill in skills.OrderBy(x => x.Key))
            {
                var document = UnityEngine.Object.Instantiate(skillDocumentPrefab, parent);
                document.Q<TMP_Text>("Header").text = skill.Key.GetName();
                document.Q<TMP_Text>("Value").text = $"Lv.{skill.Sum(x => x.Level)}";
            }
            return UniTask.CompletedTask;
        }
    }
}
