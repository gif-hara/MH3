using System.Linq;
using MH3.ActorControllers;
using UnityEditor;
using UnityEngine;

namespace MH3.Editor
{
    [CustomEditor(typeof(EditAnimationUtility))]
    public class EditAnimationUtilityEditor : UnityEditor.Editor
    {
        public void OnEnable()
        {
            EditAnimationUtility editAnimationUtility = (EditAnimationUtility)target;
            editAnimationUtility.weaponModelData = AssetDatabase.FindAssets("t:WeaponModelData")
                .Select(guid => AssetDatabase.LoadAssetAtPath<WeaponModelData>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();
        }

        public override void OnInspectorGUI()
        {
            DrawLocatorHolder();
            DrawWeaponModelData();
        }

        private void DrawLocatorHolder()
        {
            EditAnimationUtility editAnimationUtility = (EditAnimationUtility)target;
            editAnimationUtility.locatorHolder = (LocatorHolder)EditorGUILayout.ObjectField("Locator Holder", editAnimationUtility.locatorHolder, typeof(LocatorHolder), true);
        }

        private void DrawWeaponModelData()
        {
            EditAnimationUtility editAnimationUtility = (EditAnimationUtility)target;
            foreach (var i in editAnimationUtility.weaponModelData)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(i, typeof(WeaponModelData), false);
                if (GUILayout.Button("Change"))
                {
                    foreach (var weapon in editAnimationUtility.locatorHolder.GetComponentsInChildren<Weapon>())
                    {
                        DestroyImmediate(weapon.gameObject);
                    }
                    foreach (var weaponModel in i.Elements)
                    {
                        Instantiate(weaponModel.ModelPrefab, editAnimationUtility.locatorHolder.Get(weaponModel.LocatorName));
                    }
                    EditorUtility.SetDirty(editAnimationUtility.locatorHolder);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
