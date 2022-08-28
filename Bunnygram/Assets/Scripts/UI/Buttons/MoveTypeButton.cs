using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Nonogram
{
    public class MoveTypeButton : MonoBehaviour
    {
        public List<Sprite> spriteList;
        public Transform circleTransform;
        public Transform crossPosition;
        public Transform filledPosition;
        public Image moveTypeImage;
        public Image deactiveImage;
        public Image deactiveImage2;
        public Button button;

        private void Start()
        {
            Initialize(GameManager.instance.GetInputManager().GetCurrentMove());
        }

        public void Initialize(CellState currentMove)
        {
            if (currentMove == CellState.Filled)
            {
                moveTypeImage.sprite = spriteList[0];
                circleTransform.position = filledPosition.position;
            }
            else
            {
                moveTypeImage.sprite = spriteList[1];
                circleTransform.position = crossPosition.position;
            }
        }

        public void Change()
        {
            SoundManager.instance.PlaySound(SoundType.Swipe);
            circleTransform.DOKill();
            GameManager.instance.OnMoveTypeChanged?.Invoke();

            if (GameManager.instance.GetInputManager().GetCurrentMove() == CellState.Crossed)
            {
                moveTypeImage.sprite = spriteList[1];
                circleTransform.DOMove(crossPosition.position, 0.5f);
            }
            else
            {
                moveTypeImage.sprite = spriteList[0];
                circleTransform.DOMove(filledPosition.position, 0.5f);
            }
        }

        private void OnEnable()
        {
        }
    }
}
