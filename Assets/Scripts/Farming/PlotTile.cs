using UnityEngine;
using MyGardenFriend.Core;

namespace MyGardenFriend.Farming
{
    public enum GrowthStage
    {
        Empty,
        Seed,
        Sprout,
        Mature,
        Harvestable,
        Spoiled
    }

    public enum PathogenType
    {
        None,
        Pus,
        BioPestilence,
        Necrosis
    }

    public class PlotTile : MonoBehaviour
    {
        public GardenPlotController Controller { get; private set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }

        public GrowthStage CurrentStage { get; private set; } = GrowthStage.Empty;
        public PathogenType CurrentPathogen { get; set; } = PathogenType.None;

        public BioCropData CurrentCropData { get; private set; }

        private float growthTimer = 0f;
        private float spoilTimer = 0f;

        public void Initialize(GardenPlotController controller, int x, int y)
        {
            Controller = controller;
            GridX = x;
            GridY = y;
        }

        public bool PlantSeed(BioCropData cropData)
        {
            if (CurrentStage != GrowthStage.Empty) return false;

            if (ResourceManager.Instance.ConsumeFluid(cropData.waterRequirementType, 10f)) // Arbitrary cost for planting
            {
                CurrentCropData = cropData;
                CurrentStage = GrowthStage.Seed;
                growthTimer = 0f;
                spoilTimer = 0f;
                UpdateVisuals();
                return true;
            }
            return false;
        }

        private void Update()
        {
            if (CurrentStage == GrowthStage.Empty) return;

            if (CurrentStage == GrowthStage.Seed || CurrentStage == GrowthStage.Sprout || CurrentStage == GrowthStage.Mature)
            {
                float growthMultiplier = 1f;

                if (CurrentPathogen == PathogenType.Pus)
                {
                    growthMultiplier = 0.5f; // Slows growth
                }

                growthTimer += Time.deltaTime * growthMultiplier;

                if (growthTimer >= CurrentCropData.growthTime)
                {
                    if (CurrentPathogen == PathogenType.BioPestilence)
                    {
                        CurrentStage = GrowthStage.Spoiled;
                    }
                    else
                    {
                        CurrentStage = GrowthStage.Harvestable;
                    }
                    UpdateVisuals();
                }
                else if (growthTimer >= CurrentCropData.growthTime * 0.66f && CurrentStage != GrowthStage.Mature)
                {
                    CurrentStage = GrowthStage.Mature;
                    UpdateVisuals();
                }
                else if (growthTimer >= CurrentCropData.growthTime * 0.33f && CurrentStage != GrowthStage.Sprout)
                {
                    CurrentStage = GrowthStage.Sprout;
                    UpdateVisuals();
                }
            }
            else if (CurrentStage == GrowthStage.Harvestable)
            {
                spoilTimer += Time.deltaTime;
                if (spoilTimer >= CurrentCropData.spoilTime || CurrentPathogen == PathogenType.Necrosis)
                {
                    CurrentStage = GrowthStage.Spoiled;
                    UpdateVisuals();
                }
            }
        }

        public void Harvest()
        {
            if (CurrentStage == GrowthStage.Harvestable)
            {
                ResourceManager.Instance.AddBodyPart(CurrentCropData.cropType, CurrentCropData.yieldAmount);
                ClearTile();
            }
            else if (CurrentStage == GrowthStage.Spoiled)
            {
                ClearTile();
            }
        }

        public void ClearTile()
        {
            CurrentStage = GrowthStage.Empty;
            CurrentCropData = null;
            CurrentPathogen = PathogenType.None;
            UpdateVisuals();
        }

        public void TreatPathogen()
        {
             CurrentPathogen = PathogenType.None;
             UpdateVisuals();
        }

        public void Infect(PathogenType pathogen)
        {
             CurrentPathogen = pathogen;
             UpdateVisuals();
        }

        private void OnMouseDown()
        {
            if (CurrentStage == GrowthStage.Harvestable || CurrentStage == GrowthStage.Spoiled)
            {
                Harvest();
            }
            else if (CurrentStage == GrowthStage.Empty)
            {
                // Fallback for click-to-plant directly
                BioCropData testCrop = ScriptableObject.CreateInstance<BioCropData>();
                testCrop.waterRequirementType = FluidType.Blood;
                testCrop.cropType = BioCropType.Eyes;
                testCrop.growthTime = 3f;
                testCrop.yieldAmount = 1;
                testCrop.spoilTime = 10f;
                PlantSeed(testCrop);
            }
        }

        private void UpdateVisuals()
        {
            // Update Sprite based on GrowthStage
            var renderer = GetComponent<SpriteRenderer>();
            if (renderer != null && CurrentCropData != null && CurrentCropData.growthStageSprites != null)
            {
                int stageIndex = (int)CurrentStage;
                if (stageIndex >= 0 && stageIndex < CurrentCropData.growthStageSprites.Length)
                {
                    renderer.sprite = CurrentCropData.growthStageSprites[stageIndex];
                }
            }

            // Update InfectionVisualOverlay if present
            var overlay = GetComponent<MyGardenFriend.Visuals.InfectionVisualOverlay>();
            if (overlay != null)
            {
                overlay.UpdateOverlay(CurrentPathogen);
            }
        }
    }
}
