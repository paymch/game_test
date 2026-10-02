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

        private GameObject cropChild;
        private SpriteRenderer cropRenderer;

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

                if (cropChild == null)
                {
                    cropChild = new GameObject("CropSprite");
                    cropChild.transform.parent = this.transform;
                    cropChild.transform.localPosition = Vector3.zero;
                    cropRenderer = cropChild.AddComponent<SpriteRenderer>();
                    cropRenderer.sortingOrder = 5;
                }

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

                // Crop Growth Animation (Scale up based on growth progress)
                if (cropChild != null && CurrentCropData != null && CurrentCropData.growthTime > 0)
                {
                    float progress = Mathf.Clamp01(growthTimer / CurrentCropData.growthTime);
                    // Start small, grow to full scale
                    float scale = Mathf.Lerp(0.2f, 1.0f, progress);
                    cropChild.transform.localScale = new Vector3(scale, scale, scale);
                }

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
            if (cropChild != null)
            {
                Destroy(cropChild);
                cropChild = null;
                cropRenderer = null;
            }
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
                var gm = GameManager.Instance;
                if (gm == null) gm = FindObjectOfType<GameManager>();

                if (gm != null && gm.SelectedSeed != null)
                {
                    PlantSeed(gm.SelectedSeed);
                }
            }
        }

        private void UpdateVisuals()
        {
            // Update Sprite based on GrowthStage
            if (cropRenderer != null && CurrentCropData != null && CurrentCropData.growthStageSprites != null)
            {
                int stageIndex = (int)CurrentStage;
                if (stageIndex >= 0 && stageIndex < CurrentCropData.growthStageSprites.Length)
                {
                    cropRenderer.sprite = CurrentCropData.growthStageSprites[stageIndex];
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
