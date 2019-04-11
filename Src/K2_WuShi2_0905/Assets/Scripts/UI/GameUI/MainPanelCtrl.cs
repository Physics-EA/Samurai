using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanelCtrl : MonoSingleton<MainPanelCtrl> 
{
    public Transform BloodPanel;
    public Transform SkillPanel;
    public Transform PlayerBgPanel;
   
    //  连招数字，用图片表示，分别对应的0-9的数字
    public Sprite[] comboNum = new Sprite[10];
    //  技能描述文字，共右6种终极技能
    //  分别是浪翻，半月，云切，飞龙，踏死，破将
    public Sprite[] skillMessage = new Sprite[6];
    //  玩家按钮信息
    public Sprite[] buttonMessage = new Sprite[4];
    private Image prefabsBtnMessage;

    //  技能面板下面的三个子面板
    private Transform SkillShowPanel;
    private Transform ComboMessagePanel;
    private Transform ComboProgressPanel; 

    //  组合连招信息面板下面的4图片信息，提前获取，方便操作
    private Image bai;
    private Image shi;
    private Image ge;
    private Image text;

    //  玩家信息
    private Text curCoin;
    private Slider playerHPBar;
    private Image skill;
	// Use this for initialization
	void Start () 
	{
        prefabsBtnMessage = Resources.Load<Image>("Prefabs/UI/ButtonMessage");

        if (SkillPanel != null)
        {
            SkillShowPanel = SkillPanel.GetChild(0);
            ComboMessagePanel = SkillPanel.GetChild(1);
            ComboProgressPanel = SkillPanel.GetChild(2);

            skill = SkillShowPanel.GetChild(0).GetComponent<Image>();
        }

        if (ComboMessagePanel != null)
        {
            bai = ComboMessagePanel.GetChild(0).GetComponent<Image>();
            shi = ComboMessagePanel.GetChild(1).GetComponent<Image>();
            ge = ComboMessagePanel.GetChild(2).GetComponent<Image>();
            text = ComboMessagePanel.GetChild(3).GetComponent<Image>();

            bai.sprite = comboNum[0];
            shi.sprite = comboNum[0];
            ge.sprite = comboNum[0];
            bai.gameObject.SetActive(false);
            shi.gameObject.SetActive(false);
            ge.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
        }

        if (PlayerBgPanel != null)
        {
            curCoin = PlayerBgPanel.GetChild(1).GetComponent<Text>();
            playerHPBar = PlayerBgPanel.GetChild(2).GetComponent<Slider>();

            curCoin.text = Player.Instance.coin.ToString();
            playerHPBar.value = Player.Instance.Owner.BlackBoard.Health / Player.Instance.Owner.BlackBoard.MaxHealth;
        }

	}
	
	// Update is called once per frame
	void Update () 
	{
        curCoin.text = Player.Instance.coin.ToString();
        playerHPBar.value = Player.Instance.Owner.BlackBoard.Health / Player.Instance.Owner.BlackBoard.MaxHealth;
	}

    //  显示血液面板,随机血液的位置（一定范围内随机）
    public void ShowBloodPanel()
    {
        //  一共有4滴血液类型，随机激活几个，然后随机位置，同时血液有个淡出的效果
        for (int i = 0; i < BloodPanel.childCount; i++)
        {
            //  随机激活
            BloodPanel.GetChild(i).gameObject.SetActive(Random.Range(0, 2) == 0);
            //  随机位置
            // BloodPanel.GetChild(i).GetComponent<RectTransform>().position;
            BloodPanel.GetChild(i).transform.position += Random.insideUnitSphere*0.01f;
            //  血液的淡出效果(在BLood的Update中实现)
        }
    }

    //  按键提示信息,根据输入的攻击方式来显示
    public void ComboProgressMessage(List<E_AttackType> comboProgress)
    {
        //  当前信息清空时,将SkillShowPanel下的所有孩子清除
        if (comboProgress == null || comboProgress.Count <= 0)
        {
            //  清掉之前的按键记录
            for (int i = 0; i < ComboProgressPanel.childCount; i++)
            {
                Destroy(ComboProgressPanel.GetChild(i).gameObject);
            }

            return;
        }

        //  清掉之前的记录
        if (ComboProgressPanel.childCount > 0)
        {
            for (int i = 0; i < ComboProgressPanel.childCount; i++)
            {
                Destroy(ComboProgressPanel.GetChild(i).gameObject);
            }
        }

        Image left = Instantiate<Image>(prefabsBtnMessage, ComboProgressPanel.transform.position,ComboProgressPanel.transform.rotation,ComboProgressPanel.transform);   
        left.sprite=buttonMessage[2];

        for (int i = 0; i < comboProgress.Count; i++)
        {
            Image tmp = Instantiate<Image>(prefabsBtnMessage, ComboProgressPanel.transform.position, ComboProgressPanel.transform.rotation, ComboProgressPanel.transform);
           
            if (comboProgress[i] == E_AttackType.X)
            {
                tmp.sprite = buttonMessage[0];
            }
            else
            {
                tmp.sprite = buttonMessage[1];
            }
        }

        Image right = Instantiate<Image>(prefabsBtnMessage, ComboProgressPanel.transform.position, ComboProgressPanel.transform.rotation, ComboProgressPanel.transform);   
        right.sprite = buttonMessage[3];

    }

    //  连击提示信息
    public void ShowComboMessage(int totalNum)
    {
        if (totalNum <= 0)
        {
            return;
        }

       int baiIndex = totalNum / 100;
       int shiIndex = (totalNum % 100) / 10;
       int geIndex = totalNum % 10;

       if (baiIndex > 0)
       {
           bai.sprite = comboNum[baiIndex];
           bai.gameObject.SetActive(true);
       }
       else
       {
           bai.gameObject.SetActive(false);
       }

       if (shiIndex > 0)
       {
           shi.sprite = comboNum[shiIndex];
           shi.gameObject.SetActive(true);
       }
       else
       {
           shi.gameObject.SetActive(false);
       }

       if (geIndex > 0)
       {
           ge.sprite = comboNum[geIndex];
           ge.gameObject.SetActive(true);
       }
       else
       {
           ge.gameObject.SetActive(false);
       }

       text.gameObject.SetActive(true);

    }

    //  终结技能文字描述
    public void SkillShowMessage(int index)
    {
        skill.sprite = skillMessage[index];
        skill.gameObject.SetActive(true);
    }
}
