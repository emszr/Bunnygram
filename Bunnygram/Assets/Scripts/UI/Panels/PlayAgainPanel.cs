using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Nonogram
{
    public class PlayAgainPanel : MonoBehaviour
    {
        [SerializeField] private List<Image> starList;
        [SerializeField] private List<Image> starBackList;
        [SerializeField] private float delaytime;
        [SerializeField] private TextMeshProUGUI textMesh;
        public GameObject mask;
        private int starCount = 0;
        private int indexLevel;

        public void CloseButtonFunction()
        {
            SoundManager.instance.PlaySound(SoundType.ButtoUnClick);
            foreach (Image image in starList)
            {
                image.transform.localScale = Vector3.zero;
            }
            foreach (Image image in starBackList)
            {
                image.gameObject.SetActive(true);
            }
            gameObject.SetActive(false);
            mask.gameObject.SetActive(false);
        }

        public void PlayButtonFunction()
        {
            GameManager.instance.LoadLevelSceneFromMenu(indexLevel);
        }
        public void DisplayFrame(int starCount, int indexLevel)
        {
            this.starCount = starCount;
            this.indexLevel = indexLevel;
            mask.gameObject.SetActive(true);
            gameObject.SetActive(true);
            textMesh.text = "LEVEL " + (indexLevel + 1).ToString() + " COMPLETED";
        }

        private void StarHelper(int i)
        {
            starList[i].gameObject.transform.DOScale(Vector3.one, 0.15f);
            //starBackList[i].gameObject.SetActive(false);
        }

        public void DisplayStars(int starCount)
        {
            Sequence mySequence = DOTween.Sequence();

            for (int i = 0; i < starCount; i++)
            {
                starList[i].gameObject.SetActive(true);
                int temp = i;
                transform.DOKill();
                mySequence.Append(starList[temp].transform.DOScale(new Vector3(1.5f, 1.5f), 0.3f).From(Vector3.zero).OnComplete(() => StarHelper(temp)));
            }
        }

        public void OnEnable()
        {
            EnableAnimation();
        }

        public void EnableAnimation()
        {
            for (int i = 0; i < Constants.LIVES; i++)
            {
                starBackList[i].gameObject.SetActive(true);
            }

            for (int i = 0; i < Constants.LIVES; i++)
            {
                starList[i].transform.localScale = Vector3.zero;
                starList[i].gameObject.SetActive(false);
            }

            gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).OnComplete(() => DisplayStars(starCount));
        }
    }

}