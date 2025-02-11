using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.TextCore;
using TMPro;
using System.Linq;
using UnityEditor.U2D;
using UnityEditor.U2D.Sprites;

[CreateAssetMenu(fileName = "TMPSpriteAssetGenerator", menuName = "ScriptableObjects/TMPSpriteAssetGenerator",
    order = 1)]
public class TMPSpriteAssetGenerator : ScriptableObject
{
#if UNITY_EDITOR
    public string spritesDirectory = "Assets/Sprites";
    public string outputDirectory = "Assets/Sprites";
    public bool normalizeSpriteSizes = true;
    public int spriteResolution = 128;
    public bool overwriteExistingAssets = true;
    public string spriteSheetName = "TMPSpriteSheet";

    public void CreateTMPSpriteAsset()
    {
        if (!Directory.Exists(spritesDirectory))
        {
            Debug.LogError("Sprites directory does not exist: " + spritesDirectory);
            return;
        }

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Check for existing assets
        string spriteSheetPath = Path.Combine(outputDirectory, spriteSheetName + ".png");
        string spriteAssetPath = Path.Combine(outputDirectory, spriteSheetName + "_SpriteAsset.asset");
        string materialPath = Path.Combine(outputDirectory, spriteSheetName + "_Material.mat");

        if (!overwriteExistingAssets)
        {
            if (File.Exists(spriteSheetPath) || File.Exists(spriteAssetPath) || File.Exists(materialPath))
            {
                Debug.LogWarning(
                    "Sprite sheet, material, or TMP Sprite Asset already exists. Set 'Overwrite Existing Assets' to true to overwrite.");
                return;
            }
        }

        // Step 1: Get all sprites at the given directory
        string[] spriteFiles = Directory.GetFiles(spritesDirectory, "*.png", SearchOption.TopDirectoryOnly);

        if (spriteFiles.Length == 0)
        {
            Debug.LogError("No sprites found in the specified directory: " + spritesDirectory);
            return;
        }

        List<Texture2D> spriteTextures = new List<Texture2D>();
        List<string> spriteNames = new List<string>();

        foreach (string filePath in spriteFiles)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            Texture2D processedTexture = texture;

            if (normalizeSpriteSizes)
            {
                // Step 2: Resize the sprite to the specified resolution
                processedTexture = ResizeTexture(texture, spriteResolution, spriteResolution);
            }

            spriteTextures.Add(processedTexture);

            string spriteName = Path.GetFileNameWithoutExtension(filePath);
            spriteNames.Add(spriteName);
        }

        // Step 3: Combine the sprites into a sprite sheet
        Texture2D spriteSheet = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        Rect[] rects = spriteSheet.PackTextures(spriteTextures.ToArray(), 2, 4096, false);

        // Save the sprite sheet as an asset
        byte[] pngData = spriteSheet.EncodeToPNG();
        File.WriteAllBytes(spriteSheetPath, pngData);
        AssetDatabase.ImportAsset(spriteSheetPath);
        AssetDatabase.Refresh();

        // Load the imported sprite sheet
        Texture2D importedSpriteSheet = AssetDatabase.LoadAssetAtPath<Texture2D>(spriteSheetPath);
        if (importedSpriteSheet == null)
        {
            Debug.LogError("Failed to load the sprite sheet.");
            return;
        }

        // Slice the sprite sheet using SerializedObject
        string spriteSheetAssetPath = AssetDatabase.GetAssetPath(importedSpriteSheet);
        TextureImporter ti = AssetImporter.GetAtPath(spriteSheetAssetPath) as TextureImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Multiple;

        // Use SerializedObject to set sprite metadata
        SerializedObject so = new SerializedObject(ti);
        SerializedProperty spritesSP = so.FindProperty("m_SpriteSheet.m_Sprites");

