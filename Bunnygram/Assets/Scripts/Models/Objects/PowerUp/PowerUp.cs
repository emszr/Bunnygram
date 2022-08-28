using System.Collections.Generic;
using UnityEngine;

namespace Nonogram
{
    public abstract class PowerUp: ScriptableObject
    {
        public float delayTime = 0.05f;
        public bool canBeUsed = false;
        public abstract void Use(List<Cell> list);
    }
}
