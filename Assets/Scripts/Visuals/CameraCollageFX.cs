using UnityEngine;

namespace MyGardenFriend.Visuals
{
    // A simple placeholder for camera effects
    [RequireComponent(typeof(Camera))]
    public class CameraCollageFX : MonoBehaviour
    {
        public bool enableFilmGrain = true;
        public float grainIntensity = 0.05f;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // In a real URP project, this would be handled by a Volume Profile.
            // For the prototype, we simply blit (in built-in pipeline) or do nothing.
            // We'll leave this as a stub that could apply a material shader for chromatic aberration.
            Graphics.Blit(source, destination);
        }
    }
}
