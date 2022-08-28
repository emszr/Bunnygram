using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace Nonogram
{
    public class WinPanel : MonoBehaviour
    {
        [SerializeField] private List<Image> starList;
        [SerializeField] private List<Image> starBackList;

        [SerializeField] private float delaytime;

        public void MainMenuButtonFunction()
        {
            DOTween.KillAll();
            SceneManager.LoadScene(0);
        }

        public void NextLevelButtonFunction()
        {
            DOTween.KillAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        private void StarHelper(int i)
        {
            starList[i].gameObject.transform.DOScale(Vector3.one, 0.15f);
            starBackList[i].gameObject.SetActive(false);
        }

        public void DisplayStars()
        {
            Sequence mySequence = DOTween.Sequence();

            for (int i = 0; i < Constants.LIVES; i++)
            {
                starList[i].transform.DOKill();
            }

            for (int i = 0; i < GameManager.instance.currentHealth; i++)
            {
                int temp = i;
                mySequence.Append(starList[temp].transform.DOScale(new Vector3(1.5f, 1.5f), 0.3f).OnComplete(() => StarHelper(temp)));

            }
        }
        public void OnEnable()
        {
            EnableAnimation();
        }

        public void EnableAnimation()
        {
            SoundManager.instance.PlaySound(SoundType.WinSound);
            gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).OnComplete(() => DisplayStars());
        }
    }

}
