using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

namespace Nonogram
{
    [Serializable]
    public class CellData
    {
        public bool isLocked;
        public CellState state;

        public CellData(bool locked,CellState state)
        {
            isLocked = locked;
            this.state = state;
        }
    }

    public class Cell : MonoBehaviour
    {
        public CellState cellState = CellState.Filled;
        public Sprite sprite;
        //sspublic Sprite originalSprite;

        //public SpriteRenderer cellSpriteRenderer;
        public bool isLocked = false;
        
        [SerializeField] private SpriteRenderer childSpriteRenderer;

        private int row;
        private int column;


        public SpriteRenderer GetChildSpriteRenderer()
        {
            return childSpriteRenderer;
        }

        public CellData GetCellData()
        {
            return new CellData(isLocked, cellState);
        }

        public int GetColumnIndex()
        {
            return column;
        }

        public int GetRowIndex()
        {
            return this.row;
        }
        public void SetColumn(int col)
        {
            this.column = col;
        }

        public void SetRow(int row)
        {
            this.row = row;
        }

        public CellState GetState()
        {
            return cellState;
        }
        public void SetCellState(CellState state)
        {
            cellState = state;
        }
        public void Open()
        {
            isLocked = true;
            //cellSpriteRenderer.sprite = sprite;
            childSpriteRenderer.gameObject.SetActive(true);
        }
        public void Close()
        {
            isLocked = false;
            //cellSpriteRenderer.sprite = originalSprite;
            childSpriteRenderer.gameObject.SetActive(false);
        }

        public void OpenWithAnimation(float delayDuration)
        {
            SpriteRenderer renderer = GetChildSpriteRenderer();
            Vector3 realVector = renderer.gameObject.transform.localScale;
            Tween tween = DOTween.ToAlpha(() => renderer.color, x => renderer.color = x, 1f, 0.2f).From(0f).SetDelay(delayDuration);
            renderer.gameObject.transform.DOScale(realVector, 0.35f).From(realVector * 1.7f);
            Open();
        }
    }
}
