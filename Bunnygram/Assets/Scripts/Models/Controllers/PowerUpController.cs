using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

namespace Nonogram
{
    public class PowerUpController : MonoBehaviour
    {
        #region PowerUps
        public BombPowerUp bombPowerUp;
        public VerticalRocket verticalRocket;
        public HorizontalRocket horizontalRocket;
        #endregion

        public PowerUp currentPowerUp;
        public PowerUp inShopPowerUp;
        private CurrencyItemType typeOfPowerUp;

        [SerializeField] private Transform verticalPowerupStart;
        [SerializeField] private Transform verticalPowerupEnd;
        [SerializeField] private Transform horizontalPowerupStart;
        [SerializeField] private Transform horizontalPowerupEnd;

        [SerializeField] private Transform verticalPowerupPrefab;
        [SerializeField] private Transform horizontalPowerupPrefab;
        [SerializeField] private Transform bombPowerupPrefab;

        private PoolItem poolItem;
        private Cell cell;
        private bool bombBooming = false;

        private void Awake()
        {
            currentPowerUp = null;
            bombPowerUp = ScriptableObject.CreateInstance("BombPowerUp") as BombPowerUp;
            verticalRocket = ScriptableObject.CreateInstance("VerticalRocket") as VerticalRocket;
            horizontalRocket = ScriptableObject.CreateInstance("HorizontalRocket") as HorizontalRocket;
        }

        private void PlayVerticalPowerupAnimation(Cell cell)
        {
            int columnIndex = cell.GetColumnIndex();

            GameManager.instance.ChangeGameState(GameState.Playing | GameState.PowerUpAnimation);

            poolItem = PoolManager.instance.GetFromPool(PoolItemType.VerticalPowerup, null);

            if (poolItem == null)
            {
                poolItem = Instantiate(PoolManager.instance.prefabList[(int)PoolItemType.VerticalPowerup].GetComponent<PoolItem>());
                PoolManager.instance.AddToUsing(poolItem, null);
            }

            List<Cell> tempList = GameManager.instance.GetGridManager().GetColumn(columnIndex);

            float gridSize = GameManager.instance.GetGridManager().GetGridSize();
            float scaleMultiplier = Constants.DEFAULTPOWERUPSIZE / gridSize;
            float newScale = verticalPowerupPrefab.transform.localScale.x * scaleMultiplier;

            poolItem.transform.localScale = new Vector3(newScale, newScale);
            poolItem.transform.position = new Vector3(tempList[tempList.Count - 1].transform.position.x, verticalPowerupStart.position.y);
            SoundManager.instance.PlaySound(SoundType.Vertical);
            poolItem.transform.DOMoveY(verticalPowerupEnd.position.y, 1.1f).From(poolItem.transform.position.y)
                .OnComplete(() => ResetPowerup(poolItem));
        }

        private void PlayHorizontalPowerupAnimation(Cell cell)
        {
            int rowIndex = cell.GetRowIndex();

            GameManager.instance.ChangeGameState(GameState.Playing | GameState.PowerUpAnimation);

            poolItem = PoolManager.instance.GetFromPool(PoolItemType.HorizontalPowerup, null);

            if (poolItem == null)
            {
                poolItem = Instantiate(PoolManager.instance.prefabList[(int)PoolItemType.HorizontalPowerup].GetComponent<PoolItem>());
                PoolManager.instance.AddToUsing(poolItem, null);
            }

            List<Cell> tempList = GameManager.instance.GetGridManager().GetRow(rowIndex);

            float gridSize = GameManager.instance.GetGridManager().GetGridSize();
            float scaleMultiplier = Constants.DEFAULTPOWERUPSIZE / gridSize;
            float newScale = horizontalPowerupPrefab.transform.localScale.x * scaleMultiplier;

            poolItem.transform.localScale = new Vector3(newScale, newScale);
            SoundManager.instance.PlaySound(SoundType.Horizontal);
            poolItem.transform.position = new Vector3(horizontalPowerupStart.position.x, tempList[tempList.Count - 1].transform.position.y);
            poolItem.transform.DOMoveX(horizontalPowerupEnd.position.x, 1.1f).From(poolItem.transform.position.x)
                .OnComplete(() => ResetPowerup(poolItem));
        }

