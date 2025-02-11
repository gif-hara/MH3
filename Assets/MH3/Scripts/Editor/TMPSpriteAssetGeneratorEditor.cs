using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TMPSpriteAssetGenerator))]
public class TMPSpriteAssetGeneratorEditor : Editor
{
    SerializedProperty spritesDirectoryProp;
    SerializedProperty outputDirectoryProp;
    SerializedProperty normalizeSpriteSizesProp;
    SerializedProperty spriteResolutionProp;
    SerializedProperty overwriteExistingAssetsProp;
    SerializedProperty spriteSheetNameProp;

    private void OnEnable()
    {
        spritesDirectoryProp = serializedObject.FindProperty("spritesDirectory");
        outputDirectoryProp = serializedObject.FindProperty("outputDirectory");
        normalizeSpriteSizesProp = serializedObject.FindProperty("normalizeSpriteSizes");
        spriteResolutionProp = serializedObject.FindProperty("spriteResolution");
        overwriteExistingAssetsProp = serializedObject.FindProperty("overwriteExistingAssets");
        spriteSheetNameProp = serializedObject.FindProperty("spriteSheetName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("TMP Sprite Asset Generator", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(spritesDirectoryProp, new GUIContent("Sprites Directory"));
        EditorGUILayout.PropertyField(outputDirectoryProp, new GUIContent("Output Directory"));
        EditorGUILayout.PropertyField(overwriteExistingAssetsProp, new GUIContent("Overwrite Existing Assets"));
        EditorGUILayout.PropertyField(normalizeSpriteSizesProp, new GUIContent("Normalize Sprite Sizes"));

        EditorGUI.BeginDisabledGroup(!normalizeSpriteSizesProp.boolValue);
        EditorGUILayout.PropertyField(spriteResolutionProp, new GUIContent("Normalized Sprite Size"));
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(spriteSheetNameProp, new GUIContent("Sprite Sheet Name"));

        EditorGUILayout.Space();

        if (GUILayout.Button("Create TMP Sprite Asset", GUILayout.Height(40)))
        {
            ((TMPSpriteAssetGenerator)target).CreateTMPSpriteAsset();
        }

        serializedObject.ApplyModifiedProperties();
    }
}