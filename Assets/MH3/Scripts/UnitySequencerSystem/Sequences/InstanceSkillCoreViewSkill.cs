using System;
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
    public class InstanceSkillCoreViewSkill : Sequence
    {
        [SerializeField]
        private string instanceSkillCoreKey;

        [SerializeField]
        private RectTransform parent;

        [SerializeField]
        private HKUIDocument skillDocumentPrefab;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var instanceSkillCore = container.Resolve<InstanceSkillCore>(instanceSkillCoreKey);
            foreach (var skill in instanceSkillCore.Skills)
            {
                var document = UnityEngine.Object.Instantiate(skillDocumentPrefab, parent);
                document.Q<TMP_Text>("Header").text = skill.SkillType.GetName();
                document.Q<TMP_Text>("Value").text = $"Lv.{skill.Level}";
            }
            return UniTask.CompletedTask;
        }
    }
}
