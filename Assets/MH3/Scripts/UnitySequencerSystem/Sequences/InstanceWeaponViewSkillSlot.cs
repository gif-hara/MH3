using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class InstanceWeaponViewSkillSlot : Sequence
    {
        [SerializeField]
        private string instanceWeaponDataKey;

        [SerializeField]
        private RectTransform parent;

        [SerializeField]
        private HKUIDocument skillSlotDocumentPrefab;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var instanceWeaponData = container.Resolve<InstanceWeaponData>(instanceWeaponDataKey);
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                UnityEngine.Object.Destroy(child.gameObject);
            }
            for (var i = 0; i < instanceWeaponData.SkillSlot; i++)
            {
                UnityEngine.Object.Instantiate(skillSlotDocumentPrefab, parent);
            }
            return UniTask.CompletedTask;
        }
    }
}
