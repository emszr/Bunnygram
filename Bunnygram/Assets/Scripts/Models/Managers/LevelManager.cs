using System;
using System.Collections.Generic;
using DG.Tweening;
using Nonogram.System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nonogram
{
    [Flags]
    public enum LevelState
    {
        Completed = 1,
        UnCompleted = 2
    }
    public enum LevelDifficulty
    {
        Easy=5,
        Medium=10,
        Hard=15
    }
    [Serializable]
    public class Level
    {
        public LevelState state;
        public string ID;
        public LevelDifficulty difficulty;
        public int levelCount;
        public int lives;
        public int totalStars;
        public int earnedDiamond;
        public int earnedStars;
        public List<CellData> cellData;

        public Level(LevelState state, int currentLevel, string id, int lives,int totalStars)
        {
            this.state = state;
            this.levelCount = currentLevel;
            ID = id;
            this.lives = lives;
            this.totalStars = totalStars;
        }
        public void SetCellDataList(List<CellData> datas)
        {
            cellData = new List<CellData>(datas);
        }

        public List<CellData> GetCellDataList()
        {
            return cellData;
        }
    }

    public class LevelManager : Singleton<LevelManager>
    {
        public List<Level> dataList;
        public Level currentLevel;
        public float upperFillRate;
        public float lowerFillRate;
        public int tempDiamond;

        private void Start()
        {
            DataLevelList dataLevel = SaveLoadSystem.LoadData();
            if (dataLevel != null)
            {
                dataList = dataLevel.list;
            }
        }

        public void InitilazeLevel()
        {
            GameManager.instance.levelInfo = currentLevel;
            DataLevelList levelData = SaveLoadSystem.LoadData();
            LoadDataList();
            // For the first level
            if (levelData == null||dataList.Count==0)
            {
                SetCurrentLevel(new Level(LevelState.UnCompleted, 1, GenerateLevelID(1), Constants.LIVES,0));
                currentLevel.difficulty = LevelDifficulty.Easy;
                GameManager.instance.SetCurrentHealth(Constants.LIVES);
            }
            else
            {
                GameManager.instance.SetCurrentHealth(currentLevel.lives);
            }
            if (levelData != null && levelData.GetLevels() != null)
            {
                dataList = levelData.GetLevels();

            }
            else
            {
                dataList = new List<Level>();
            }
            bool check = true;
            foreach(Level level in dataList)
            {
                if (level.ID.CompareTo(currentLevel.ID) == 0)
                {
                    check = false;
                }
            }
            if (check)
            {
                dataList.Add(currentLevel);
            }
            GenerateBoundsRate();
        }
        #region Level Savers
        public void SaveLevel()
        {

            
            Level level = dataList.Find(x => x.levelCount == currentLevel.levelCount);
            if (level == null) { return; }

            int lastLives = GameManager.instance.currentHealth;

            //SAVE THE PROPERTIES OF LEVEL DATA EXCEPT(STATE AND EARNED STARS )
            level.ID = currentLevel.ID;
            level.levelCount = currentLevel.levelCount;
            level.lives = GameManager.instance.currentHealth;
            level.difficulty = currentLevel.difficulty;
            //SAVE GRID DATA TO LEVEL DATA
            GameManager.instance.GetGridManager().UpdateCellDatas();
            currentLevel.SetCellDataList(GameManager.instance.GetGridManager().GetCellDataList());
            level.cellData = new List<CellData>(currentLevel.cellData);
            // GAME LOST
            if (!GameManager.instance.currentGameState.HasFlag(GameState.Win))
            {
                level.earnedStars = 0;
                tempDiamond += currentLevel.earnedDiamond;
                if (GameManager.instance.currentGameState.HasFlag(GameState.Lose))
                {
                    level.cellData = new List<CellData>();
                    level.lives = 3;
                }
                level.earnedDiamond = 0;
                SaveLoadSystem.SaveData(this);
                return;
            }

            // IF LEVEL COMPLETED AFTER PLAYING COMPLETED LEVEL => SET STATE COMPLETED AND UNCOMPLETED
            if (currentLevel.state.HasFlag(LevelState.Completed)&&level.state.HasFlag(LevelState.Completed)&&level.totalStars>0)
            {
                tempDiamond = 0;
                level.state = LevelState.Completed | LevelState.UnCompleted;
                int temp= Mathf.Max(level.totalStars, lastLives) - level.totalStars;
                if(temp>0)
                {
                    level.earnedStars = temp;
                }
                level.totalStars = Mathf.Max(level.totalStars,lastLives);
                CurrencyManager.instance.IncreaseCurrencyAmount(CurrencyItemType.Star, temp);
            }
            //IF LEVEL COMPLETED FOR THE FIRST TIME
            else if(currentLevel.state.HasFlag(LevelState.Completed))
            {
                level.earnedDiamond = currentLevel.earnedDiamond;
                level.totalStars = Mathf.Max(level.totalStars, lastLives);
                level.earnedStars = Mathf.Max(level.totalStars, lastLives);
                level.state= currentLevel.state;
                CurrencyManager.instance.IncreaseCurrencyAmount(CurrencyItemType.Diamond,level.earnedDiamond+tempDiamond);
                CurrencyManager.instance.IncreaseCurrencyAmount(CurrencyItemType.Star, level.earnedStars);
                level.earnedDiamond = level.earnedDiamond + tempDiamond;
                tempDiamond = 0;
            }

            SaveLoadSystem.SaveData(this);
        }
        public bool CheckSavedGrid()
        {
            if (currentLevel.cellData != null && currentLevel.cellData.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Level Loaders
        public void LoadDataList()
        {
            if (!PlayerPrefs.HasKey("DataList"))
            {
                dataList = new List<Level>();
            }
            else
            {
                dataList = SaveLoadSystem.LoadData().GetLevels();
            }
        }

        public void LoadNextLevel()
        {
            int nextIndex = currentLevel.levelCount;
            currentLevel = dataList[nextIndex];
            currentLevel.cellData = new List<CellData>();
            currentLevel.lives = Constants.LIVES;
            DOTween.KillAll();
        }

        public void LoadLevelOnMenu(int levelCount)
        {
            Level level = dataList.Find(x => x.levelCount == levelCount + 1);
            ResetCollectedItems(CurrencyItemType.Star);
            ResetCollectedItems(CurrencyItemType.Diamond);
            if (PlayerPrefs.HasKey("DataList") && level != null)
            {
                dataList = SaveLoadSystem.LoadData().list;
                currentLevel = level;
                if (currentLevel.state.HasFlag(LevelState.Completed))
                {
                    currentLevel.lives = Constants.LIVES;
                    currentLevel.cellData = new List<CellData>();
                    currentLevel.state = LevelState.UnCompleted;
                }

            }
            else
            {
                currentLevel = new Level(LevelState.UnCompleted, 1, GenerateLevelID(1), Constants.LIVES,0);
            }
            DOTween.KillAll();
        }
        public void LoadMainMenu()
        {
            DOTween.KillAll();
            SaveLevel();
            PoolManager.instance.ResetAllPools();
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        public void LoadRefereshedLevel()
        {
            currentLevel.cellData = new List<CellData>();
            currentLevel.lives = Constants.LIVES;
            currentLevel.earnedDiamond = 0;
            currentLevel.earnedStars = 0;
        }

        #endregion

        public void GenerateLevelAfterCompleted(GameState state)
        {
            if (state == GameState.Win)
            {
                if (dataList.Find(x => x.levelCount == currentLevel.levelCount + 1) == null)
                {
                    Level level = new Level(LevelState.UnCompleted, currentLevel.levelCount + 1, GenerateLevelID(currentLevel.levelCount + 1), Constants.LIVES, 0);
                    level.difficulty = GenerateDifficulty();
                    dataList.Add(level);
                }
                SaveLevel();
            }
        }

        public bool IsLevelCompleted()
        {
            return currentLevel != null && currentLevel.state == LevelState.Completed;
        }
        public bool IsLevelUnCompleted()
        {
            return currentLevel != null && currentLevel.state == LevelState.UnCompleted;
        }

        public Level GetCurrentLevel()
        {
            return currentLevel;
        }
        public void SetCurrentLevel(Level level)
        {
            this.currentLevel = level;
        }
        public void ChangeLevelState(GameState state)
        {
            if (state == GameState.Win)
            {
                currentLevel.state = LevelState.Completed;
            }
            else if (state == GameState.Lose)
            {
                currentLevel.state = LevelState.UnCompleted;
            }
        }

        public void SetLevelItems(LevelState state, int currentLevel, string id, int lives)
        {
            this.currentLevel.state = state;
            this.currentLevel.levelCount = currentLevel;
            this.currentLevel.ID = id;
            this.currentLevel.lives = lives;
        }

        public LevelDifficulty GenerateDifficulty()
        {
            int levelDiff = currentLevel.levelCount % 5;
            /*
            if (currentLevel.levelCount % 10 == 0)
            {
                return LevelDifficulty.Hard;
            }
            */
            if (levelDiff < 3)
            {
                return LevelDifficulty.Easy;
            }
            else if(levelDiff<=5)
            {
                return LevelDifficulty.Medium;
            }

            return LevelDifficulty.Easy;
        }

        public string GenerateLevelID(int count)
        {
            string id = "Level_" + count.ToString();
            return id;
        }

        public void GenerateBoundsRate()
        {
            if(currentLevel.difficulty== LevelDifficulty.Easy)
            {
                lowerFillRate = 0.45f;
                upperFillRate = 0.8f;
            }
            else if(currentLevel.difficulty== LevelDifficulty.Medium)
            {
                lowerFillRate = 0.65f;
                upperFillRate = 0.80f;
            }
            else
            {
                lowerFillRate = 0.6f;
                upperFillRate = 0.75f;
            }
        }

        public Level GetLastItem()
        {
            if (dataList.Count == 0) { return null; }
            return dataList[dataList.Count - 1];
        }

        public int GetCollectedItems(CurrencyItemType type)
        {
            int temp = 0;
            foreach (Level level in dataList)
            {
                if (type == CurrencyItemType.Diamond)
                {
                    temp += level.earnedDiamond;
                }
                if (type == CurrencyItemType.Star)
                {
                    temp += level.earnedStars;
                }
            }
            return temp;
        }

        public void ResetCollectedItems(CurrencyItemType type)
        {
            foreach (Level level in dataList)
            {
                if (type == CurrencyItemType.Diamond)
                {
                    level.earnedDiamond=0;
                }
                if (type == CurrencyItemType.Star)
                {
                    level.earnedStars = 0;
                }
            }
            SaveLoadSystem.SaveData(this);
        }

        private void OnApplicationQuit()
        {
            ResetCollectedItems(CurrencyItemType.Star);
            ResetCollectedItems(CurrencyItemType.Diamond);
            SaveLevel();
        }
        private void OnEnable()
        {
            GameManager.instance.OnGameStateChanged += ChangeLevelState;
            GameManager.instance.OnGameStateChanged += GenerateLevelAfterCompleted;
        }
        private void OnDisable()
        {
            GameManager.instance.OnGameStateChanged -= ChangeLevelState;
            GameManager.instance.OnGameStateChanged -= GenerateLevelAfterCompleted;
        }
    }
}




