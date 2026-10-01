using UnityEngine;

namespace MyGardenFriend.Farming
{
    public enum FluidType
    {
        None,
        Blood,
        Sweat,
        Urine
    }

    public enum BioCropType
    {
        Eyes,
        Ears,
        Fingernails,
        Heels,
        HairLawn
    }

    [CreateAssetMenu(fileName = "NewBioCropData", menuName = "My Garden Friend/Bio Crop Data")]
    public class BioCropData : ScriptableObject
    {
        [Header("Identity")]
        public BioCropType cropType;
        public string cropName;

        [Header("Growth Parameters")]
        public float growthTime; // Time in seconds to grow fully
        public FluidType waterRequirementType; // The fluid needed to grow

        [Header("Harvest Parameters")]
        public int yieldAmount; // Amount of items yielded
        public float spoilTime; // Time in seconds before it spoils after fully grown

        [Header("Visuals")]
        public Sprite[] growthStageSprites; // Sprites for each growth stage
    }
}
