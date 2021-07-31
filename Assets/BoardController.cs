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
        RoundController _rc;

        int TOP_COLUMN_INDEX = 7;
        int COUNT_ROWS = 8;
        int COUNT_COLS = 8;

        private void Awake()
        {
            _rc = GameObject.FindObjectOfType<RoundController>();
        }

        void StartGame()
        {
            for (var rowid = 0; rowid < COUNT_ROWS; rowid++)
            {
                for (var colid = 0; colid < COUNT_COLS; colid++)
                {
                    GameObject g = GameObject.Instantiate(
                        tilePrefab,
                        new Vector2(rowid, colid),
                        Quaternion.identity
                    );
                    GameTile tile = g.GetComponent<GameTile>();
                    _rc.RecycleTileForPosition(tile, new Vector2(colid, rowid));
                    tiles.Add(tile);
                }
            }
        }

        void CollectTiles(List<GameTile> collected)
        {
            _rc.PlayerCollectedTiles(collected, this);
        }

        bool Reclick()
        {
            var finishable = selection.Count > 2;

            if (finishable)
            {
                CollectTiles(selection);
                ClearSelection();
            }

            return finishable;
        }

        void ClearSelection()
        {
            foreach(GameTile t in selection)
            {
                t.ToggleHighlight(false);
            }
            selection.Clear();
        }

        public List<GameTile> GetMonsters()
        {
            return tiles.Where((o) => o.tileType == TileType.Monster).ToList();
        }

        public void ClearTiles(List<GameTile> clearedTiles)
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

            _rc.RecycleTileForPosition(tile, new Vector2(tile.col, TOP_COLUMN_INDEX));
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

                if (selection.Count == 0 && tile.tileType != TileType.Monster) {
                    isEligible = true;
                }

                GameTile lastTile = selection.Count > 0 ? selection.ElementAt(selection.Count - 1) : null;
                if (
                    lastTile != null
                    && isTangential(tile, lastTile)
                    && isChainable(tile, lastTile)
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

        bool isChainable(GameTile first, GameTile next)
        {
            TileType[] attackChainTiles = new TileType[] { TileType.Sword, TileType.Monster };
            if (first.tileType == next.tileType) return true;

            if (
                attackChainTiles.Contains(first.tileType)
                && attackChainTiles.Contains(next.tileType)
            ) {
                return true;
            }

            return false;
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
