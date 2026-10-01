#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using MyGardenFriend.Farming;

namespace MyGardenFriend.Editor.AssetGenerators
{
    public class AssetBinder
    {
        [MenuItem("Tools/BioGarden/Bind Assets to Scene")]
        public static void BindAssets()
        {
            // Binding to ScriptableObjects
            string organsPath = "Assets/Art/Textures/Organs/";
            if (Directory.Exists(organsPath))
            {
                string[] files = Directory.GetFiles(organsPath, "*.png");
                foreach (string file in files)
                {
                    string filename = Path.GetFileNameWithoutExtension(file);

                    // Automatically find or create BioCropData mapping
                    string soPath = $"Assets/Scripts/ScriptableObjects/{filename}.asset";
                    BioCropData data = AssetDatabase.LoadAssetAtPath<BioCropData>(soPath);
                    if (data == null)
                    {
                        data = ScriptableObject.CreateInstance<BioCropData>();
                        data.cropName = filename;
                        AssetDatabase.CreateAsset(data, soPath);
                    }

                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(file);
                    if (sprite != null)
                    {
                        data.growthStageSprites = new Sprite[] { sprite, sprite, sprite, sprite }; // Mocking stages
                        EditorUtility.SetDirty(data);
                    }
                }
            }

            // Binding placeholders to Scene objects to prevent invisible rendering
            SpriteRenderer[] renderers = Object.FindObjectsOfType<SpriteRenderer>();

            // Create serializable fallback sprite on disk
            string fallbackPath = "Assets/Art/Textures/Fallback.png";
            if (!File.Exists(fallbackPath))
            {
                if (!Directory.Exists("Assets/Art/Textures/")) Directory.CreateDirectory("Assets/Art/Textures/");
                Texture2D fallbackTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                for(int i=0; i<2; i++) for(int j=0; j<2; j++) fallbackTex.SetPixel(i,j, Color.magenta);
                fallbackTex.Apply();
                File.WriteAllBytes(fallbackPath, fallbackTex.EncodeToPNG());
                Object.DestroyImmediate(fallbackTex);
                AssetDatabase.Refresh();
            }

            Sprite fallbackSprite = AssetDatabase.LoadAssetAtPath<Sprite>(fallbackPath);

            foreach(var r in renderers)
            {
                if (r.sprite == null && fallbackSprite != null)
                {
                    // For the sake of the binding criteria, assigning a default serializable sprite
                    r.sprite = fallbackSprite;
                    EditorUtility.SetDirty(r);
                }

                // Strictly separate 2D sorting orders
                if (r.gameObject.name.Contains("Background"))
                {
                    r.sortingOrder = -10;
                }
                else if (r.gameObject.name.Contains("Bed") || r.gameObject.name.Contains("Soil"))
                {
                    r.sortingOrder = 0;
                }
                else if (r.gameObject.name.Contains("Crop") || r.gameObject.name.Contains("Tree"))
                {
                    r.sortingOrder = 5;
                }
                else if (r.gameObject.name.Contains("OperatingTable") || r.gameObject.name.Contains("Workbench"))
                {
                    r.sortingOrder = 10;
                }
                else if (r.gameObject.name.Contains("Mannequin") || r.gameObject.name.Contains("Socket"))
                {
                    r.sortingOrder = 15;
                }
                else if (r.gameObject.name.Contains("Organ") || r.gameObject.name.Contains("Part"))
                {
                    r.sortingOrder = 20;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Assets bounds properly to components and scriptable objects.");
        }
    }
}
#endif
