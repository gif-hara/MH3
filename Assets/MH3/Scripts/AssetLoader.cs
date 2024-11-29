using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MH3
{
    /// <summary>
    /// 
    /// </summary>
    public static class AssetLoader
    {
        public static async UniTask<T> LoadAsync<T>(string path) where T : Object
        {
            var result = await Resources.LoadAsync<T>(path).ToUniTask();
            if (result == null)
            {
                Debug.LogError($"Failed to load asset: {path}");
            }
            return result as T;
        }
    }
}