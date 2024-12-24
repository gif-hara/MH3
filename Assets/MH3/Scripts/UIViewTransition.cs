using System.Threading;
using Cysharp.Threading.Tasks;
using HK;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace MH3
{
    public class UIViewTransition
    {
        private readonly HKUIDocument document;

        private readonly RawImage image;

        private Material material;

        public UIViewTransition(HKUIDocument documentPrefab, CancellationToken scope)
        {
            document = Object.Instantiate(documentPrefab);
            image = document.Q<RawImage>("Image");
            image.enabled = false;
            scope.RegisterWithoutCaptureExecutionContext(() =>
            {
                document.DestroySafe();
            });
        }

        public UIViewTransition Build()
        {
            image.enabled = true;
            return this;
        }

        public UIViewTransition SetMaterial(string materialKey)
        {
            if (material != null)
            {
                Object.Destroy(material);
            }
            material = Object.Instantiate(TinyServiceLocator.Resolve<MaterialManager>().Get(materialKey));
            image.material = material;
            return this;
        }

        public UniTask BeginAsync(MotionBuilder<float, NoOptions, LitMotion.Adapters.FloatMotionAdapter> motionBuilder)
        {
            return motionBuilder
                .BindToMaterialFloat(image.material, "_Progress")
                .ToUniTask();
        }
    }
}
