using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

namespace Nonogram
{
    public class GameManager : Singleton<GameManager>
    {
        #region Serialized Field
        [SerializeField]
        private GridManager gridManager;
        [SerializeField]
        private InputManager inputManager;
        [SerializeField]
        private PowerUpController powerUpController;
        [SerializeField]
        private UIManager uIManager;
        [SerializeField]
        private MapParent mapParent;
        [SerializeField]
        private GridBuilder gridBuilder;
        #endregion

        public int currentHealth;
        public GameState currentGameState;
        public Level levelInfo;
        public List<Map> mapPrefabList;

        #region Seed
        public UnityEngine.Random.State seedGenerator;
        #endregion

        #region Action
        public Action<GameState> OnGameStateChanged;
        public Action<CurrencyItemType> OnPowerButtonPushed;
        public Action<CurrencyItemType> OnShopPanelChanged;
        public Action<Cell> OnUsePowerUp;
        public Action<Cell,CellState> OnCellDown;
        public Action OnBombAnimationEnded;
        public Action OnCellUp;
        public Action OnWrongMove;
        public Action OnMoveTypeChanged;
        #endregion

        public int GetMainMenuMapIndex(int levelIndex)
        {
            int index = -1;
            int tempLevelIndex = levelIndex;

            if (tempLevelIndex < 0)
            {
                return index;
            }

            while (tempLevelIndex >= 0 && index < mapPrefabList.Count)
            {
                tempLevelIndex -= mapPrefabList[index + 1].levelButtonPosList.Count;
                index++;
            }

            return index;
        }

        public int GetGameplayBackgroundIndex(int mapIndex)
        {
            PoolItem tempPoolItem = mapPrefabList[mapIndex].GetComponent<PoolItem>();

            int index = -1;

            if (tempPoolItem.GetPoolItemType() == PoolItemType.BeachMap || tempPoolItem.GetPoolItemType() == PoolItemType.BeachMap2)
            {
                index = 0;
            }

            else if (tempPoolItem.GetPoolItemType() == PoolItemType.ForestMap)
            {
                index = 1;
            }

            else if (tempPoolItem.GetPoolItemType() == PoolItemType.WinterMap)
            {
                index = 2;
            }

            return index;
        }

        public void ChangeGameState(GameState state)
        {
            if (currentGameState != state)
            {
                currentGameState = state;
                OnGameStateChanged?.Invoke(state);
            }
        }

        public void RefreshLevel()
        {
            currentGameState = GameState.Playing;
        }

        public void DecreaseHealth()
        {
            LevelManager.instance.currentLevel.lives--;
            SetCurrentHealth(GetCurrentHealth() - 1);
            if (GetCurrentHealth() <= 0) Die();
        }

        private void Die()
        {
            ChangeGameState(GameState.Lose);
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public void SetCurrentHealth(int currentHealth)
        {
            this.currentHealth = currentHealth;
        }

        public MapParent GetMapParent()
        {
            return mapParent;
        }
        public void SetMapParent(MapParent mapParent)
        {
            this.mapParent = mapParent;
        }

        public GridManager GetGridManager()
        {
            return gridManager;
        }
        public void SetGridManager(GridManager manager)
        {
            this.gridManager = manager;
        }

        public GridBuilder GetGridBuilder()
        {
            return gridBuilder;
        }
        public void SetGridBuilder(GridBuilder builder)
        {
            gridBuilder = builder;
        }
        public UIManager GetUIManager()
        {
            return uIManager;
        }
        public void SetUIManager(UIManager manager)
        {
            this.uIManager = manager;
        }

        public void SetInputManager(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        public InputManager GetInputManager()
        {
            return inputManager;
        }
        public void SetPowerUpController(PowerUpController powerUpController)
        {
            this.powerUpController = powerUpController;
        }
        public PowerUpController GetPowerUpController()
        {
            return powerUpController;
        }

        public void LoadNextLevelScene()
        {
            PoolManager.instance.ResetAllPools();
            LevelManager.instance.LoadNextLevel();
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        public void LoadLevelSceneFromMenu(int levelCount)
        {
            PoolManager.instance.ResetAllPools();
            LevelManager.instance.LoadLevelOnMenu(levelCount);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
  
        private void OnEnable()
        {
            OnWrongMove += DecreaseHealth;
        }

        private void OnDisable()
        {
            OnWrongMove -= DecreaseHealth;
        }
    }
}





