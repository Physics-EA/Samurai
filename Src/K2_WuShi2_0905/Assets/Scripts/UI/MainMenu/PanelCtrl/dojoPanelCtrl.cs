using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.GameData;

public class dojoPanelCtrl : MonoBehaviour 
{
    public void OnContinueBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UISoundManager.Instance.MusicOn(MusicType.MT_SubmenuIn);

        Debug.Log("继续游戏");
        //  读取之前的道场游戏进度，进入游戏界面
        StartCoroutine(StartGame(GameType.DT_Continue));
    }

    public void OnStartBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UISoundManager.Instance.MusicOn(MusicType.MT_SubmenuIn);

        Debug.Log("开始道场模式");
        //  重新开始道场游戏
        StartCoroutine(StartGame(GameType.DT_NewGame));
    }

    private IEnumerator StartGame(GameType _type)
    {
       yield return new WaitForSeconds(0.8f);  
   
       GameDate.LoadGamePropsOnXML(_type);

       SceneManager.LoadScene("dojo"); 
       //Application.LoadLevel("dojo");
    }
}
