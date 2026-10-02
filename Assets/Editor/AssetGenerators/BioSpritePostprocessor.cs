#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyGardenFriend.Editor.AssetGenerators
{
    public class BioSpritePostprocessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (assetPath.StartsWith("Assets/Art/Textures/") || assetPath.StartsWith("Assets/Resources/"))
            {
                TextureImporter textureImporter = (TextureImporter)assetImporter;

                // Configure base settings
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.alphaIsTransparency = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.filterMode = FilterMode.Bilinear; // Or Point for sharper edges

                // Configure pivots based on the folder path
                TextureImporterSettings settings = new TextureImporterSettings();
                textureImporter.ReadTextureSettings(settings);

                if (assetPath.Contains("/Mannequin/"))
                {
                    settings.spriteAlignment = (int)SpriteAlignment.TopCenter;
                    textureImporter.SetTextureSettings(settings);
                }
                else if (assetPath.Contains("/Organs/") || assetPath.Contains("/Trees/"))
                {
                    settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                    textureImporter.SetTextureSettings(settings);
                }
            }
        }
    }
}
#endif
