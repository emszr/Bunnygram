using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nonogram
{

    public class Bomb : MonoBehaviour
    {
        public void RaiseEvent()
        {
            GameManager.instance.OnBombAnimationEnded?.Invoke();
        }
    }
}
