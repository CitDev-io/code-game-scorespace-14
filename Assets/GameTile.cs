using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace citdev {
    public class GameTile : MonoBehaviour
    {
        [Header("Plumbing")]
        [SerializeField] GameObject highlight;
        [SerializeField] SpriteRenderer sr;
        [SerializeField] public TextMeshProUGUI label1;
        [SerializeField] public TextMeshProUGUI label2;
        [SerializeField]
        List<Sprite> icons = new List<Sprite>();

        [Header("State")]
        public bool isHighlighted = false;
        public TileType tileType;
        public int HitPoints = 0;
        public int Power = 0;
        public int TurnAppeared = 0;

        [Space(25)]
        float speed = 7f;

        [Header("Assignment")]
        public int row = 5; // Y Y Y Y Y Y 
        public int col = 5; // X X X X X X

        public void SetTileType(TileType tt)
        {
            tileType = tt;
            sr.sprite = icons[(int)tt];
        }

        void Start()
        {
            highlight.SetActive(false);   
        }

        void Update()
        {
            Vector2 dest = new Vector2(col, row);

            if (Vector2.Distance(transform.position, dest) > 0.2f)
            {
                transform.position = Vector2.MoveTowards(transform.position, dest, speed * Time.deltaTime);
            } else
            {
                transform.position = dest;
            }
        }

        public void SnapToPosition(float x, float y)
        {
            SnapToPosition(new Vector2(x, y));
        }

        public void SnapToPosition(Vector2 newPos)
        {
            transform.position = newPos;
        }

        public void AssignPosition(float x, float y)
        {
            AssignPosition(new Vector2(x, y));
        }

        public void AssignPosition(Vector2 newPos)
        {
            row = (int) newPos.y;
            col = (int) newPos.x;
        }

        private void OnMouseDown()
        {
            var bc = GameObject.FindObjectOfType<BoardController>();

            bc.OnTileClick(this);
        }

        public void ToggleHighlight(bool setVal)
        {
            highlight.SetActive(setVal);
            isHighlighted = setVal;
        }

        public void ToggleHighlight()
        {
            ToggleHighlight(!isHighlighted);
        }
    }
}
