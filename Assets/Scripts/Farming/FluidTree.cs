using UnityEngine;
using MyGardenFriend.Core;

namespace MyGardenFriend.Farming
{
    public class FluidTree : MonoBehaviour
    {
        public FluidType producedFluid;
        public float generationRate = 5f; // Amount per tick
        public float tickInterval = 2f; // Seconds between ticks

        private float timer = 0f;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= tickInterval)
            {
                GenerateFluid();
                timer = 0f;
            }
        }

        private void GenerateFluid()
        {
            if (ResourceManager.Instance != null && producedFluid != FluidType.None)
            {
                ResourceManager.Instance.AddFluid(producedFluid, generationRate);
            }
        }
    }
}