        private void ResetPowerup(PoolItem poolItem)
        {
            GameManager.instance.ChangeGameState(GameState.Playing);
            PoolManager.instance.ResetPoolItem(poolItem);
        }

        private void PlayBombPowerupAnimation(Cell cell)
        {
            if ((currentPowerUp == null || cell == null) || bombBooming)
            {
                return;
            }

            GameManager.instance.ChangeGameState(GameState.Playing | GameState.PowerUpAnimation);

            poolItem = PoolManager.instance.GetFromPool(PoolItemType.BombPowerup, null);

            if (poolItem == null)
            {
                poolItem = Instantiate(PoolManager.instance.prefabList[(int)PoolItemType.BombPowerup].GetComponent<PoolItem>());
                PoolManager.instance.AddToUsing(poolItem, null);
            }

            bombBooming = true;

            float gridSize = GameManager.instance.GetGridManager().GetGridSize();
            float scaleMultiplier = Constants.DEFAULTPOWERUPSIZE / gridSize;
            float newScale = bombPowerupPrefab.transform.localScale.x * scaleMultiplier;

            poolItem.transform.localScale = new Vector3(newScale, newScale);
            poolItem.transform.position = cell.transform.position;
            ParticleSystem particleSystem = poolItem.transform.GetChild(0).GetComponentInChildren<ParticleSystem>();

            IEnumerator coroutine;
            float duration = particleSystem.duration;
            coroutine = ResetBomb(poolItem, duration);

            StartCoroutine(coroutine);
        }

        public void OpenBombCells()
        {
            SoundManager.instance.PlaySound(SoundType.Bomb);
            currentPowerUp.Use(GameManager.instance.GetGridManager().
                        GetNearbyCells(cell));

            currentPowerUp.canBeUsed = false;
            GameManager.instance.GetInputManager().onPowerUpMode = false;
            SetPowerUp(null);
        }

        private IEnumerator ResetBomb(PoolItem poolItem, float lifeTime)
        {
            yield return new WaitForSeconds(lifeTime);
            bombBooming = false;
            GameManager.instance.ChangeGameState(GameState.Playing);
            PoolManager.instance.ResetPoolItem(poolItem);
        }


        public void UsePowerUpOnCell(Cell cell)
        {
            this.cell = cell;
            bool used = false;
            CurrencyItemType type = CurrencyItemType.None;
            if (cell == null) { return; }
            if (currentPowerUp.canBeUsed)
            {
                if (currentPowerUp is VerticalRocket
                    && CurrencyManager.instance.GetItemCount(CurrencyItemType.VerticalRocket) > 0)
                {
                    //currentPowerUp.Use(GameManager.instance.GetGridManager().GetColumn(cell.GetColumnIndex()));
                    used = true;
                    type = CurrencyItemType.VerticalRocket;
                    PlayVerticalPowerupAnimation(cell);

                    currentPowerUp.canBeUsed = false;
                    GameManager.instance.GetInputManager().onPowerUpMode = false;
                    SetPowerUp(null);
                }
                else if (currentPowerUp is HorizontalRocket
                    && CurrencyManager.instance.GetItemCount(CurrencyItemType.HorizontalRocket) > 0)
                {
                    used = true;
                    type = CurrencyItemType.HorizontalRocket;

                    PlayHorizontalPowerupAnimation(cell);

                    currentPowerUp.canBeUsed = false;
                    GameManager.instance.GetInputManager().onPowerUpMode = false;
                    SetPowerUp(null);

                }
                else if (currentPowerUp is BombPowerUp
                         && CurrencyManager.instance.GetItemCount(CurrencyItemType.Bomb) > 0)
                {
                    used = true;
                    type = CurrencyItemType.Bomb;

                    PlayBombPowerupAnimation(cell);
                }

                if (used)
                {
                    CurrencyManager.instance.TryToDecreaseCurrencyAmount(type, 1, 0f);
                }

            }
        }

