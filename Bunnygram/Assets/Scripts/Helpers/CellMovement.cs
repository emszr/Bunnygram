using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nonogram
{
    public enum CellMovement
    {
        SingleMove = 0,
        HorizontalMove = 1,
        VerticalMove = 2,
        UnplayableMove = 3,
    }
}