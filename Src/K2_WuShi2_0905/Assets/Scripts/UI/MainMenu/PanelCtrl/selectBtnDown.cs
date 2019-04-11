using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.GameData;

public class selectBtnDown : MonoBehaviour 
{
    public Text text;

    public void OnSelectBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UISoundManager.Instance.MusicOn(MusicType.MT_SubmenuIn);

        if (text==null)
        {
            return;
        }

        Debug.Log("进入第"+text.text+"章节");
        StartCoroutine(EnterInto(int.Parse(text.text)));
    }

    private IEnumerator EnterInto(int id)
    {
        yield return new WaitForSeconds(0.8f);

        //  按照默认的游戏模式加载玩家数据信息，easy
        GameDate.LoadGamePropsOnXML();
        string comicsID = "Comics0" + id.ToString();

        SceneManager.LoadScene(comicsID);
        //Application.LoadLevel(comicsID);
    }
}
