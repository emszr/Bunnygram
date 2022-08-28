using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Nonogram.System;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace Nonogram
{
    public class MainMenuManager : MonoBehaviour
    {
        public List<LevelButton> levelButtonList = new List<LevelButton>();
        [SerializeField] private PlayAgainPanel panel;
        [SerializeField] private Button settingsButton;
        [SerializeField] private CanvasGroup settingsPanel;
        [SerializeField] private Image mainMask;
        [SerializeField] private ItemCollector collector;

        public Sprite unLockedSprite;
        public Sprite lockedSprite;
        private LevelButton tempLevelButton;
        private bool isSettingsPanelOpen = false;
        public bool IsDiamondShowed = false;
        public bool IsStarShowed = false;



        public void InitializeButton(List<PoolItem> poolItemList, int previousCount)
        {
            levelButtonList.Clear();




            foreach (PoolItem poolItem in poolItemList)
            {
                tempLevelButton = poolItem.GetComponent<LevelButton>();
                tempLevelButton.levelButtonUnLocked = false;
                tempLevelButton.image.sprite = lockedSprite;
                ClearStars(tempLevelButton);
                levelButtonList.Add(tempLevelButton);
            }

            if ((LevelManager.instance.dataList == null || LevelManager.instance.dataList.Count == 0))
            {
                if(previousCount == 0)
                {
                    levelButtonList[0].levelButtonUnLocked = true;
                    levelButtonList[0].image.sprite = unLockedSprite;
                }
            }

            else
            {
                int sizeDataList = LevelManager.instance.dataList.Count;

                for(int index = 0; index < levelButtonList.Count; index++)
                {
                    levelButtonList[index].indexLevel = index + previousCount;

                    if((index + previousCount) < sizeDataList)
                    {
                        levelButtonList[index].levelButtonUnLocked = true;
                        levelButtonList[index].image.sprite = unLockedSprite;
                        SetStarsOnButton(levelButtonList[index], LevelManager.instance.dataList);
                    }
                    if ((index + previousCount) == sizeDataList - 1)
                    {
                        DisplayDiamonds();
                        DisplayAnimatedStars();
                    }
                }
            }

            int index2 = 0;
            foreach (LevelButton levelButton in levelButtonList)
            {  
                levelButton.button.onClick.AddListener(() => LoadPressedLevel(levelButton, LevelManager.instance.dataList));
                levelButton.textMeshPro.text= (index2 + 1 + previousCount).ToString();
                index2++;
            }
        }

        private void ClearStars(LevelButton levelButton)
        {
            foreach(Image image in levelButton.starList)
            {
                image.gameObject.SetActive(false);
            }
        }

        public void DisplayDiamonds()
        {
            if (IsDiamondShowed)
            {
                return;
            }
            if (LevelManager.instance.GetCollectedItems(CurrencyItemType.Diamond) != 0&&LevelManager.instance.currentLevel.ID.CompareTo("")!=0)
            {
                if (LevelManager.instance.currentLevel.state.HasFlag(LevelState.Completed))
                {
                    collector.RevealDiamonds(levelButtonList.Find(x => x.indexLevel == LevelManager.instance.currentLevel.levelCount - 1)
                    .transform);
                }
                else if(0<LevelManager.instance.currentLevel.levelCount - 1)
                {
                    collector.RevealDiamonds(levelButtonList.Find(x => x.indexLevel == LevelManager.instance.currentLevel.levelCount - 2)
                    .transform);
                }
           
                IsDiamondShowed = true;
            }
        }

        public void DisplayAnimatedStars()
        {
            if (IsStarShowed)
            {
                return;
            }
            if (LevelManager.instance.currentLevel.ID.CompareTo("") != 0 && LevelManager.instance.GetCollectedItems(CurrencyItemType.Star) != 0)
            {
                if (LevelManager.instance.currentLevel.state.HasFlag(LevelState.Completed))
                {
                    collector.RevealStars(levelButtonList.Find(x => x.indexLevel == LevelManager.instance.currentLevel.levelCount - 1).transform);
                }
                else if (0 < LevelManager.instance.currentLevel.levelCount - 1)
                {
                    collector.RevealStars(levelButtonList.Find(x => x.indexLevel == LevelManager.instance.currentLevel.levelCount - 2).transform);
                }
                IsStarShowed = true;
            }
        }

        public void LoadPressedLevel(LevelButton levelButton, List<Level> list)
        {
            if (levelButton.levelButtonUnLocked)
            {
                if (list == null||list.Count==0)
                {
                    GameManager.instance.LoadLevelSceneFromMenu(levelButton.indexLevel);
                }
                else 
                {
                    if (levelButton.indexLevel < list.Count)
                    {
                        if (list[levelButton.indexLevel].state.HasFlag(LevelState.Completed))
                        {
                            SoundManager.instance.PlaySound(SoundType.ButtonClick);
                            panel.DisplayFrame(list[levelButton.indexLevel].totalStars, levelButton.indexLevel);
                        }
                        else
                        {
                            GameManager.instance.LoadLevelSceneFromMenu(levelButton.indexLevel);
                        }
                    }
                }
                
            }
            else
            {
                Debug.Log("Level is locked!");
            }
        }


        public void SetStarsOnButton(LevelButton levelButton,List<Level> list)
        {
            if (list==null||levelButton.indexLevel >= list.Count) { return; }

            Level level = list.Find(x => x.levelCount == (levelButton.indexLevel+1));
            if (level == null)
            {
                return;
            }
            if (level.state.HasFlag(LevelState.Completed))
            {
                for (int i = 0; i < level.totalStars;i++)
                {
                    levelButton.starList[i].gameObject.SetActive(true);
                }

            }
        }
        public void ControlSettingsPanel()
        {
            if (!isSettingsPanelOpen)
            {
                SoundManager.instance.PlaySound(SoundType.ButtonClick);
                FadeInSettingsPanel();
            }
            else
            {
                SoundManager.instance.PlaySound(SoundType.ButtoUnClick);
                FadeOutSettingsPanel();
            }
        }

        public void FadeInSettingsPanel()
        {
            settingsButton.GetComponent<Canvas>().sortingOrder = 13;
            settingsButton.transform.GetChild(0).DORotate(new Vector3(0, 0, -90f), 0.2f);
            isSettingsPanelOpen = true;
            DOTween.Kill(settingsPanel.transform);
            SetMaskState(mainMask, true, FadeOutSettingsPanel);
            settingsPanel.gameObject.SetActive(true);
            GameManager.instance.ChangeGameState(GameState.Pause);
            settingsPanel.transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutBack, 4f).From(0f);
        }

        public void FadeOutSettingsPanel()
        {
            settingsButton.GetComponent<Canvas>().sortingOrder = 10;
            settingsButton.transform.GetChild(0).DORotate(Vector3.zero, 0.2f);
            isSettingsPanelOpen = false;
            DOTween.Kill(settingsPanel.transform);
            GameManager.instance.ChangeGameState(GameState.Playing);
            settingsPanel.transform.DOScaleY(0f, 0.3f).OnComplete(() => settingsPanel.gameObject.SetActive(true));
            SetMaskState(mainMask, false);
        }
        public void SetMaskState(Image mask, bool isActive, Action onClickAction = null)
        {
            if (isActive)
            {
                SetMaskClickAction(mask, onClickAction);
                mask.gameObject.SetActive(true);
                mask.DOFade(0.5f, 0.15f).From(0f);
            }
            else
            {
                mask.gameObject.SetActive(false);
            }
        }

        private void SetMaskClickAction(Image mask, Action action)
        {
            EventTrigger trigger = mask.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            trigger.triggers.Clear();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { action?.Invoke(); });
            trigger.triggers.Add(entry);
        }
    }
}
