using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class toolBarAnim : MonoBehaviour 
{

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
            MoveAway();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveBack();
        }
        #endregion
	}

    public void MoveAway()
    {
        this.GetComponent<RectTransform>().DOAnchorPosY(-120,0.2f);
    }

    public void MoveBack()
    {
        this.GetComponent<RectTransform>().DOAnchorPosY(-1, 0.2f);
    }
}
