using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nonogram
{
    public class PoolItem : MonoBehaviour
    {
        [SerializeField] private PoolItemType poolItemType;

        public PoolItemType GetPoolItemType()
        {
            return poolItemType;
        }

        public void SetPoolItemType(PoolItemType poolItemType)
        {
            this.poolItemType = poolItemType;
        }
    }
 
}
