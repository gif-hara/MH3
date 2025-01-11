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
            DrawActor();
            DrawWeaponModelData();
        }

        private void DrawActor()
        {
            EditAnimationUtility editAnimationUtility = (EditAnimationUtility)target;
            editAnimationUtility.actor = (Actor)EditorGUILayout.ObjectField("Actor", editAnimationUtility.actor, typeof(Actor), true);
        }

        private void DrawWeaponModelData()
        {
            EditAnimationUtility editAnimationUtility = (EditAnimationUtility)target;
            for (int i = 0; i < editAnimationUtility.weaponModelData.Count; i++)
            {
                var horizontalLayout = EditorGUILayout.BeginHorizontal();
                WeaponModelData weaponModelData = editAnimationUtility.weaponModelData[i];
                EditorGUILayout.LabelField(weaponModelData.name);
                if (GUILayout.Button("Edit"))
                {
                    Debug.Log("Edit " + weaponModelData.name);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
