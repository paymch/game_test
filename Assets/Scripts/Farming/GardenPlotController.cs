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
            gridList = new PlotTile[gridWidth * gridHeight];

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int i = x + y * gridWidth;

                    GameObject tileObj = new GameObject($"PlotTile_{x}_{y}");
                    tileObj.transform.parent = this.transform;
                    tileObj.transform.localPosition = new Vector3(x * tileSize, y * tileSize, 0);

                    // Add visuals and physics
                    SpriteRenderer sr = tileObj.AddComponent<SpriteRenderer>();
                    sr.sortingOrder = 0;
                    if (MyGardenFriend.Visuals.RuntimeTextureGenerator.Instance != null)
                    {
                        sr.sprite = MyGardenFriend.Visuals.RuntimeTextureGenerator.Instance.GetSprite("SoilBed");
                    }

                    BoxCollider2D col = tileObj.AddComponent<BoxCollider2D>();
                    col.size = new Vector2(tileSize, tileSize);

                    PlotTile tile = tileObj.AddComponent<PlotTile>();
                    tile.Initialize(this, x, y);

                    gridList[i] = tile;
                }
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
