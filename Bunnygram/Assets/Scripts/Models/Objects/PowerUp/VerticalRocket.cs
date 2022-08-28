using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nonogram
{
    public class VerticalRocket : PowerUp
    {
        public override void Use(List<Cell> list)
        {
            if (!canBeUsed) { return; }

            int index = 0;

            var temp2 = new List<Cell>(list);
            temp2.Reverse();
            foreach (Cell cell in temp2)
            {
                if (!cell.isLocked)
                {
                    cell.OpenWithAnimation(index * delayTime);
                    index++;
                }
            }

            var temp = new List<Cell>(list);
            foreach (Cell cell in temp)
            {
                    GameManager.instance.GetGridManager().ValidateWin(cell);
            }
        }

    }

}
