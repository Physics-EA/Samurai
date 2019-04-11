using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BloodFadeOut : MonoBehaviour 
{
    private float fadeOutTime = 4;
    private bool old = false;
    private Image blood;

    void Start () 
    {
        blood = GetComponent<Image>();
    }

    void Update()
    {
        if (gameObject.activeSelf != old)
        {
            old = gameObject.activeSelf;
            //  Debug.Log("淡出");

            blood.DOFade(0, fadeOutTime).OnComplete(Reset);         
        }
    }

    private void Reset()
    {
        //  Debug.Log("重置");
        Color color = Color.white;
        color.a = 1;
        blood.color = color;
        blood.gameObject.SetActive(false);

        old = false;
    }
}
