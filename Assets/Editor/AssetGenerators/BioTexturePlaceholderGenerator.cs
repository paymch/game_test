#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace MyGardenFriend.Editor.AssetGenerators
{
    public class BioTexturePlaceholderGenerator
    {
        [MenuItem("Tools/BioGarden/Generate Placeholder Textures")]
        public static void GeneratePlaceholders()
        {
            GenerateOrgan("Eye", 128, 128, Color.white);
            GenerateOrgan("Ear", 128, 128, new Color(0.9f, 0.7f, 0.6f));
            GenerateOrgan("Heel", 128, 128, new Color(0.8f, 0.6f, 0.5f));
            GenerateOrgan("Fingernail", 64, 64, new Color(0.9f, 0.9f, 0.8f));
            GenerateOrgan("Hair", 256, 256, new Color(0.2f, 0.1f, 0.05f));
            GenerateOrgan("Brain", 256, 256, new Color(0.8f, 0.5f, 0.6f));
            GenerateOrgan("Heart", 128, 128, new Color(0.8f, 0.2f, 0.2f));
            GenerateOrgan("Lung", 128, 256, new Color(0.7f, 0.4f, 0.4f));

            GenerateTree("BloodTree", 256, 512, new Color(0.4f, 0.2f, 0.2f));
            GenerateTree("SweatTree", 256, 512, new Color(0.6f, 0.5f, 0.4f));
            GenerateTree("UrineTree", 256, 512, new Color(0.7f, 0.6f, 0.3f));
            GenerateTree("Flask", 256, 256, new Color(0.8f, 0.9f, 1.0f, 0.5f));
            GenerateTree("FluidDrop", 64, 64, new Color(0.5f, 0.7f, 0.9f, 0.8f));

            GenerateMannequin("Head", 128, 128, Color.gray);
            GenerateMannequin("Neck", 64, 64, Color.gray);
            GenerateMannequin("Torso", 256, 256, Color.gray);
            GenerateMannequin("UpperArm", 64, 128, Color.gray);
            GenerateMannequin("Forearm", 64, 128, Color.gray);
            GenerateMannequin("Hand", 64, 64, Color.gray);
            GenerateMannequin("Thigh", 64, 128, Color.gray);
            GenerateMannequin("Shin", 64, 128, Color.gray);
            GenerateMannequin("Foot", 64, 64, Color.gray);

            AssetDatabase.Refresh();
            Debug.Log("Placeholder textures generated.");
        }

        private static void GenerateOrgan(string name, int width, int height, Color color)
        {
            GenerateTexture(name, width, height, color, "Assets/Art/Textures/Organs/");
        }

        private static void GenerateTree(string name, int width, int height, Color color)
        {
            GenerateTexture(name, width, height, color, "Assets/Art/Textures/Trees/");
        }

        private static void GenerateMannequin(string name, int width, int height, Color color)
        {
            GenerateTexture(name, width, height, color, "Assets/Art/Textures/Mannequin/");
        }

        private static void GenerateTexture(string name, int width, int height, Color color, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color clearColor = new Color(0, 0, 0, 0);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Create a simple circular/oval silhouette based on distance from center
                    float u = (x / (float)width) * 2f - 1f;
                    float v = (y / (float)height) * 2f - 1f;

                    if (u * u + v * v < 1f)
                    {
                        tex.SetPixel(x, y, color);
                    }
                    else
                    {
                        tex.SetPixel(x, y, clearColor);
                    }
                }
            }
            tex.Apply();

            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path + name + ".png", bytes);
            Object.DestroyImmediate(tex);
        }
    }
}
#endif
