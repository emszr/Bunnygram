using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nonogram
{
    public class Map : MonoBehaviour
    {
        public int index;
        public LevelButton levelButtonPrefab;
        public List<Transform> levelButtonPosList;
        private List<PoolItem> buttonList;
        private PoolItem poolItem;
        public int previousButtonCount;

        private void Awake()
        {
            buttonList = new List<PoolItem>();
        }

        public void Initialize()
        {
            for (int i = 0; i < levelButtonPosList.Count; i++)
            {
                poolItem = PoolManager.instance.GetFromPool(PoolItemType.LevelButton, levelButtonPosList[i]);

                if (poolItem == null)
                {
                    poolItem = Instantiate(levelButtonPrefab.GetComponent<PoolItem>());
                    PoolManager.instance.AddToUsing(poolItem, levelButtonPosList[i]);                 
                }

                buttonList.Add(poolItem);
                poolItem.transform.localPosition = Vector3.zero;
                poolItem.transform.localScale = Vector3.one;
            }

        }

        public List<PoolItem> GetButtonList()
        {
            return buttonList;
        }

        public void SendButtonsBackToPool()
        {
            int size = buttonList.Count;
            for(int i = 0; i < size; i++)
            {
                poolItem = buttonList[0];
                buttonList.RemoveAt(0);
                PoolManager.instance.ResetPoolItem(poolItem);
            }
        }

        private void OnEnable()
        {
            buttonList.Clear();
            Initialize();
            
        }

        private void OnDisable()
        {
            
        }
    }
}
