using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MH3
{
    public class GameSceneController : MonoBehaviour
    {
        private async void Start()
        {
            await UniTask.DelayFrame(1);
            Debug.Log("GameSceneController Start");
            throw new Exception("Test");
        }
    }
}
