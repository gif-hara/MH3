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
            var instanceWeaponData = container.Resolve<InstanceWeapon>(instanceWeaponDataKey);
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                UnityEngine.Object.Destroy(child.gameObject);
            }
            var userData = TinyServiceLocator.Resolve<UserData>();
            var slotCount = 0;
            foreach (var i in instanceWeaponData.InstanceSkillCoreIds)
            {
                var instanceSkillCore = userData.InstanceSkillCores.Find(x => x.InstanceId == i);
                for (var j = 0; j < instanceSkillCore.Slot; j++)
                {
                    var skillSlotDocument = UnityEngine.Object.Instantiate(skillSlotDocumentPrefab, parent);
                    skillSlotDocument.Q<Image>("Image").color = instanceSkillCore.SkillCoreSpec.Color;
                }
                slotCount += instanceSkillCore.Slot;
            }
            for (var i = slotCount; i < instanceWeaponData.SkillSlot; i++)
            {
                UnityEngine.Object.Instantiate(skillSlotDocumentPrefab, parent);
            }
            return UniTask.CompletedTask;
        }
    }
}
