using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Nonogram
{

    public class GridManager : MonoBehaviour
    {
        public List<Cell> gridList;
        public List<int> infoList;
        public List<TextMeshPro> textInfoList;
        public List<CellData> cellDataList;
        public List<Cell> nearbyCells;

        [SerializeField] private Sprite[] backgroundSpriteArray;
        [SerializeField] private SpriteRenderer backgroundSpriteRenderer;

        [SerializeField] private int gridSize;
        private List<Cell> tempList = new List<Cell>();
        private Cell previousCell;
        private CellMovement movement = CellMovement.SingleMove;

        #region Serialized Field
        [SerializeField] private GridBuilder gridBuilder;
        #endregion
        private void Start()
        {
            LevelManager.instance.InitilazeLevel();

            int currentLevelIndex = LevelManager.instance.currentLevel.levelCount - 1;
            int mainMenuMapIndex = GameManager.instance.GetMainMenuMapIndex(currentLevelIndex);
            int backgroundSpriteIndex = GameManager.instance.GetGameplayBackgroundIndex(mainMenuMapIndex);

            backgroundSpriteRenderer.sprite = backgroundSpriteArray[backgroundSpriteIndex];

            InitilazeGrid(
                (int)LevelManager.instance.currentLevel.difficulty
                ,LevelManager.instance.upperFillRate,
                LevelManager.instance.lowerFillRate
                );
        }

        public void InitilazeGrid(int size,float upper,float lower)
        {
            gridSize = size;
            GameManager.instance.RefreshLevel();
            infoList = new List<int>(gridSize);
            nearbyCells = new List<Cell>();
            gridBuilder.InitiliazeBuilder();
            bool loadData = LevelManager.instance.CheckSavedGrid();
            if (!loadData)
            {
                gridList = gridBuilder.GenerateGrids(gridSize,upper,lower);
                cellDataList=CreateCellDataList();
            }
            else
            {
                gridList = gridBuilder.GenerateFromData(LevelManager.instance.currentLevel.cellData, gridSize);
            }
            LevelManager.instance.currentLevel.SetCellDataList(cellDataList);
            textInfoList = gridBuilder.GenerateInfoGrids(gridSize);
            AssignInfoGrids();
            gridBuilder.ScaleGrid();
            CheckFadeAllText();
        }
        public void CheckFadeAllText()
        {
            foreach (Cell cell in gridList)
            {
                FadeInfoText(GetColumn(cell.GetColumnIndex()), cell, CheckType.ColCheck);
                FadeInfoText(GetRow(cell.GetRowIndex()), cell, CheckType.RowCheck);
            }
        }
        public void ActivateCell(Cell cell,CellState currentMoveType)
        {
            if (cell == null)
            {
                return;
            }
            if (GetMovement(cell, previousCell) == CellMovement.UnplayableMove)
            {
                return;
            }
            if (movement == CellMovement.SingleMove)
            {
                movement = GetMovement(cell, previousCell);
            }
            if (GetMovement(cell, previousCell) != movement)
            {
                return;
            }

            // If Cell has different CellState
            if (currentMoveType != cell.cellState)
            {
                LevelManager.instance.currentLevel.earnedDiamond -= 10;
                SoundManager.instance.PlaySound(SoundType.ErrorClick);
                GameManager.instance.OnWrongMove?.Invoke();
            }

            else
            {
                LevelManager.instance.currentLevel.earnedDiamond += 20;
                SoundManager.instance.PlaySound(SoundType.CellClick);
             
            }
            // Unlock cell for both state
            cell.Open();
            previousCell = cell;
            ValidateWin(cell);

        }

        public void RefereshMovement()
        {
            previousCell = null;
            movement = CellMovement.SingleMove;
        }

        public List<int> InfoNumbers(List<Cell> tempList)
        {
            infoList.Clear();
            int counter = 0;

            foreach (Cell cell in tempList)
            {
                if (cell.cellState == CellState.Filled)
                {
                    counter++;
                }
                else
                {
                    if (counter != 0)
                    {
                        infoList.Add(counter);
                        counter = 0;
                    }
                }
            }
            if (counter != 0)
            {
                infoList.Add(counter);
            }
            return infoList;
        }

        private void AssignInfoGrids()
        {
            //first n gridsize elemnts mean columns and rest of the list means rows
            int size = gridSize * 2;
            for (int index = 0; index < size; index++)
            {
                if (index < gridSize)// COLUMN
                {
                    infoList = InfoNumbers(GetColumn(index));
                    foreach (int element in infoList)
                    {
                        textInfoList[index].text += element + "\n";
                    }
                }
                else
                {
                    infoList = InfoNumbers(GetRow(index % gridSize));
                    foreach (int element in infoList)
                    {
                        textInfoList[index].text += element + " ";
                    }
                }
            }
        }
        private CellMovement GetMovement(Cell currentCell, Cell previousCell)
        {
            if (previousCell == null)
            {
                return CellMovement.SingleMove;
            }
            else if (previousCell == currentCell)
            {
                return CellMovement.UnplayableMove;
            }
            else if (currentCell.GetColumnIndex() == previousCell.GetColumnIndex())
            {
                return CellMovement.VerticalMove;
            }
            else if (currentCell.GetRowIndex() == previousCell.GetRowIndex())
            {
                return CellMovement.HorizontalMove;
            }
            else
            {
                return CellMovement.UnplayableMove;
            }
        }

        public void ValidateWin(Cell cell)
        {
            if (CheckGridFilled(gridList, cell) && GameManager.instance.GetCurrentHealth() > 0)
            {
                GameManager.instance.ChangeGameState(GameState.Win);
                //ConvertLine(gridList);
            }
        }
        public bool CheckGridFilled(List<Cell> list, Cell cell)
        {

            bool rowCheck = CheckRowFilled(cell.GetRowIndex());
            bool colCheck = CheckColFilled(cell.GetColumnIndex());

            if (rowCheck)
            {
                ConvertLine(GetRow(cell.GetRowIndex()), cell, CheckType.RowCheck);
            }

            if (colCheck)
            {
                ConvertLine(GetColumn(cell.GetColumnIndex()), cell, CheckType.ColCheck);
            }


            FadeInfoText(GetColumn(cell.GetColumnIndex()), cell, CheckType.ColCheck);
            FadeInfoText(GetRow(cell.GetRowIndex()), cell, CheckType.RowCheck);

            bool check = true;
            if (rowCheck && colCheck)
            {

                foreach (Cell tempCell in list)
                {
                    if (tempCell.cellState == CellState.Filled)
                    {
                        check = (tempCell.isLocked && check);
                    }
                }
                return check;
            }
            else
            {
                return false;
            }
        }


        public List<Cell> GetColumn(int index)
        {
            tempList.Clear();
            for (int i = 0; i < gridSize; i++)
            {
                tempList.Add(gridList[(gridSize * i) + index]);
            }
            return tempList;
        }

        public List<Cell> GetRow(int index)
        {
            tempList.Clear();
            for (int i = 0; i < gridSize; i++)
            {
                tempList.Add(gridList[(index * gridSize) + i]);
            }
            return tempList;
        }

        public bool CheckRowFilled(int row_index)
        {
            bool check = true;
            int counter = 0;
            for (int index = 0; index < GetRow(row_index).Count; index++)
            {
                if (GetRow(row_index)[index].cellState == CellState.Filled)
                {
                    check = (check && GetRow(row_index)[index].isLocked);
                    counter++;
                }
            }
            if (counter == 0) check = false;
            return check;
        }

        public bool CheckColFilled(int col_index)
        {
            bool check = true;
            int counter = 0;
            for (int index = 0; index < GetColumn(col_index).Count; index++)
            {
                if (GetColumn(col_index)[index].cellState == CellState.Filled)
                {
                    check = (check && GetColumn(col_index)[index].isLocked);
                    counter++;
                }
            }
            if (counter == 0) check = false;
            return check;
        }

        private void ConvertLine(List<Cell> list, Cell currentCell, CheckType type)
        {
            int position;
            if (type == CheckType.RowCheck)
            {
                position = currentCell.GetColumnIndex();
            }
            else
            {
                position = currentCell.GetRowIndex();
            }
            int index = 0;
            for (int i = position; i >= 0; i--)
            {
                Cell cell = list[i];
                if (!cell.isLocked && cell.cellState == CellState.Crossed)
                {
                    cell.OpenWithAnimation(index * 0.1f);
                    index++;
                }
            }
            index = 0;
            for (int i = position; i < list.Count; i++)
            {
                Cell cell = list[i];
                if (!cell.isLocked && cell.cellState == CellState.Crossed)
                {
                    cell.OpenWithAnimation(index * 0.1f);
                    index++;
                }
            }
        }


        public List<Cell> GetNearbyCells(Cell cell)
        {
            nearbyCells.Clear();
            int row = cell.GetRowIndex();
            int col = cell.GetColumnIndex();
            for (int index = row - 1; index <= row + 1; index++)
            {
                if (index < 0 || index >= gridSize) { continue; }
                for (int colIndex = col - 1; colIndex <= col + 1; colIndex++)
                {
                    if (colIndex < 0 || colIndex >= gridSize) { continue; }
                    nearbyCells.Add(gridList[index * gridSize + colIndex]);
                }
            }
            return nearbyCells;
        }

        public void FadeInfoText(List<Cell> list, Cell cell, CheckType type)
        {
            int infoIndex;
            List<int> infoList = InfoNumbers(list);
            var tempList = new List<int>();
            foreach (int x in infoList) { tempList.Add(0); }

            // CHECK TYPE OF FADING
            if (type == CheckType.RowCheck)
            {
                infoIndex = cell.GetRowIndex() + (textInfoList.Count / 2);
            }
            else
            {
                infoIndex = cell.GetColumnIndex();
            }
            int groupIndex = tempList.Count-1;
            int count = 0;
            int lastIndex = gridSize - 1;
            while (lastIndex>=0&& groupIndex >= 0)
            {
                Cell tempCell = list[lastIndex];
                if (!tempCell.isLocked)
                {
                    break;
                }

                else if (tempCell.cellState == CellState.Filled)
                {
                    count++;
                }
                else if (tempCell.cellState == CellState.Crossed)
                {
                    count = 0;
                }
                if (count != 0)
                {
                    if (infoList[groupIndex] == count)
                    {
                        tempList[groupIndex] = count;
                        groupIndex--;
                    }
                }
                lastIndex--;
            }
            groupIndex = 0;
            count = 0;
            int index = 0;
            while (index < gridSize && groupIndex < tempList.Count)
            {
                Cell tempCell = list[index];
                if (!tempCell.isLocked)
                {
                    break;
                }

                else if (tempCell.cellState == CellState.Filled)
                {
                    count++;
                }
                else if (tempCell.cellState == CellState.Crossed)
                {
                    count = 0;
                }
                if (count != 0)
                {
                    if (groupIndex < infoList.Count)
                    {
                        if (infoList[groupIndex] == count)
                        {
                            tempList[groupIndex] = count;
                            groupIndex++;
                        }
                    }
                }
                index++;
            }

            count = 0;
            if (list[gridSize - 1].isLocked && list[gridSize - 1].cellState == CellState.Filled)
            {
                for (int a = list.Count - 1; a > 0; a--)
                {
                    if (list[a].isLocked && list[a].cellState == CellState.Filled)
                    {
                        count++;
                    }
                    if (list[a].cellState == CellState.Crossed) { break; }
                }
            }
            if (infoList.Count - 1 >= 0 && count == infoList[infoList.Count - 1])
            {
                tempList[tempList.Count - 1] = count;
            }

            ArrangeFadedText(infoIndex, tempList, infoList, type);
        }

        public void ArrangeFadedText(int infoIndex,List<int> tempList, List<int> infoList, CheckType type)
        {
            TextMeshPro textMeshPro = textInfoList[infoIndex];
            textMeshPro.text = "";
            for (int i = 0; i < infoList.Count; i++)
            {
                if (i < tempList.Count && tempList[i] == infoList[i])
                {
                    textMeshPro.text += "<alpha=#88>" + infoList[i].ToString() + "<alpha=#FF>";
                }
                else
                {
                    textMeshPro.text += infoList[i].ToString();
                }
                if (type == CheckType.RowCheck) { textMeshPro.text += " "; }
                else { textMeshPro.text += "\n"; }
            }
            tempList.Clear();
        }


        public Cell GetPreviousCell()
        {
            return previousCell;
        }

        public void SetPreviousCell(Cell cell)
        {
            previousCell = cell;
        }

        public Cell GetCell(int x, int y)
        {
            int index = (x * gridSize) + y;
            return gridList[index];
        }

        public void SetCell(Cell cell, int x, int y)
        {
            int index = (x * gridSize) + y;
            gridList[index] = cell;
        }
        public int GetGridSize()
        {
            return gridSize;
        }
        public void SetGridSize(int size)
        {
            this.gridSize = size;
        }
        public List<CellData> GetCellDataList()
        {
            return cellDataList;
        }
        public List<CellData> CreateCellDataList()
        {
            cellDataList = new List<CellData>();
            foreach (Cell cell in gridList)
            {
                cellDataList.Add(cell.GetCellData());
            }
            return cellDataList;
        }
        public void UpdateCellDatas()
        {
            cellDataList.Clear();
            foreach (Cell cell in gridList)
            {
                cellDataList.Add(cell.GetCellData());
            }
        }
        public void SetCellDataList(List<CellData> cellDatas)
        {
            cellDataList = cellDatas;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (GameManager.instance.GetGridManager() == null) GameManager.instance.SetGridManager(this);
        }

        private void OnEnable()
        {
            GameManager.instance.OnCellDown += ActivateCell;
            GameManager.instance.OnCellUp += RefereshMovement;
            SceneManager.sceneLoaded += OnSceneLoaded;

        }

        private void OnDisable()
        {
            GameManager.instance.OnCellDown -= ActivateCell;
            GameManager.instance.OnCellUp -= RefereshMovement;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

}


