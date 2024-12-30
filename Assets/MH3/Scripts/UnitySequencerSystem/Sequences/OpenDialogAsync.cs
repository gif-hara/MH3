using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using UnityEngine;
using UnitySequencerSystem;
using UnitySequencerSystem.Resolvers;

namespace MH3
{
    [Serializable]
    public class OpenDialogAsync : Sequence
    {
        [SerializeField]
        private HKUIDocument documentPrefab;

        [SerializeReference, SubclassSelector]
        private StringResolver messageResolver;

        [SerializeReference, SubclassSelector]
        private List<StringResolver> optionsResolvers;

        [SerializeReference, SubclassSelector]
        private IntResolver initialSelectionIndexResolver;

        [SerializeReference, SubclassSelector]
        private IntResolver cancelIndexResolver;

        [SerializeReference, SubclassSelector]
        private StringResolver resultResolver;

        public override async UniTask PlayAsync(Container container, CancellationToken cancellationToken)
        {
            var dialog = new UIViewSimpleDialog(documentPrefab);
            var result = await dialog.OpenAsync(
                messageResolver.Resolve(container),
                optionsResolvers.Select(r => r.Resolve(container)),
                initialSelectionIndexResolver.Resolve(container),
                cancelIndexResolver.Resolve(container),
                cancellationToken
                );
            container.RegisterOrReplace(resultResolver.Resolve(container), result);
        }
    }
}
