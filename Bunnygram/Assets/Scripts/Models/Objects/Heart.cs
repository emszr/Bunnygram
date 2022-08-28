using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class Heart : MonoBehaviour
{
    public int index;
    public Image childImage;
    public Sprite brokenSprite;


    public void Die()
    {
        childImage.sprite = brokenSprite;
        childImage.transform.DOScale(new Vector3(2f, 2f), 0.5f).OnComplete(() => childImage.transform.DOScale(Vector3.zero, 0.3f));
    }
}