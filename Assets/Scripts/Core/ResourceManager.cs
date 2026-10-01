using System;
using System.Collections.Generic;
using UnityEngine;
using MyGardenFriend.Farming;

namespace MyGardenFriend.Core
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        private Dictionary<FluidType, float> fluids = new Dictionary<FluidType, float>();
        private Dictionary<BioCropType, int> bodyParts = new Dictionary<BioCropType, int>();

        public event Action OnResourcesChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeResources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeResources()
        {
            fluids[FluidType.Blood] = 0f;
            fluids[FluidType.Sweat] = 0f;
            fluids[FluidType.Urine] = 0f;

            foreach (BioCropType cropType in Enum.GetValues(typeof(BioCropType)))
            {
                bodyParts[cropType] = 0;
            }
        }

        public void AddFluid(FluidType type, float amount)
        {
            if (type == FluidType.None) return;

            if (fluids.ContainsKey(type))
            {
                fluids[type] += amount;
                OnResourcesChanged?.Invoke();
            }
        }

        public bool ConsumeFluid(FluidType type, float amount)
        {
            if (type == FluidType.None) return true;

            if (fluids.ContainsKey(type) && fluids[type] >= amount)
            {
                fluids[type] -= amount;
                OnResourcesChanged?.Invoke();
                return true;
            }
            return false;
        }

        public float GetFluidAmount(FluidType type)
        {
            if (fluids.ContainsKey(type))
            {
                return fluids[type];
            }
            return 0f;
        }

        public void AddBodyPart(BioCropType type, int amount)
        {
            if (bodyParts.ContainsKey(type))
            {
                bodyParts[type] += amount;
                OnResourcesChanged?.Invoke();
            }
        }

        public bool ConsumeBodyPart(BioCropType type, int amount)
        {
            if (bodyParts.ContainsKey(type) && bodyParts[type] >= amount)
            {
                bodyParts[type] -= amount;
                OnResourcesChanged?.Invoke();
                return true;
            }
            return false;
        }

        public int GetBodyPartAmount(BioCropType type)
        {
            if (bodyParts.ContainsKey(type))
            {
                return bodyParts[type];
            }
            return 0;
        }
    }
}
