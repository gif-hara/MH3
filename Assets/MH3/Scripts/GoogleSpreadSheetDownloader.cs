using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace HK
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class GoogleSpreadSheetDownloader
    {
        const string url = "https://script.google.com/macros/s/AKfycbwqWHRGzk8_ajxPxNFvJB9u9vsUHkHBHc12VkOizHfsOOyCXuLr8OprvScKN8u6yx7ZVQ/exec";

        public static async UniTask<string> DownloadAsync(string sheetName)
        {
            var request = UnityWebRequest.Get(url + "?sheetName=" + sheetName);
            request.timeout = 20;
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                // エラー処理
                UnityEngine.Debug.LogError(request.error);
                return null;
            }
            else
            {
                return request.downloadHandler.text;
            }
        }
    }
}