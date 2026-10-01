using UnityEngine;
using System.Collections.Generic;

namespace MyGardenFriend.Farming
{
    public class GardenPlotController : MonoBehaviour
    {
        public int gridWidth = 5;
        public int gridHeight = 5;
        public float tileSize = 1.0f;
        public GameObject plotTilePrefab;

        private PlotTile[,] grid;
        public PlotTile[,] Grid => grid;

        private void Start()
        {
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            grid = new PlotTile[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 position = new Vector3(x * tileSize, 0, y * tileSize) + transform.position;
                    GameObject tileObj = Instantiate(plotTilePrefab, position, Quaternion.identity, transform);
                    PlotTile tile = tileObj.GetComponent<PlotTile>();
                    if (tile != null)
                    {
                        tile.Initialize(this, x, y);
                        grid[x, y] = tile;
                    }
                }
            }
        }

        public List<PlotTile> GetNeighbors(int x, int y)
        {
            List<PlotTile> neighbors = new List<PlotTile>();

            if (x > 0) neighbors.Add(grid[x - 1, y]);
            if (x < gridWidth - 1) neighbors.Add(grid[x + 1, y]);
            if (y > 0) neighbors.Add(grid[x, y - 1]);
            if (y < gridHeight - 1) neighbors.Add(grid[x, y + 1]);

            return neighbors;
        }
    }
}
