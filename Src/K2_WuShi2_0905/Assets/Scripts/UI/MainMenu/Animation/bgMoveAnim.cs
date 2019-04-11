using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class bgMoveAnim : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
        StartMove();
	}
	
	// Update is called once per frame
	void Update ()
    {
        #region 动画测试代码
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveAway();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveBack();
        }
        #endregion
    }

    private void StartMove()
    {
        transform.DOMoveY(1, 0.2f).SetEase(Ease.InBack);
    }

    public void MoveAway()
    {
        Sequence mySeq = DOTween.Sequence();
        mySeq.Append(transform.DOMoveX(2, 0.2f));
        mySeq.Join(transform.DOMoveY(2, 0.2f));
        mySeq.Join(transform.DOScale(4,0.2f));
    }

    public void MoveBack()
    {
        Sequence mySeq = DOTween.Sequence();
        mySeq.Append(transform.DOMoveX(0, 0.2f));
        mySeq.Join(transform.DOMoveY(1, 0.2f));
        mySeq.Join(transform.DOScale(8, 0.2f));
    }
}
