using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nonogram
{
    public class HorizontalRocket : PowerUp
    {
        public override void Use(List<Cell> list)
        {
            if (!canBeUsed) { return; }

            list.Reverse();

            int index = 0;
            foreach (Cell cell in list)
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
