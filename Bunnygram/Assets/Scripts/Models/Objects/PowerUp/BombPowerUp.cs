using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Nonogram
{
    public class BombPowerUp : PowerUp
    {
        public override void Use(List<Cell> list)
        {
            if (!canBeUsed) { return; }

            int index = 0;

            for (int i = 0; i < list.Count; i++)
            {
                Cell cell = list[i];
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
