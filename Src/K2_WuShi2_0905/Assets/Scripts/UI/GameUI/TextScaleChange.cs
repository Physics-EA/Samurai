using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextScaleChange : MonoBehaviour 
{
    private float scale = 1.2f;
    private float scaleTime = 0.2f;
    private float stayTime = 1.0f;
    private bool old = false;
    private Image text;

    void Start()
    {
        text = GetComponent<Image>();
    }

    void Update()
    {
        if (gameObject.activeSelf != old)
        {
            old = gameObject.activeSelf;
            text.transform.DOScale(scale, scaleTime).SetEase(Ease.InOutBounce).OnComplete(Reset);
        }
    }

    private void Reset()
    {      
        Invoke("DisableObject",stayTime);    
    }

    private void DisableObject()
    {
        text.transform.DOScale(1, 0.1f);
        text.gameObject.SetActive(false);
        old = false;
    }
}
