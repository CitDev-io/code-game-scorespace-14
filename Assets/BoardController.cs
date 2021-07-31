using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using citdev;

namespace citdev {
    public class BoardController : MonoBehaviour
    {
        [SerializeField] GameObject tilePrefab;
        List<GameTile> tiles = new List<GameTile>();
        List<GameTile> selection = new List<GameTile>();

        TileType GetRandomNextTile()
        {
            int tileChoice = Random.Range(0, 4);
            return (TileType)tileChoice;
        }

        void StartGame()
        {
            for (var rowid = 0; rowid <= 8; rowid++)
            {
                for (var colid = 0; colid <= 8; colid++)
                {
                    GameObject g = GameObject.Instantiate(
                        tilePrefab,
                        new Vector2(rowid, colid),
                        Quaternion.identity
                    );
                    GameTile tile = g.GetComponent<GameTile>();
                    tile.SetTileType(GetRandomNextTile());
                    tile.SnapToPosition(colid, 10);
                    tile.AssignPosition(colid, rowid);
                    tiles.Add(tile);
                }
            }
        }

        void CollectTiles(List<GameTile> collected)
        {
            // yay! here's the stuff you got

        }

        bool Reclick()
        {
            // check if this is a finishable sequence
            // take action
            // remove selection


            CollectTiles(selection);
            ClearSelection();
            return true;
        }

        void ClearSelection()
        {
            ClearTiles(selection);
            foreach(GameTile t in selection)
            {
                t.ToggleHighlight(false);
            }
            selection.Clear();
        }

        void ClearTiles(List<GameTile> clearedTiles)
        {
            clearedTiles.OrderByDescending(o => o.row);
            foreach(GameTile t in clearedTiles)
            {
                RecascadeTile(t);
            }
        }

        void RecascadeTile(GameTile tile)
        {
            List<GameTile> aboveTiles = tiles.FindAll((o) => o.col == tile.col && o.row > tile.row);

            foreach(GameTile t in aboveTiles)
            {
                t.row -= 1;
            }

            tile.SnapToPosition(tile.col, 9);
            tile.SetTileType(GetRandomNextTile());
            tile.row = 8;
        }

        public void OnTileClick(GameTile tile)
        {
            if (selection.Contains(tile))
            {
                int index = selection.IndexOf(tile);

                bool reclickingLast = (index + 1 == selection.Count);
                if (reclickingLast)
                {
                    bool captureableReclick = Reclick();
                    if (captureableReclick)
                    {
                        return;
                    }
                }

                List<GameTile> tilesToUnhighlight = selection.GetRange(index, selection.Count - index);
                foreach(GameTile t in tilesToUnhighlight)
                {
                    t.ToggleHighlight(false);
                    selection.Remove(t);
                }
            } else
            {
                bool isEligible = false;

                if (selection.Count == 0) {
                    isEligible = true;
                }

                GameTile lastTile = selection.Count > 0 ? selection.ElementAt(selection.Count - 1) : null;
                if (
                    lastTile != null
                    && isTangential(tile, lastTile)
                    && lastTile.tileType == tile.tileType
                )
                {
                    isEligible = true;
                }


                if (isEligible)
                {
                    selection.Add(tile);
                    tile.ToggleHighlight(true);
                } else
                {
                    // Clicked an ineligible tile
                }
            }


        }

        bool isTangential(GameTile tile1, GameTile tile2)
        {
            int rowDiff = Mathf.Abs(tile1.row - tile2.row);
            int colDiff = Mathf.Abs(tile1.col - tile2.col);
            return (rowDiff < 2) && (colDiff < 2) && (rowDiff + colDiff > 0);
        }

        void Start()
        {
            StartGame();
        }

        void Update()
        {

        }
    }

}