        spritesSP.arraySize = rects.Length;
        for (int i = 0; i < rects.Length; i++)
        {
            SerializedProperty spriteSP = spritesSP.GetArrayElementAtIndex(i);

            spriteSP.FindPropertyRelative("m_Name").stringValue = spriteNames[i];
            spriteSP.FindPropertyRelative("m_Rect").rectValue = new Rect(
                rects[i].x * spriteSheet.width,
                rects[i].y * spriteSheet.height,
                rects[i].width * spriteSheet.width,
                rects[i].height * spriteSheet.height
            );
            spriteSP.FindPropertyRelative("m_Alignment").intValue = (int)SpriteAlignment.Center;
            spriteSP.FindPropertyRelative("m_Pivot").vector2Value = new Vector2(0.5f, 0.5f);
            spriteSP.FindPropertyRelative("m_Border").vector4Value = Vector4.zero;
        }

        so.ApplyModifiedProperties();
        AssetDatabase.ImportAsset(spriteSheetAssetPath, ImportAssetOptions.ForceUpdate);

        // Step 5: Create TMP Sprite Asset
        TMP_SpriteAsset spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
        spriteAsset.name = spriteSheetName + "_SpriteAsset";

        // Assign the sprite sheet
        spriteAsset.spriteSheet = importedSpriteSheet;

        // Populate the sprite info
        List<TMP_SpriteCharacter> spriteCharacterTable = new List<TMP_SpriteCharacter>();
        List<TMP_SpriteGlyph> spriteGlyphTable = new List<TMP_SpriteGlyph>();
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetAssetPath).OfType<Sprite>().ToArray();

        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite sprite = sprites[i];

            // Create Glyph
            TMP_SpriteGlyph spriteGlyph = new TMP_SpriteGlyph();
            spriteGlyph.index = (uint)i;
            spriteGlyph.metrics = new GlyphMetrics(
                sprite.rect.width,
                sprite.rect.height,
                -sprite.pivot.x * sprite.rect.width,
                -sprite.pivot.y * sprite.rect.height,
                sprite.rect.width
            );
            spriteGlyph.glyphRect = new GlyphRect(
                (int)sprite.rect.x,
                (int)sprite.rect.y,
                (int)sprite.rect.width,
                (int)sprite.rect.height
            );
            spriteGlyph.scale = 1.0f;
            spriteGlyph.sprite = sprite;

            spriteGlyphTable.Add(spriteGlyph);

            // Create Character
            TMP_SpriteCharacter spriteChar = new TMP_SpriteCharacter((uint)(0xE000 + i), spriteGlyph);
            spriteChar.name = sprite.name;
            spriteChar.scale = 1.0f;

            spriteCharacterTable.Add(spriteChar);
        }

        // Populate the sprite info by adding to the existing tables
        spriteAsset.spriteCharacterTable.Clear();
        spriteAsset.spriteCharacterTable.AddRange(spriteCharacterTable);

        spriteAsset.spriteGlyphTable.Clear();
        spriteAsset.spriteGlyphTable.AddRange(spriteGlyphTable);

        // Update the lookup tables
        spriteAsset.UpdateLookupTables();

        // Step 6: Create a new material using the "TextMeshPro/Sprite" shader as the final step
        Material spriteMaterial = new Material(Shader.Find("TextMeshPro/Sprite"));
        spriteMaterial.name = spriteSheetName + "_Material";
        spriteMaterial.mainTexture = importedSpriteSheet;

        // Save the material as an asset
        AssetDatabase.CreateAsset(spriteMaterial, materialPath);

        // Assign the material to the sprite asset
        spriteAsset.material = spriteMaterial;

        // Save the TMP Sprite Asset
        AssetDatabase.CreateAsset(spriteAsset, spriteAssetPath);
        AssetDatabase.SaveAssets();

        Debug.Log("TMP Sprite Asset created successfully at: " + spriteAssetPath);
    }

    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Bilinear;
        RenderTexture.active = rt;

        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(newWidth, newHeight, source.format, false);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }
#endif
}