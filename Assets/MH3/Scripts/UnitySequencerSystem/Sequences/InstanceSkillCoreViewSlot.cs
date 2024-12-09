using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnityEngine.UI;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class InstanceSkillCoreViewSlot : Sequence
    {
        [SerializeField]
        private string instanceSkillCoreKey;

        [SerializeField]
        private RectTransform parent;

        [SerializeField]
        private HKUIDocument skillSlotDocumentPrefab;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var instanceSkillCore = container.Resolve<InstanceSkillCore>(instanceSkillCoreKey);
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                UnityEngine.Object.Destroy(child.gameObject);
            }
            for (var i = 0; i < instanceSkillCore.Slot; i++)
            {
                var skillSlotDocument = UnityEngine.Object.Instantiate(skillSlotDocumentPrefab, parent);
                skillSlotDocument.Q<Image>("Image").color = instanceSkillCore.SkillCoreSpec.Color;
            }
            return UniTask.CompletedTask;
        }
    }
}
