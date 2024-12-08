using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
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
                UnityEngine.Object.Instantiate(skillSlotDocumentPrefab, parent);
            }
            return UniTask.CompletedTask;
        }
    }
}
