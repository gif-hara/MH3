using UnityEditor;
using UnityEngine;

namespace MH3
{
    public class DeleteSaveData
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
    }
}
