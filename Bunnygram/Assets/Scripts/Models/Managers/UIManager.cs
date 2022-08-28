using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Nonogram
{
    public class UIManager : MonoBehaviour
    {
        #region Buttons
        [SerializeField] private Button verticalButton;
        [SerializeField] private Button horizontalButton;
        [SerializeField] private Button bombButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button moveTypeButton;
        #endregion

        [SerializeField] private TextMeshProUGUI textMeshPro;

        private TextMeshProUGUI verticalText;
        private TextMeshProUGUI horizontalText;
        private TextMeshProUGUI bombText;
        private PowerUpController powerUpController;

        #region Canvas Groups
        [SerializeField] private CanvasGroup settingsPanel;
        [SerializeField] private CanvasGroup winPanel;
        [SerializeField] private CanvasGroup losePanel;
        [SerializeField] private CanvasGroup shopPanel;

        [SerializeField] private SortingGroup gridSortinggroup;
        #endregion

        [SerializeField] private Image mainMask;
        [SerializeField] private Image worldMask;
        [SerializeField] private Image everythingMask;

        [SerializeField] private List<Heart> heartList;

        public List<PowerupButton> powerUpButtonList;

        private bool isSettingsPanelOpen = false;
        private bool isBombAnimating = false;

        private void Start()
        {
            powerUpController = GameManager.instance.GetPowerUpController();
            verticalText = verticalButton.GetComponentInChildren<TextMeshProUGUI>();
            horizontalText = horizontalButton.GetComponentInChildren<TextMeshProUGUI>();
            bombText = bombButton.GetComponentInChildren<TextMeshProUGUI>();
            InitializeHearts();
        }

        private void InitializeHearts()
        {
            for (int i = 0; i < GameManager.instance.currentHealth; i++)
            {
                heartList[i].childImage.enabled = true;
            }
        }

        public void KillHeart()
        {
            int index = 0;
            if (GameManager.instance.currentHealth > 0) index = GameManager.instance.currentHealth;
            Heart heart = null;
            heart = heartList[index];
            heart.Die();
        }

        public void SetMaskState(Image mask, bool isActive, Action onClickAction = null)
        {
            if (isActive)
            {
                SetMaskClickAction(mask, onClickAction);
                mask.gameObject.SetActive(true);
                //mask.DOFade(0.5f, 0.15f).From(0f);
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

        public void FadeInWinPanel()
        {
            winPanel.gameObject.SetActive(true);
        }

        public void FadeInLosePanel()
        {
            losePanel.gameObject.SetActive(true);
            losePanel.gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
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
            settingsButton.GetComponent<Canvas>().sortingOrder = 16;
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

        public void ControlPanels(GameState gameState)
        {
            if (gameState.HasFlag(GameState.Lose))
            {
                mainMask.DOFade(0.5f, 0.5f).SetDelay(0.8f).From(0f);
                SetMaskState(mainMask, true);
                Sequence sequence = DOTween.Sequence();
                sequence.PrependInterval(0.8f).OnStepComplete(() => gridSortinggroup.transform.DOScale(Vector3.zero, 0.5f)).OnComplete(() => FadeInLosePanel());
            }
            else if (gameState.HasFlag(GameState.Win))
            {
                mainMask.DOFade(0.5f, 0.5f).SetDelay(0.8f).From(0f);
                SetMaskState(mainMask, true);
                Sequence sequence = DOTween.Sequence();
                sequence.PrependInterval(0.8f).OnStepComplete(() => gridSortinggroup.transform.DOScale(Vector3.zero, 0.5f)).OnComplete(() => FadeInWinPanel());
            }
            else if (gameState.HasFlag(GameState.Shop))
            {
                FadeInShopPanel();
            }
        }

        public void FadeInShopPanel()
        {
            shopPanel.gameObject.SetActive(true);
            SetMaskState(mainMask, true);
        }

        public void PowerUpPanelControl(GameState gameState)
        {
            if (gameState.HasFlag(GameState.PowerUpAnimation))
            {
                isBombAnimating = true;
            }

            else if (isBombAnimating && !gameState.HasFlag(GameState.PowerUpAnimation) && gameState.HasFlag(GameState.Playing))
            {
                worldMask.gameObject.SetActive(false);
                isBombAnimating = false;
            }

            if (gameState.HasFlag(GameState.PowerUp))
            {
                gridSortinggroup.sortingOrder = 13;
                worldMask.gameObject.SetActive(true);
            }
            if (gameState.HasFlag(GameState.Playing) && !gameState.HasFlag(GameState.PowerUp))
            {
                gridSortinggroup.sortingOrder = 10;
                worldMask.gameObject.SetActive(false);
            }
        }

        private void ControlEverthingMask(GameState gameState)
        {
            if (gameState.HasFlag(GameState.PowerUpAnimation))
            {
                everythingMask.gameObject.SetActive(true);
            }
            else
            {
                everythingMask.gameObject.SetActive(false);
            }
        }


        public Image GetMainMask()
        {
            return mainMask;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (GameManager.instance.GetUIManager() == null) GameManager.instance.SetUIManager(this);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            GameManager.instance.OnWrongMove += KillHeart;
            GameManager.instance.OnGameStateChanged += PowerUpPanelControl;
            GameManager.instance.OnGameStateChanged += ControlPanels;
            GameManager.instance.OnGameStateChanged += ControlEverthingMask;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameManager.instance.OnWrongMove -= KillHeart;
            GameManager.instance.OnGameStateChanged -= PowerUpPanelControl;
            GameManager.instance.OnGameStateChanged -= ControlPanels;
            GameManager.instance.OnGameStateChanged -= ControlEverthingMask;
        }
    }
}