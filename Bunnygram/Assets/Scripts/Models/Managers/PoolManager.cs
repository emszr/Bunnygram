using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nonogram
{
    public class PoolManager : Singleton<PoolManager>
    {
        public int poolSize;
        public int infoPoolSize;
        public Dictionary<PoolItemType, Queue<PoolItem>> availablePoolDictionary;
        public static Dictionary<PoolItemType, Queue<PoolItem>> usingPoolDictionary;
        public List<PoolItem> prefabList;

        private List<ObjectPool> objectPoolList;

        private void Start()
        {
            DontDestroyOnLoad(this);
            objectPoolList = new List<ObjectPool>(prefabList.Count);

            for (int i = 0; i < prefabList.Count; i++)
            {
                ObjectPool tempPool = new ObjectPool(prefabList[i].GetComponent<PoolItem>().GetPoolItemType(), poolSize);
                objectPoolList.Add(tempPool);
            }
        }

        public ObjectPool GetObjectPool(PoolItemType poolItemType)
        {
            foreach (ObjectPool objectPool in objectPoolList)
            {
                if (objectPool.GetPoolItemType() == poolItemType)
                {
                    return objectPool;
                }
            }

            return null;
        }

        public void AddToAvailable(PoolItem poolItem)
        {
            ObjectPool objectPool = GetObjectPool(poolItem.GetPoolItemType());

            if (objectPool == null)
            {
                return;
            }

            if(poolItem == null)
            {
                Debug.LogWarning("Available List Pool Item is Null");
                return;
            }

            if (objectPool.availableQueue.Contains(poolItem))
            {
                Debug.LogWarning("Available List Contains the Same Item!");
                return;
            }

            objectPool.availableQueue.Add(poolItem);
            poolItem.gameObject.transform.SetParent(this.transform);
            poolItem.gameObject.SetActive(false);
        }

        public void AddToUsing(PoolItem poolItem, Transform parentTransform)
        {
            ObjectPool objectPool = GetObjectPool(poolItem.GetPoolItemType());

            if (objectPool == null)
            {
                return;
            }

            if (objectPool.availableQueue.Contains(poolItem))
            {
                Debug.LogWarning("Using List Contains the Same Item!");
                return;
            }

            objectPool.usingQueue.Add(poolItem);
            poolItem.gameObject.SetActive(true);
            poolItem.gameObject.transform.SetParent(parentTransform);
        }

        public PoolItem GetFromPool(PoolItemType poolItemType, Transform parentTransform)
        {
            ObjectPool objectPool = GetObjectPool(poolItemType);

            if (objectPool == null)
            {
                return null;
            }
            else if (objectPool.availableQueue.Count == 0)
            {
                return null;
            }

            PoolItem tempPoolItem = objectPool.availableQueue[0];
            objectPool.availableQueue.Remove(tempPoolItem);
            AddToUsing(tempPoolItem, parentTransform);
            return tempPoolItem;
        }

        public void ResetPoolItem(PoolItem poolItem)
        {
            ObjectPool objectPool = GetObjectPool(poolItem.GetPoolItemType());

            if (objectPool == null)
            {
                return;
            }

            objectPool.usingQueue.Remove(poolItem);
            AddToAvailable(poolItem);
        }

        public void ResetPoolItemsType(PoolItemType poolItemType)
        {
            ObjectPool objectPool = GetObjectPool(poolItemType);

            if (objectPool == null)
            {
                return;
            }

            foreach (PoolItem poolItem in objectPool.usingQueue)
            {
                ResetPoolItem(poolItem);
            }
        }

        public void ResetAllPools()
        {
            foreach (ObjectPool objectPool in objectPoolList)
            {
                int size = objectPool.usingQueue.Count;
                for (int i = 0; i < size; i++)
                {
                    PoolItem poolItem = objectPool.usingQueue[0];
                    ResetPoolItem(poolItem);
                }
            }
        }
    }
}