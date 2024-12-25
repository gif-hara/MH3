using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using MH3.UnitySequencerSystem.Resolvers;
using TMPro;
using UnityEngine;
using UnitySequencerSystem;

namespace MH3
{
    [Serializable]
    public class ActorSpecViewSkill : Sequence
    {
        [SerializeReference, SubclassSelector]
        private ActorResolver actorResolver;

        [SerializeField]
        private Transform parent;

        [SerializeField]
        private HKUIDocument skillDocumentPrefab;

        public override UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var actor = actorResolver.Resolve(container);
            for (int i = 0; i < parent.childCount; i++)
            {
                UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
            }
            foreach (var skill in actor.SpecController.Skills)
            {
                var document = UnityEngine.Object.Instantiate(skillDocumentPrefab, parent);
                document.Q<TMP_Text>("Header").text = skill.SkillType.GetName();
                document.Q<TMP_Text>("Value").text = $"Lv.{skill.Level}";
            }
            return UniTask.CompletedTask;
        }
    }
}
