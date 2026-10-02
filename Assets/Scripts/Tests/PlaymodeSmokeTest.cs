using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MyGardenFriend.Core;
using MyGardenFriend.Farming;
using MyGardenFriend.Assembly;

namespace MyGardenFriend.Tests
{
    public class PlaymodeSmokeTest
    {
        private GameObject gameManagerObj;
        private GameObject resourceManagerObj;
        private GardenPlotController gardenController;
        private FluidTree bloodTree;
        private AssemblyWorkbench workbench;

        [SetUp]
        public void Setup()
        {
            // Initialize Core Managers
            gameManagerObj = new GameObject();
            gameManagerObj.AddComponent<GameManager>();

            resourceManagerObj = new GameObject();
            resourceManagerObj.AddComponent<ResourceManager>();

            // Setup Garden
            GameObject gardenObj = new GameObject();
            gardenController = gardenObj.AddComponent<GardenPlotController>();

            // Setup a Plot Tile
            GameObject tileObj = new GameObject();
            tileObj.transform.parent = gardenObj.transform;
            PlotTile tile = tileObj.AddComponent<PlotTile>();

            // Setup a Fluid Tree
            GameObject treeObj = new GameObject();
            bloodTree = treeObj.AddComponent<FluidTree>();
            bloodTree.producedFluid = FluidType.Blood;
            bloodTree.generationRate = 10f;

            // Setup Workbench
            GameObject benchObj = new GameObject();
            workbench = benchObj.AddComponent<AssemblyWorkbench>();

            // Setup a Socket
            GameObject socketObj = new GameObject();
            socketObj.transform.parent = benchObj.transform;
            AssemblySlot slot = socketObj.AddComponent<AssemblySlot>();
            slot.acceptedPartType = BioCropType.Eyes;
            slot.isEssential = false;

            // Setup an Essential Socket
            GameObject essentialSocketObj = new GameObject();
            essentialSocketObj.transform.parent = benchObj.transform;
            AssemblySlot essentialSlot = essentialSocketObj.AddComponent<AssemblySlot>();
            essentialSlot.acceptedPartType = BioCropType.Eyes; // In the enum provided we don't have Brain, so we mock with Eyes to represent an essential slot missing
            essentialSlot.isEssential = true;
        }

        [UnityTest]
        public IEnumerator TestFullGameplayLoop()
        {
            // 1. Fluid Gathering
            float initialBlood = ResourceManager.Instance.GetFluidAmount(FluidType.Blood);
            bloodTree.SendMessage("OnMouseDown"); // Simulate click
            yield return null;
            float newBlood = ResourceManager.Instance.GetFluidAmount(FluidType.Blood);
            Assert.Greater(newBlood, initialBlood, "Fluid gathering failed");

            // 2. Planting & Irrigation
            PlotTile targetTile = gardenController.Grid[0];
            BioCropData testCrop = ScriptableObject.CreateInstance<BioCropData>();
            testCrop.waterRequirementType = FluidType.Blood;
            testCrop.cropType = BioCropType.Eyes;
            testCrop.growthTime = 0.1f;
            testCrop.yieldAmount = 1;

            GameManager.Instance.SelectedSeed = testCrop;
            targetTile.SendMessage("OnMouseDown"); // Trigger click to plant
            yield return null;
            Assert.AreEqual(GrowthStage.Seed, targetTile.CurrentStage, "Planting failed");

            // Wait for growth
            yield return new WaitForSeconds(0.2f);
            Assert.AreEqual(GrowthStage.Harvestable, targetTile.CurrentStage, "Growth failed");

            // 3. Harvesting
            targetTile.SendMessage("OnMouseDown"); // Simulate harvest click
            yield return null;
            Assert.AreEqual(1, ResourceManager.Instance.GetBodyPartAmount(BioCropType.Eyes), "Harvesting failed");
            Assert.AreEqual(GrowthStage.Empty, targetTile.CurrentStage, "Tile not cleared after harvest");

            // 4. Assembly & Viability
            workbench.SendMessage("OnMouseDown"); // Simulate bench click to auto-slot
            yield return null;
            Assert.AreEqual(0, ResourceManager.Instance.GetBodyPartAmount(BioCropType.Eyes), "Inventory not consumed during slotting");
            Assert.IsTrue(workbench.slots[0].IsFilled, "Part not slotted into workbench");

            // 5. Viability Eval
            GameObject evalObj = new GameObject();
            CreatureViabilityEvaluator evaluator = evalObj.AddComponent<CreatureViabilityEvaluator>();
            evaluator.workbench = workbench;

            ViabilityState state = evaluator.EvaluateViability(out float timer);

            // Expected Incomplete because we lack essential organs in this test setup
            Assert.AreEqual(ViabilityState.Incomplete, state, "Expected Incomplete state due to missing essentials");
        }
    }
}