        public void ActivatePowerUpMode(CurrencyItemType currencyItemType)
        {
            if (currencyItemType == CurrencyItemType.VerticalRocket)
            {
                currentPowerUp = verticalRocket;
            }
            else if (currencyItemType == CurrencyItemType.HorizontalRocket)
            {
                currentPowerUp = horizontalRocket;
            }
            else if (currencyItemType == CurrencyItemType.Bomb)
            {
                currentPowerUp = bombPowerUp;
            }

            currentPowerUp.canBeUsed = true;
        }

        public void DeactivatePowerUpMode()
        {
            currentPowerUp.canBeUsed = false;
            currentPowerUp = null;
        }

        public void PowerUpActivationController(CurrencyItemType currencyItemType)
        {
            if (currentPowerUp != null && typeOfPowerUp == currencyItemType)
            {
                DeactivatePowerUpMode();
            }
            else
            {
                ActivatePowerUpMode(currencyItemType);
                typeOfPowerUp = currencyItemType;
            }
        }

        public void ShopActivationController(CurrencyItemType currencyItemType)
        {
            if (currencyItemType != CurrencyItemType.None && inShopPowerUp == null)
            {
                AssignShopPowerUpMode(currencyItemType);
            }
            else
            {
                inShopPowerUp = null;
            }
        }
        public void AssignShopPowerUpMode(CurrencyItemType currencyItemType)
        {
            if (currencyItemType == CurrencyItemType.VerticalRocket)
            {
                inShopPowerUp = verticalRocket;
            }
            else if (currencyItemType == CurrencyItemType.HorizontalRocket)
            {
                inShopPowerUp = horizontalRocket;
            }
            else if (currencyItemType == CurrencyItemType.Bomb)
            {
                inShopPowerUp = bombPowerUp;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (GameManager.instance.GetPowerUpController() == null) GameManager.instance.SetPowerUpController(this);
        }

        public PowerUp GetPowerUp()
        {
            return currentPowerUp;
        }
        public void SetPowerUp(PowerUp powerUp)
        {
            this.currentPowerUp = powerUp;
        }

        public void SetPowerUp(CurrencyItemType currencyItemType)
        {
            if (currencyItemType == CurrencyItemType.VerticalRocket)
            {
                currentPowerUp = verticalRocket;
            }
            else if (currencyItemType == CurrencyItemType.HorizontalRocket)
            {
                currentPowerUp = horizontalRocket;
            }
            else if (currencyItemType == CurrencyItemType.Bomb)
            {
                currentPowerUp = bombPowerUp;
            }
        }

        public PowerUp GetShopPowerUp()
        {
            return inShopPowerUp;
        }
        public void SetShopPowerUp(PowerUp powerUp)
        {
            this.inShopPowerUp = powerUp;
        }

        private void OnEnable()
        {
            GameManager.instance.OnUsePowerUp += UsePowerUpOnCell;
            GameManager.instance.OnPowerButtonPushed += PowerUpActivationController;
            GameManager.instance.OnBombAnimationEnded += OpenBombCells;
            GameManager.instance.OnShopPanelChanged += ShopActivationController;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            GameManager.instance.OnUsePowerUp -= UsePowerUpOnCell;
            GameManager.instance.OnPowerButtonPushed -= PowerUpActivationController;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameManager.instance.OnShopPanelChanged -= ShopActivationController;
            GameManager.instance.OnBombAnimationEnded -= OpenBombCells;
        }
    }
}