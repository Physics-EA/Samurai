using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.GameData;

public class backBtnAnim : MonoBehaviour 
{
	
	// Update is called once per frame
	void Update () 
	{
        #region 动画测试代码
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveBack();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {         
            MoveAway();
        }
        #endregion
	}

    public void MoveBack()
    {
        GetComponent<RectTransform>().DOAnchorPosX(455,0.2f);
    }

    public void MoveAway()
    {
        GetComponent<RectTransform>().DOAnchorPosX(825, 0.2f);
    }
}
