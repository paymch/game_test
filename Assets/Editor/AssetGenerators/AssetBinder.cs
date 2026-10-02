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

            // Procedurally generate fallback sprites matching the objective description
            Sprite backgroundSprite = GenerateRuntimeSprite("LabBackground", 2, 2, new Color(0.125f, 0.125f, 0.145f));
            Sprite soilSprite = GenerateRuntimeSprite("SoilBed", 64, 64, new Color(0.3f, 0.2f, 0.1f));
            Sprite bloodVatSprite = GenerateRuntimeSprite("BloodVat", 64, 64, new Color(0.6f, 0.0f, 0.0f));
            Sprite sweatVatSprite = GenerateRuntimeSprite("SweatVat", 64, 64, new Color(0.0f, 0.9f, 1.0f));
            Sprite urineVatSprite = GenerateRuntimeSprite("UrineVat", 64, 64, new Color(0.9f, 0.75f, 0.0f));
            Sprite tableSprite = GenerateRuntimeSprite("OperatingTable", 128, 64, new Color(0.16f, 0.16f, 0.18f));
            Sprite genericPartSprite = GenerateRuntimeSprite("GenericPart", 32, 32, Color.gray);

            // Binding placeholders to Scene objects to prevent invisible rendering
            SpriteRenderer[] renderers = Object.FindObjectsOfType<SpriteRenderer>();

            foreach(var r in renderers)
            {
                if (r.sprite == null)
                {
                    if (r.gameObject.name.Contains("Background")) r.sprite = backgroundSprite;
                    else if (r.gameObject.name.Contains("Bed")) r.sprite = soilSprite;
                    else if (r.gameObject.name.Contains("Blood")) r.sprite = bloodVatSprite;
                    else if (r.gameObject.name.Contains("Sweat")) r.sprite = sweatVatSprite;
                    else if (r.gameObject.name.Contains("Urine")) r.sprite = urineVatSprite;
                    else if (r.gameObject.name.Contains("OperatingTable")) r.sprite = tableSprite;
                    else r.sprite = genericPartSprite;

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

        private static Sprite GenerateRuntimeSprite(string name, int width, int height, Color color)
        {
            string path = $"Assets/Resources/{name}.png";
            if (!File.Exists(path))
            {
                if (!Directory.Exists("Assets/Resources/")) Directory.CreateDirectory("Assets/Resources/");
                Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
                for(int i=0; i<width; i++) for(int j=0; j<height; j++) tex.SetPixel(i,j, color);
                tex.Apply();
                File.WriteAllBytes(path, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
#endif
