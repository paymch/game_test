using UnityEngine;
using MyGardenFriend.Farming;

namespace MyGardenFriend.Visuals
{
    public class InfectionVisualOverlay : MonoBehaviour
    {
        public SpriteRenderer targetRenderer;
        public Color pusColor = new Color(0.8f, 0.9f, 0.2f, 0.7f);
        public Color necrosisColor = new Color(0.2f, 0.1f, 0.1f, 0.9f);

        private CyriakTwitchController twitchController;
        private Color originalColor;

        private void Awake()
        {
            if (targetRenderer == null) targetRenderer = GetComponent<SpriteRenderer>();
            twitchController = GetComponent<CyriakTwitchController>();

            if (targetRenderer != null)
            {
                originalColor = targetRenderer.color;
            }
        }

        public void UpdateOverlay(PathogenType pathogen)
        {
            if (targetRenderer == null) return;

            switch (pathogen)
            {
                case PathogenType.None:
                    targetRenderer.color = originalColor;
                    if (twitchController != null) twitchController.isSpasming = false;
                    break;
                case PathogenType.Pus:
                    targetRenderer.color = pusColor;
                    if (twitchController != null) twitchController.isSpasming = true;
                    break;
                case PathogenType.BioPestilence:
                    targetRenderer.color = Color.magenta; // Placeholder for pestilence
                    if (twitchController != null) twitchController.isSpasming = true;
                    break;
                case PathogenType.Necrosis:
                    targetRenderer.color = necrosisColor;
                    if (twitchController != null) twitchController.isSpasming = false; // Dead, stops twitching
                    break;
            }
        }
    }
}
