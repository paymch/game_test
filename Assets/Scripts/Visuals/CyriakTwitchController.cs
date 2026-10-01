using UnityEngine;

namespace MyGardenFriend.Visuals
{
    public class CyriakTwitchController : MonoBehaviour
    {
        [Header("Twitch Parameters")]
        public float twitchIntensity = 4f; // Max angle to snap
        public Vector2 twitchFrequencyMinMax = new Vector2(0.1f, 0.5f);

        [Header("Pulse Parameters")]
        public float scalePulseAmount = 0.1f;
        public float scalePulseSpeed = 2f;

        [Header("State")]
        public bool isSpasming = false; // Multiplier for diseased/active parts

        private float nextTwitchTime;
        private Quaternion baseRotation;
        private Vector3 baseScale;
        private Vector3 basePosition;

        private void Start()
        {
            baseRotation = transform.localRotation;
            baseScale = transform.localScale;
            basePosition = transform.localPosition;
            ScheduleNextTwitch();
        }

        private void Update()
        {
            float time = Time.time;

            // 1. Angular Twitch
            if (time >= nextTwitchTime)
            {
                ApplyTwitch();
                ScheduleNextTwitch();
            }

            // 2. Scale Pulse
            float spasmMultiplier = isSpasming ? 3f : 1f;
            float pulse = Mathf.Sin(time * scalePulseSpeed * spasmMultiplier) * scalePulseAmount;
            transform.localScale = baseScale + new Vector3(pulse, pulse, pulse);

            // 3. Translation jitter (segmented paper-doll effect)
            if (isSpasming && time >= nextTwitchTime - 0.05f) // Jitter near twitch
            {
                transform.localPosition = basePosition + (Vector3)Random.insideUnitCircle * 0.05f;
            }
            else
            {
                transform.localPosition = basePosition;
            }
        }

        private void ApplyTwitch()
        {
            float intensity = isSpasming ? twitchIntensity * 2f : twitchIntensity;
            float angle = Random.Range(-intensity, intensity);
            transform.localRotation = baseRotation * Quaternion.Euler(0, 0, angle);
        }

        private void ScheduleNextTwitch()
        {
            float multiplier = isSpasming ? 0.3f : 1f;
            nextTwitchTime = Time.time + Random.Range(twitchFrequencyMinMax.x, twitchFrequencyMinMax.y) * multiplier;
        }
    }
}
