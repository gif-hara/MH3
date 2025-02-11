using HK;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MH3.Editor
{
    public class MH3MenuItem
    {
        [MenuItem("MH3/Delete Save Data")]
        public static void Delete()
        {
            var path = Application.persistentDataPath + "/" + SaveData.Path;
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        [MenuItem("MH3/Enumerate Input Control")]
        public static void EnumerateInputControl()
        {
            foreach (var i in InputSystem.devices)
            {
                foreach (var j in i.children)
                {
                    Debug.Log(InputSprite.GetSpriteName(j));
                }
            }
        }
    }
}
