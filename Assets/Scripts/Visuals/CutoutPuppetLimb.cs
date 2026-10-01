using UnityEngine;

namespace MyGardenFriend.Visuals
{
    [RequireComponent(typeof(CyriakTwitchController))]
    public class CutoutPuppetLimb : MonoBehaviour
    {
        private CyriakTwitchController twitchController;

        private void Awake()
        {
            twitchController = GetComponent<CyriakTwitchController>();
        }

        public void PlaySlottedReaction()
        {
            if (twitchController != null)
            {
                // Force a spasm for a short duration
                StartCoroutine(SpasmRoutine(1.5f));
            }
        }

        private System.Collections.IEnumerator SpasmRoutine(float duration)
        {
            twitchController.isSpasming = true;
            yield return new WaitForSeconds(duration);
            twitchController.isSpasming = false;
        }
    }
}
