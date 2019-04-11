using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.GameData;

public class PlayerPanelCtrl : MonoBehaviour 
{
    //  1.  需要实时更新当前的金币数量
    //  2.  判断生命等级按钮和连击按钮的激活状态
    //  3.  管控技能列表面板的状态
    public Text CurCoin;
    public Text UpLevelCoin;
    public Text NewSkillCoin;

    //  记录技能列表
    public Transform SkillList;
    private Transform[] Skills;
    private int skillLearned = 0;

	// Use this for initialization
	void Start () 
	{
        //  初始化技能
        Skills = new Transform[SkillList.childCount];

        for (int i = 0; i < Skills.Length; i++)
        {
            Skills[i] = SkillList.GetChild(i);
        }
	}
	
	// Update is called once per frame
	void Update () 
	{
        CurCoin.text = Player.Instance.coin.ToString();

        if (int.Parse(CurCoin.text) >= int.Parse(UpLevelCoin.text))
        {
            //Debug.Log("激活生命等级按钮");
            //UplevelBtn.interactable=true;
            UpLevelCoin.GetComponent<Button>().interactable = true;
        }
        else
        {
            UpLevelCoin.GetComponent<Button>().interactable = false;
        }

        if (int.Parse(CurCoin.text) >= int.Parse(NewSkillCoin.text))
        {
            NewSkillCoin.GetComponent<Button>().interactable = true;
        }
        else
        {
            NewSkillCoin.GetComponent<Button>().interactable = false;
        }

        if (skillLearned >= Skills.Length)
        {
             NewSkillCoin.GetComponent<Button>().interactable = false;
        }
	}

    //  当升级按钮按下时,玩家扣除相应的金币，同时玩家等级+1
    public void OnUpLevelBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        Player.Instance.coin -= int.Parse(UpLevelCoin.text);
        Debug.Log("玩家等级加一");
    }

    //  当新攻击技能按下时,开启一个新的攻击技能，同时玩家金币减少
    public void OnNewSkillBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        if (ActiveNewSkill())
        {
            Player.Instance.coin -= int.Parse(NewSkillCoin.text);
            Debug.Log("开启一个新的技能");
            skillLearned++;
        }
    }

    //  激活技能列表中第一个未激活的技能
    private bool ActiveNewSkill()
    {
        for (int i = 0; i < Skills.Length; i++)
        {
            Transform skillmask = Skills[i].GetChild(0);
            if (skillmask.gameObject.activeSelf)
            {
                skillmask.gameObject.SetActive(false);
                Skills[i].GetChild(1).gameObject.SetActive(true);

                Debug.Log("激活技能"+i+1);               
                return true;
            }
        }

        Debug.Log("技能已全部激活");
        return false;
    }
}
