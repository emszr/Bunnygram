using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Nonogram
{
    public class GridBuilder : MonoBehaviour
    {
        #region Serialized Field
        [SerializeField] private Cell cell;
        [SerializeField] private GameObject grid;
        [SerializeField] private GameObject rowInfoPrefab;
        [SerializeField] private GameObject colInfoPrefab;
        [SerializeField] private SpriteRenderer gridRenderer;
        [SerializeField] private SpriteRenderer cellRenderer;
        [SerializeField] private SpriteRenderer thinLineRenderer;
        [SerializeField] private SpriteRenderer boldLineRenderer;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Image powerUpMaskParent;
        [SerializeField] private Image powerUpMaskChild;
        #endregion

        private float scaleRateWidth;
        private float scaleRateHeight;

        public Sprite[] sprite_list;
        private List<Cell> listGrid;
        private List<TextMeshPro> listInfoGrid;

        public void InitiliazeBuilder()
        {
            Random.InitState(GenerateSeed());
        }

        public void ScaleGrid()
        {
            float newGridScale = (float)((float)mainCamera.pixelWidth / (float)mainCamera.pixelHeight);

            if (newGridScale < Constants.DEFAULTSCREENRATIO)
            {
                newGridScale /= Constants.DEFAULTSCREENRATIO;
                grid.gameObject.transform.localScale = new Vector3(newGridScale, newGridScale);

                powerUpMaskParent.rectTransform.sizeDelta = new Vector2(powerUpMaskParent.rectTransform.sizeDelta.x * newGridScale,
                powerUpMaskParent.rectTransform.sizeDelta.y * newGridScale);
            }
         
        }

        public List<Cell> ArrangeCells(List<Cell> tempList, int n)
        { 
            var list = new List<Cell>(tempList);

            float gridWidth = gridRenderer.bounds.size.x;
            float gridHeight = gridRenderer.bounds.size.y;

            float boldLineHeight = boldLineRenderer.bounds.size.y;
            float thinLineHeight = thinLineRenderer.bounds.size.y;

            boldLineRenderer.size = new Vector2(gridWidth - 2 * (boldLineHeight), boldLineHeight);
            thinLineRenderer.size = new Vector2(gridWidth - 2 * (boldLineHeight), thinLineHeight);

            float originalCellWidth = cellRenderer.bounds.size.x;
            float originalCellHeight = cellRenderer.bounds.size.y;

            float newCellWidth = gridWidth / n;
            float newCellHeight = gridHeight / n;

            scaleRateWidth = newCellWidth / originalCellWidth / grid.transform.localScale.x * cell.transform.localScale.x;
            scaleRateHeight = newCellHeight / originalCellHeight / grid.transform.localScale.y * cell.transform.localScale.y;

            GameObject tempLine;
            Vector3 linePos = Vector3.zero;
            Quaternion rotated = Quaternion.Euler(0f, 0f, -90f);

            for (int row = 0; row < n; row++)
            {
                linePos = new Vector3(grid.transform.position.x - (gridWidth / 2f) + boldLineHeight,
                                      grid.transform.position.y + (gridHeight / 2f) - (newCellWidth * row));

                if (row != 0 && row % 5 == 0)
                {
                    tempLine = Instantiate(boldLineRenderer, linePos, Quaternion.identity).gameObject;
                    tempLine.transform.SetParent(grid.transform);
                }

                else if (row != 0)
                {
                    tempLine = Instantiate(thinLineRenderer, linePos, Quaternion.identity).gameObject;
                    tempLine.transform.SetParent(grid.transform);
                }

                for (int col = 0; col < n; col++)
                {
                    linePos = new Vector3(grid.transform.position.x - (gridWidth / 2f) + (newCellHeight * col),
                                      grid.transform.position.y + (gridHeight / 2f) - boldLineHeight);

                    if (row == 0 && col != 0 && col % 5 == 0)
                    {
                        tempLine = Instantiate(boldLineRenderer, linePos, rotated).gameObject;
                        tempLine.transform.SetParent(grid.transform);
                    }

                    else if (row == 0 && col != 0)
                    {
                        tempLine = Instantiate(thinLineRenderer, linePos, rotated).gameObject;
                        tempLine.transform.SetParent(grid.transform);
                    }

                    Vector3 position = new Vector3(grid.transform.position.x - (gridWidth / 2) + (newCellWidth / 2) + (newCellWidth * col),
                    grid.transform.position.y + (gridHeight / 2) - (newCellHeight / 2) - (newCellHeight * row));

                    list[row * n + col].transform.position = position;
                    list[row * n + col].transform.localScale = new Vector2(scaleRateWidth, scaleRateHeight);
                }
            }

            return list;
        }

        public List<Cell> GenerateGrids(int size,float upperBound,float lowerBound)
        {
            int filledBoxCount= (int)(Random.Range(lowerBound,upperBound)*(size*size));
            List<Cell> tempList = GenerateFilledList(filledBoxCount, size);
            tempList.Shuffle<Cell>(Random.Range(0, 10));
            listGrid = new List<Cell>(size * size);

            int count = 0;
            foreach (Cell cell in tempList)
            {
                cell.name = "cell " + (count / size) + "_" + (count % size);
                cell.SetRow((count / size));
                cell.SetColumn((count % size));
                listGrid.Add(cell);
                count++;
            }
            listGrid = ArrangeCells(listGrid, size);
            return listGrid;
        }

        public List<Cell> GenerateFromData(List<CellData> datas, int size)
        {
            listGrid = new List<Cell>(size * size);
            int count = 0;
            foreach (CellData data in datas)
            {
                PoolItem poolItem = PoolManager.instance.GetFromPool(PoolItemType.Cell, grid.transform);

                if (poolItem == null)
                {
                    poolItem = Instantiate(PoolManager.instance.prefabList[0]);
                    PoolManager.instance.AddToUsing(poolItem, grid.transform);
                }

                Cell pooledCell = poolItem.GetComponent<Cell>();

                int index = (int)data.state;
                pooledCell.Close();
                pooledCell.SetCellState((CellState)index);

                pooledCell.GetChildSpriteRenderer().sprite = sprite_list[index];

                int row = count / size;
                int column = count % size;
                pooledCell.name = "cell " + row + "_" + column;
                pooledCell.SetRow(row);
                pooledCell.SetColumn(column);

                if (data.isLocked)
                {
                    pooledCell.Open();
                }

                listGrid.Add(pooledCell);
                count++;
            }

            listGrid = ArrangeCells(listGrid, size);
            return listGrid;
        }

        public List<TextMeshPro> GenerateInfoGrids(int size)
        {
            listInfoGrid = new List<TextMeshPro>(size * 2);

            float gridWidth = gridRenderer.bounds.size.x;
            float gridHeight = gridRenderer.bounds.size.y;

            float newCellWidth = gridWidth / size;
            float newCellHeight = gridHeight / size;

            float colInfoHeight = colInfoPrefab.GetComponent<RectTransform>().rect.size.y / Constants.INFOPREFABMULTIPLIER;
            float rowInfoWidth = rowInfoPrefab.GetComponent<RectTransform>().rect.size.x / Constants.INFOPREFABMULTIPLIER;

            //Col info
            for (int index = 0; index < size; index++)
            {
                PoolItem pooledCol = PoolManager.instance.GetFromPool(PoolItemType.InfoColumn, grid.transform);

                if (pooledCol == null)
                {
                    pooledCol = Instantiate(PoolManager.instance.prefabList[1]);
                    PoolManager.instance.AddToUsing(pooledCol, grid.transform);
                }
                pooledCol.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                Vector3 position = new Vector3(grid.transform.position.x - (gridWidth / 2) + (newCellWidth / 2) + (newCellWidth * index),
                                   grid.transform.position.y + (gridHeight / 2) + colInfoHeight + Constants.INFOPREFABSPACE);

                pooledCol.gameObject.transform.position = position;
                pooledCol.name = "info col " + index;

                TextMeshPro tmp = pooledCol.GetComponent<TextMeshPro>();
                tmp.fontSize = Constants.DEFAULTFONTSIZE * (Constants.FONTMULTIPLIER / size);
                tmp.text = "";

                listInfoGrid.Add(pooledCol.GetComponent<TextMeshPro>());
            }

            //Row info
            for (int index = 0; index < size; index++)
            {
                PoolItem pooledRow = PoolManager.instance.GetFromPool(PoolItemType.InfoRow, grid.transform);

                if (pooledRow == null)
                {
                    pooledRow = Instantiate(PoolManager.instance.prefabList[2]);
                    PoolManager.instance.AddToUsing(pooledRow, grid.transform);
                }
                pooledRow.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                Vector3 position = new Vector3(grid.transform.position.x - (gridWidth / 2) - (rowInfoWidth + Constants.INFOPREFABSPACE),
                                   grid.transform.position.y + (gridHeight / 2) - (newCellHeight / 2) - (newCellHeight * index));

                pooledRow.gameObject.transform.position = position;
                pooledRow.name = "info row " + index;
                TextMeshPro tmp = pooledRow.GetComponent<TextMeshPro>();
                tmp.fontSize = Constants.DEFAULTFONTSIZE * (Constants.FONTMULTIPLIER / size);
                listInfoGrid.Add(pooledRow.GetComponent<TextMeshPro>());
            }

            return listInfoGrid;
        }
        public int GenerateSeed()
        {
            var temp = Random.state;
            Random.InitState(LevelManager.instance.GetCurrentLevel().levelCount);
            GameManager.instance.seedGenerator = Random.state;
            Random.state = GameManager.instance.seedGenerator;
            int generatedSeed = Random.Range(int.MinValue, int.MaxValue);
            Random.state = temp;
            return generatedSeed;
        }

        public List<Cell> GenerateFilledList(int filledCount,int gridsize)
        {
            List<Cell> tempList = new List<Cell>(gridsize*gridsize);
            int total = gridsize*gridsize;
            int count = 0;
            while (count < total)
            {
                PoolItem poolItem = PoolManager.instance.GetFromPool(PoolItemType.Cell, grid.transform);

                if (poolItem == null)
                {
                    poolItem = Instantiate(PoolManager.instance.prefabList[0]);
                    PoolManager.instance.AddToUsing(poolItem, grid.transform);
                }
                Cell pooledCell = poolItem.GetComponent<Cell>();
                if (count < filledCount)
                {
                    pooledCell.SetCellState(CellState.Filled);
                    pooledCell.GetChildSpriteRenderer().sprite = sprite_list[(int)CellState.Filled];
                }
                else
                {
                    pooledCell.SetCellState(CellState.Crossed);
                    pooledCell.GetChildSpriteRenderer().sprite = sprite_list[(int)CellState.Crossed];
                }
                pooledCell.Close();
                pooledCell.gameObject.SetActive(true);

                tempList.Add(pooledCell);
                count++;
            }
            return tempList;
        }
    }
}