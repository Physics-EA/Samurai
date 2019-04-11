using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.GameData;

public class startPaneCtrl : MonoBehaviour 
{

    public void OnContinueBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UISoundManager.Instance.MusicOn(MusicType.MT_SubmenuIn);

        Debug.Log("继续游戏");
        //  通过XML读取本地存档记录，然后到对应的游戏进度中
        StartCoroutine(StartGame(GameType.GT_Continue));
    }
	
    public void OnEasyBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UISoundManager.Instance.MusicOn(MusicType.MT_SubmenuIn);

        Debug.Log("简单模式");
        //  按照简单模式的本地XML配置来配置怪物属性，然后进入第一关
        StartCoroutine(StartGame(GameType.GT_Easy));
    }

    public void OnMidBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UISoundManager.Instance.MusicOn(MusicType.MT_SubmenuIn);

        Debug.Log("中等模式");
        StartCoroutine(StartGame(GameType.GT_Middle));
    }

    public void OnHardBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UISoundManager.Instance.MusicOn(MusicType.MT_SubmenuIn);

        Debug.Log("困难模式");
        StartCoroutine(StartGame(GameType.GT_Hard));
    }

    private IEnumerator StartGame(GameType _type)
    {
        yield return new WaitForSeconds(0.8f);

        GameDate.LoadGamePropsOnXML(_type);

        if (_type==GameType.GT_Continue)
        {
            Debug.Log("根据本地存储的游戏进度到达对应关卡开场漫画场景");
        }
        else
        {
            SceneManager.LoadScene("Comics01");
            //Application.LoadLevel("Comics01");
        }     
    }
}
