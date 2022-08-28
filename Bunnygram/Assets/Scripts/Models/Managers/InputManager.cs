using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nonogram
{
    public class InputManager : MonoBehaviour
    {
        private Vector3 position;
        private Camera mainCamera;
        private RaycastHit2D[] hit;

        private int hit_count;
        private bool checkResume = true;
        
        private CellState currentMoveType = CellState.Filled;
        public bool onPowerUpMode = false;

        private void Awake()
        {
            mainCamera = Camera.main;
            hit = new RaycastHit2D[1];
        }
        private void Update()
        {
            GenerateInput();
        }
        private void GenerateInput()
        {

            if (!checkResume)
            {
                return;
            }
           
            if (Input.GetMouseButton(0))
            {
                position = Input.mousePosition;
                position.z = mainCamera.transform.position.z;
                GameObject hitted_object = GetHittedObject();
                if (hitted_object == null) { return; }
                if(hitted_object.CompareTag("Cell"))
                {
                    Cell cell = hitted_object.GetComponent<Cell>();
                    bool cellValidation = ValidateCell(cell);
                    bool checkPowerUp = CheckPowerUp(cellValidation,cell);
                    if (checkPowerUp&&GameManager.instance.OnUsePowerUp!=null)
                    {
                        GameManager.instance.OnUsePowerUp(cell);
                        return;
                    }
                    if (cellValidation&& GameManager.instance.OnCellDown!=null)
                    {
                        GameManager.instance.OnCellDown(cell,currentMoveType);
                    }
                }
                
            }
            else if (Input.GetMouseButtonUp(0)&&GameManager.instance.OnCellUp!=null)
            {
                GameManager.instance.OnCellUp();
            }
        }
        private GameObject GetHittedObject()
        {
            hit_count = Physics2D.RaycastNonAlloc(mainCamera.ScreenToWorldPoint(position), Vector2.zero, hit);

            if (hit_count>0 && hit[0].collider != null)
            {
                return hit[0].collider.gameObject;
            }
            return null;
        }

        private bool ValidateCell(Cell cell)
        {
            if (cell.isLocked == false) return true;
            return false;
        }

        private bool CheckPowerUp(bool validation,Cell cell)
        {
            if (onPowerUpMode&&validation &&
                GameManager.instance.GetPowerUpController()
                .GetPowerUp() != null)
            {
                return true;
            }
            return false;
        }
        public void ChangeMoveType()
        {
            int tempState = (int)currentMoveType;
            tempState = (tempState + 1) % 2;
            currentMoveType = (CellState)tempState;
        }
        private void ValidateInput(GameState gameState)
        {
            if (gameState.HasFlag(GameState.Playing)&&!gameState.HasFlag(GameState.PowerUpAnimation))
            {
                if(gameState.HasFlag(GameState.PowerUp))
                {
                    onPowerUpMode = true;
                }
                else
                {
                    onPowerUpMode = false;
                }

                checkResume = true;
            }
            else checkResume = false;

        }

        public CellState GetCurrentMove()
        {
            return currentMoveType;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (GameManager.instance.GetInputManager() == null) GameManager.instance.SetInputManager(this);
        }

        public void PowerUpModeCheck(CurrencyItemType currencyItem)
        {
            onPowerUpMode = !onPowerUpMode;
        }
        
        private void OnEnable()
        {
            GameManager.instance.OnMoveTypeChanged += ChangeMoveType;
            GameManager.instance.OnGameStateChanged += ValidateInput;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            GameManager.instance.OnMoveTypeChanged -= ChangeMoveType;
            GameManager.instance.OnGameStateChanged -= ValidateInput;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
