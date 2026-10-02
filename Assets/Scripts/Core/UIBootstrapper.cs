using UnityEngine;

namespace MyGardenFriend.Core
{
    using UnityEngine.UI;

    // A runtime bootstrapper to generate interactive UGUI buttons and text dynamically.
    public class UIBootstrapper : MonoBehaviour
    {
        private GameManager gameManager;

        private Text hudText;
        private Text inventoryText;
        private Text statusText;

        private void Start()
        {
            gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            BuildUGUICanvas();
        }

        private void BuildUGUICanvas()
        {
            GameObject canvasObj = new GameObject("Runtime_UI_Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();

            // Setup Text Elements
            hudText = CreateTextElement(canvasObj.transform, "HUD_Text", new Vector2(200, -50));
            inventoryText = CreateTextElement(canvasObj.transform, "Inventory_Text", new Vector2(200, -100));
            statusText = CreateTextElement(canvasObj.transform, "Status_Text", new Vector2(1700, -50));

            // Setup Buttons
            CreateButtonElement(canvasObj.transform, "Btn_PlantEye", "Plant Eye", new Vector2(150, 1000), () => SelectSeed(Farming.BioCropType.Eyes));
            CreateButtonElement(canvasObj.transform, "Btn_PlantHair", "Plant Hair", new Vector2(350, 1000), () => SelectSeed(Farming.BioCropType.HairLawn));
            CreateButtonElement(canvasObj.transform, "Btn_PlantFinger", "Plant Finger", new Vector2(550, 1000), () => SelectSeed(Farming.BioCropType.Fingernails));
            CreateButtonElement(canvasObj.transform, "Btn_Animate", "ANIMATE", new Vector2(1700, 1000), () => gameManager?.AttemptRevive());
        }

        private Text CreateTextElement(Transform parent, string name, Vector2 anchoredPos)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(300, 50);

            Text textComp = textObj.AddComponent<Text>();
            textComp.text = "Initializing...";
            // Font setup would go here in a real environment
            return textComp;
        }

        private void CreateButtonElement(Transform parent, string name, string label, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(150, 50);

            btnObj.AddComponent<Image>(); // Background for button
            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(onClick);

            Text textComp = CreateTextElement(btnObj.transform, "Text", Vector2.zero);
            textComp.text = label;
        }

        private void SelectSeed(Farming.BioCropType cropType)
        {
            Farming.BioCropData seed = ScriptableObject.CreateInstance<Farming.BioCropData>();
            seed.cropType = cropType;
            seed.waterRequirementType = Farming.FluidType.Blood;
            seed.growthTime = 2f;
            seed.yieldAmount = 1;
            seed.spoilTime = 10f;

            if (gameManager != null)
            {
                gameManager.SelectedSeed = seed;
            }
        }

        private void Update()
        {
            if (gameManager == null) return;

            if (hudText != null) hudText.text = gameManager.HUD_Text;
            if (inventoryText != null) inventoryText.text = gameManager.Inventory_Text;
            if (statusText != null) statusText.text = gameManager.Status_Text;
        }
    }
}
