using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HK
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class GoogleSpreadSheetDownloader
    {
        const string url = "https://script.google.com/macros/s/AKfycbzQ2KSHriUxxwL1XeQW2PFVYid-YgUu4i-lGMcHCPPbM2C6FNB27ksyxE9Mg6TAoCaBTg/exec";

        public static async UniTask<string> DownloadAsync(string sheetName)
        {
            var request = UnityWebRequest.Get(url + "?sheetName=" + sheetName);
            request.timeout = 60;
            try
            {
                await request.SendWebRequest();
                return request.downloadHandler.text;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"sheetName: {sheetName}{System.Environment.NewLine}{e.Message}");
                return null;
            }
        }
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MH3/Test")]
        public static async void Test()
        {
            var text = await DownloadAsync("Skill.AttackUp");
            Debug.Log(text);
        }
#endif
    }
}