using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class startPanelAnim : MonoBehaviour 
{
    public RectTransform title;
    public RectTransform selectPanel;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
        #region 动画测试代码
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveInto();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveOut();
        }
        #endregion
	}

    public void MoveInto()
    {
        title.DOAnchorPosX(-350,0.2f);
        selectPanel.DOAnchorPosX(0, 0.2f);
    }

    public void MoveOut()
    {
        title.DOAnchorPosX(-875, 0.2f);
        selectPanel.DOAnchorPosX(1100, 0.2f);
    }
}
