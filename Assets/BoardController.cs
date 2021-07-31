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

                    tile.SnapToPosition(colid, 10);
                    tile.AssignPosition(colid, rowid);
                    tiles.Add(tile);
                }
            }
        }

        void Reclick()
        {
            // check if this is a finishable sequence
            // take action
            // remove selection




            // for now clear it
            ClearSelection();
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
            // must go top down in order
            clearedTiles.OrderByDescending(o => o.row);
            foreach(GameTile t in clearedTiles)
            {
                RecascadeTile(t);
            }
        }

        void RecascadeTile(GameTile tile)
        {
            // get tiles above me
            List<GameTile> aboveTiles = tiles.FindAll((o) => o.col == tile.col && o.row > tile.row);
            Debug.Log("above tiles count " + aboveTiles.Count);
            // drop down one
            foreach(GameTile t in aboveTiles)
            {
                t.row -= 1;
            }

            tile.SnapToPosition(tile.col, 9);
            tile.row = 8;
        }

        public void OnTileClick(GameTile tile)
        {
            if (selection.Contains(tile))
            {
                int index = selection.IndexOf(tile);

                // re-clicking the last item?
                bool reclickingLast = (index + 1 == selection.Count);
                Debug.Log(index + " " + selection.Count);
                if (reclickingLast)
                {
                    Debug.Log("last item");
                    Reclick();
                    return;
                }
                Debug.Log("not last item");

                // trim back to the selection
                List<GameTile> tilesToUnhighlight = selection.GetRange(index, selection.Count - index);
                Debug.Log(selection.Count + " " + index + " " + (selection.Count - index));
                foreach(GameTile t in tilesToUnhighlight)
                {
                    t.ToggleHighlight(false);
                    selection.Remove(t);
                }
            } else
            {
                bool isEligible = false;

                // eligible to select if selection is empty
                if (selection.Count == 0) {
                    Debug.Log("first selection");
                    isEligible = true;
                }
                // eligible to select if tangential to last
                GameTile lastTile = selection.Count > 0 ? selection.ElementAt(selection.Count - 1) : null;
                if (lastTile != null && isTangential(tile, lastTile))
                {
                    Debug.Log("tangential");
                    isEligible = true;
                }


                if (isEligible)
                {
                    // add to selection
                    selection.Add(tile);
                    tile.ToggleHighlight(true);
                    Debug.Log("elig");
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
