using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class helpPanelAnim : MonoBehaviour 
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
        GetComponent<RectTransform>().DOAnchorPosX(0, 0.2f);
    }

    public void MoveAway()
    {
        GetComponent<RectTransform>().DOAnchorPosX(-4300, 0.2f);
    }
}
