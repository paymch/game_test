using UnityEngine;
using System.Collections.Generic;

namespace MyGardenFriend.Farming
{
    public class PathogenController : MonoBehaviour
    {
        public GardenPlotController gardenController;
        public float contagionTickInterval = 5f;
        public float infectionChance = 0.2f;

        private float timer = 0f;

        private void Update()
        {
            if (gardenController == null || gardenController.Grid == null) return;

            timer += Time.deltaTime;
            if (timer >= contagionTickInterval)
            {
                SpreadContagion();
                timer = 0f;
            }
        }

        private void SpreadContagion()
        {
            int width = gardenController.gridWidth;
            int height = gardenController.gridHeight;
            PlotTile[,] grid = gardenController.Grid;

            // We need a list of new infections so we don't spread newly infected tiles in the same tick
            List<PlotTile> tilesToInfect = new List<PlotTile>();
            List<PathogenType> pathogensToSpread = new List<PathogenType>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PlotTile tile = grid[x, y];
                    if (tile != null && tile.CurrentPathogen != PathogenType.None)
                    {
                        List<PlotTile> neighbors = gardenController.GetNeighbors(x, y);
                        foreach (PlotTile neighbor in neighbors)
                        {
                            if (neighbor.CurrentStage != GrowthStage.Empty && neighbor.CurrentPathogen == PathogenType.None)
                            {
                                if (Random.value <= infectionChance && !tilesToInfect.Contains(neighbor))
                                {
                                    tilesToInfect.Add(neighbor);
                                    pathogensToSpread.Add(tile.CurrentPathogen);
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < tilesToInfect.Count; i++)
            {
                tilesToInfect[i].Infect(pathogensToSpread[i]);
            }
        }
    }
}
