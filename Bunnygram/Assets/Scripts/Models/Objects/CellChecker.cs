using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nonogram
{
    public class CellChecker : MonoBehaviour
    {
        Camera mainCamera;
        Cell cell;
        private int hit_count;
        RaycastHit2D hit;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            hit = Physics2D.Raycast(transform.position, -Vector2.up);

                if (hit)
                {
                    if(hit.transform.CompareTag("Cell"))
                    {
                        cell = hit.transform.GetComponent<Cell>();

                        if (!cell.isLocked)
                        {
                            cell.OpenWithAnimation(0f);
                            GameManager.instance.GetGridManager().ValidateWin(cell);
                        }
                    }
                }
        }
    }
}
