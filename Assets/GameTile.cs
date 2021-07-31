using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace citdev {
    public class GameTile : MonoBehaviour
    {
        public int row = 5; // Y Y Y Y Y Y 
        public int col = 5; // X X X X X X
        [SerializeField] GameObject highlight;
        [SerializeField] bool isHighlighted = false;

        float speed = 7f;

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
