using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillMessageFadeOut : MonoBehaviour 
{
    private float fadeOutTime = 2;
    private float scale = 1.2f;
    private float scaleTime = 0.2f;
    private bool old = false;
    private Image skill;

    void Start()
    {
        skill = GetComponent<Image>();
    }

    void Update()
    {
        if (gameObject.activeSelf != old)
        {
            old = gameObject.activeSelf;
            //  Debug.Log("淡出");
            Sequence mySeq = DOTween.Sequence();

            mySeq.Append(skill.transform.DOScale(scale, scaleTime));
            mySeq.Append(skill.DOFade(0, fadeOutTime).OnComplete(Reset));      
        }
    }

    private void Reset()
    {
        skill.transform.DOScale(1, 0.1f);

        Color color = Color.white;
        color.a = 1;
        skill.color = color;

        skill.gameObject.SetActive(false);
        old = false;
    }
}
