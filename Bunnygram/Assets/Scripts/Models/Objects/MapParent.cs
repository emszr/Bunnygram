using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Nonogram
{
    public class MapParent : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Canvas canvas;

        private RectTransform rectTransform;

        private int index;
        private int maxLevelIndex;
        private int moveDest = -1;

        private Vector3 lastMousePos;
        private Vector3 startMousePos;

        private Map botMap;
        private Map midMap;
        private Map topMap;
        private Map tempMap;

        private Vector3 defaultBotPos;
        private Vector3 defaultMidPos;
        private Vector3 defaultTopPos;

        private float mapHeight = 2048f;
        private float mapChangeOffset = 250f;
        private float botBound;
        private float topBound;

        private bool check = false;
        [SerializeField] private MainMenuManager mainMenuManager;

        private void Initialize()
        {
            if (index == 0)
            {
                botMap = null;
                midMap = InstantiateMap(0, 0f);
                topMap = InstantiateMap(1, mapHeight);
            }
            else if (index == maxLevelIndex)
            {
                topMap = null;
                botMap = InstantiateMap(0, -mapHeight);
                midMap = InstantiateMap(1, 0f);
            }
            else
            {
                botMap = InstantiateMap(index - 1, -mapHeight);
                midMap = InstantiateMap(index, 0f);
                topMap = InstantiateMap(index + 1, mapHeight);
            }
        }

        private void Start()
        {
            int mapCounter = -1;
            int temp = LevelManager.instance.dataList.Count;

            while (temp >= 0 && mapCounter < GameManager.instance.mapPrefabList.Count)
            {
                temp -= GameManager.instance.mapPrefabList[mapCounter + 1].levelButtonPosList.Count;
                mapCounter++;
            }

            if (LevelManager.instance.currentLevel.state == 0)
            {
                index = mapCounter;
            }

            else
            {
                temp = LevelManager.instance.currentLevel.levelCount - 1;

                mapCounter = -1;
                while (temp >= 0 && mapCounter < GameManager.instance.mapPrefabList.Count)
                {
                    temp -= GameManager.instance.mapPrefabList[mapCounter + 1].levelButtonPosList.Count;
                    mapCounter++;
                }

                index = mapCounter;
            }

            maxLevelIndex = GameManager.instance.mapPrefabList.Count - 1;
            botBound = (index * mapHeight) - (mapHeight / 2f) + (mapHeight - canvas.GetComponent<RectTransform>().sizeDelta.y) / 2;
            topBound = (-(maxLevelIndex - index) * mapHeight) - (mapHeight / 2f) - (mapHeight - canvas.GetComponent<RectTransform>().sizeDelta.y) / 2;

            defaultBotPos = Vector3.up * (-mapHeight);
            defaultMidPos = Vector3.zero;
            defaultTopPos = Vector3.up * (mapHeight);

            Initialize();

        }

        private void Update()
        {
            if (index == 0)
            {
                check = false;

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    startMousePos = Input.mousePosition;
                    check = true;
                }

                if (scrollRect.content.localPosition.y > botBound)
                {
                    scrollRect.velocity = Vector2.zero;
                    scrollRect.vertical = false;
                    scrollRect.content.localPosition = Vector3.up * botBound;
                }

                if (check && startMousePos.y < lastMousePos.y)
                {
                    scrollRect.vertical = true;
                }

                lastMousePos = Input.mousePosition;
            }

            else if (index == maxLevelIndex)
            {
                check = false;

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    startMousePos = Input.mousePosition;
                    check = true;
                }

                if (scrollRect.content.localPosition.y < topBound)
                {
                    scrollRect.velocity = Vector2.zero;
                    scrollRect.vertical = false;
                    scrollRect.content.localPosition = Vector3.up * topBound;
                }

                if (check && startMousePos.y > lastMousePos.y)
                {
                    scrollRect.vertical = true;
                }

                lastMousePos = Input.mousePosition;
            }
        }

        private int CalculatePreviousButtonCount(int mapIndex)
        {
            int totalCounter = 0;

            for (int i = 0; i < mapIndex; i++)
            {
                totalCounter += GameManager.instance.mapPrefabList[i].levelButtonPosList.Count;
            }

            return totalCounter;
        }



        private Map InstantiateMap(int index, float verticalPosition)
        {
            PoolItem poolItem = PoolManager.instance.GetFromPool(GameManager.instance.mapPrefabList[index].GetComponent<PoolItem>().GetPoolItemType(), transform);

            if (poolItem == null)
            {
                poolItem = Instantiate(PoolManager.instance.prefabList[(int)GameManager.instance.mapPrefabList[index].GetComponent<PoolItem>().GetPoolItemType()]);
                PoolManager.instance.AddToUsing(poolItem, transform);
            }

            rectTransform = poolItem.GetComponent<RectTransform>();
            poolItem.transform.SetParent(transform);
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = new Vector3(0f, verticalPosition, 0f);
            tempMap = poolItem.GetComponent<Map>();
            tempMap.index = index;
            tempMap.previousButtonCount = CalculatePreviousButtonCount(index);
            mainMenuManager.InitializeButton(tempMap.GetButtonList(), tempMap.previousButtonCount);
            return tempMap;
        }

        public void CheckMap(Vector2 vector)
        {
            if (botMap != null && index > 0 && scrollRect.content.transform.localPosition.y > -botMap.transform.localPosition.y - (mapHeight / 2f) - mapChangeOffset)
            {
                index--;
                moveDest = 0;
            }

            else if (topMap != null && index < maxLevelIndex && scrollRect.content.transform.localPosition.y < -midMap.transform.localPosition.y - (3f * mapHeight / 2f) + mapChangeOffset)
            {
                index++;
                moveDest = 1;
            }

            if (index > 0 && moveDest == 0)
            {
                defaultBotPos = botMap.transform.localPosition;
                defaultMidPos = midMap.transform.localPosition;

                tempMap = midMap;
                midMap = botMap;

                if (topMap != null)
                {
                    topMap.SendButtonsBackToPool();
                    PoolManager.instance.ResetPoolItem(topMap.GetComponent<PoolItem>());
                }

                topMap = tempMap;
                botMap = InstantiateMap(index - 1, defaultBotPos.y - mapHeight).GetComponent<Map>();

                midMap.transform.localPosition = defaultBotPos;
                topMap.transform.localPosition = defaultMidPos;
            }
            else if (index == 0 && moveDest == 0)
            {
                defaultBotPos = botMap.transform.localPosition;
                defaultMidPos = midMap.transform.localPosition;

                tempMap = midMap;
                midMap = botMap;

                if (topMap != null)
                {
                    topMap.SendButtonsBackToPool();
                    PoolManager.instance.ResetPoolItem(topMap.GetComponent<PoolItem>());
                }

                botMap = null;
                topMap = tempMap;

                midMap.transform.localPosition = defaultBotPos;
                topMap.transform.localPosition = defaultMidPos;
            }

            else if (index < maxLevelIndex && moveDest == 1)
            {
                defaultMidPos = midMap.transform.localPosition;
                defaultTopPos = topMap.transform.localPosition;

                tempMap = midMap;
                midMap = topMap;

                if (botMap != null)
                {
                    botMap.SendButtonsBackToPool();
                    PoolManager.instance.ResetPoolItem(botMap.GetComponent<PoolItem>());
                }

                botMap = tempMap;
                topMap = InstantiateMap(index + 1, defaultTopPos.y + mapHeight).GetComponent<Map>();

                midMap.transform.localPosition = defaultTopPos;
                botMap.transform.localPosition = defaultMidPos;
            }

            else if (index == maxLevelIndex && moveDest == 1)
            {
                defaultMidPos = midMap.transform.localPosition;
                defaultTopPos = topMap.transform.localPosition;

                tempMap = midMap;
                midMap = topMap;

                if (botMap != null)
                {
                    botMap.SendButtonsBackToPool();
                    PoolManager.instance.ResetPoolItem(botMap.GetComponent<PoolItem>());
                }

                topMap = null;
                botMap = tempMap;

                midMap.transform.localPosition = defaultTopPos;
                botMap.transform.localPosition = defaultMidPos;
            }

            moveDest = -1;
        }
    }
}