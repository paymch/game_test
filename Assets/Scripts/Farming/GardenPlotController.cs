using UnityEngine;
using System.Collections.Generic;

namespace MyGardenFriend.Farming
{
    public class GardenPlotController : MonoBehaviour
    {
        public int gridWidth = 5;
        public int gridHeight = 5;
        public float tileSize = 1.0f;

        private PlotTile[] gridList;
        public PlotTile[] Grid => gridList;

        private void Start()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            gridList = GetComponentsInChildren<PlotTile>();
            int i = 0;
            foreach (var tile in gridList)
            {
                tile.Initialize(this, i % gridWidth, i / gridWidth);
                i++;
            }
        }

        public List<PlotTile> GetNeighbors(int x, int y)
        {
            List<PlotTile> neighbors = new List<PlotTile>();

            if (gridList == null) return neighbors;

            if (x > 0)
            {
                int idx = (x - 1) + y * gridWidth;
                if(idx >= 0 && idx < gridList.Length) neighbors.Add(gridList[idx]);
            }
            if (x < gridWidth - 1)
            {
                int idx = (x + 1) + y * gridWidth;
                if(idx >= 0 && idx < gridList.Length) neighbors.Add(gridList[idx]);
            }
            if (y > 0)
            {
                int idx = x + (y - 1) * gridWidth;
                if(idx >= 0 && idx < gridList.Length) neighbors.Add(gridList[idx]);
            }
            if (y < gridHeight - 1)
            {
                int idx = x + (y + 1) * gridWidth;
                if(idx >= 0 && idx < gridList.Length) neighbors.Add(gridList[idx]);
            }

            return neighbors;
        }
    }
}
