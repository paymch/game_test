using UnityEngine;
using System.Collections.Generic;
using MyGardenFriend.Farming;

namespace MyGardenFriend.Visuals
{
    public class RuntimeTextureGenerator : MonoBehaviour
    {
        public static RuntimeTextureGenerator Instance { get; private set; }

        private Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                GenerateAllSprites();
                AssignToSceneObjects();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void GenerateAllSprites()
        {
            cache["LabBackground"] = GenerateSprite(2, 2, new Color(0.125f, 0.125f, 0.145f));
            cache["SoilBed"] = GenerateSprite(64, 64, new Color(0.3f, 0.2f, 0.1f));
            cache["BloodVat"] = GenerateSprite(64, 64, new Color(0.6f, 0.0f, 0.0f));
            cache["SweatVat"] = GenerateSprite(64, 64, new Color(0.0f, 0.9f, 1.0f));
            cache["UrineVat"] = GenerateSprite(64, 64, new Color(0.9f, 0.75f, 0.0f));
            cache["OperatingTable"] = GenerateSprite(128, 64, new Color(0.16f, 0.16f, 0.18f));
            cache["GenericPart"] = GenerateSprite(32, 32, Color.gray);
        }

        public Sprite GetSprite(string name)
        {
            if (cache.TryGetValue(name, out Sprite s)) return s;
            return cache["GenericPart"];
        }

        private void AssignToSceneObjects()
        {
            SpriteRenderer[] renderers = FindObjectsOfType<SpriteRenderer>();
            foreach (var r in renderers)
            {
                if (r.sprite == null)
                {
                    if (r.gameObject.name.Contains("Background")) r.sprite = cache["LabBackground"];
                    else if (r.gameObject.name.Contains("Bed")) r.sprite = cache["SoilBed"];
                    else if (r.gameObject.name.Contains("Blood")) r.sprite = cache["BloodVat"];
                    else if (r.gameObject.name.Contains("Sweat")) r.sprite = cache["SweatVat"];
                    else if (r.gameObject.name.Contains("Urine")) r.sprite = cache["UrineVat"];
                    else if (r.gameObject.name.Contains("OperatingTable")) r.sprite = cache["OperatingTable"];
                    else r.sprite = cache["GenericPart"];
                }

                // Strictly separate 2D sorting orders
                if (r.gameObject.name.Contains("Background")) r.sortingOrder = -10;
                else if (r.gameObject.name.Contains("Bed") || r.gameObject.name.Contains("Soil")) r.sortingOrder = 0;
                else if (r.gameObject.name.Contains("Crop") || r.gameObject.name.Contains("Tree")) r.sortingOrder = 5;
                else if (r.gameObject.name.Contains("OperatingTable") || r.gameObject.name.Contains("Workbench")) r.sortingOrder = 10;
                else if (r.gameObject.name.Contains("Mannequin") || r.gameObject.name.Contains("Socket")) r.sortingOrder = 15;
                else if (r.gameObject.name.Contains("Organ") || r.gameObject.name.Contains("Part")) r.sortingOrder = 20;
            }
        }

        private Sprite GenerateSprite(int width, int height, Color color)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tex.SetPixel(i, j, color);
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }
    }
}
