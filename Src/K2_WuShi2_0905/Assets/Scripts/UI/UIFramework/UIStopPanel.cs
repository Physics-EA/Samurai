using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.GameData;

public class UIStopPanel : UIBasePanel 
{
    private GameObject LeaveGameBtn;
    private GameObject SettingBtn;
    private GameObject BackToGameBtn;

    public override void OnInit()
    {
        base.OnInit();

        this.id = UIPanelID.ID_MainPanel;
        this.showMode = UIShowMode.M_HideAll;
        this.IsAlwaysAbove = true;

        LeaveGameBtn = UIUtils.findChild(this.gameObject, "LeaveGame");

        if (LeaveGameBtn == null)
        {
            return;
        }

        LeaveGameBtn.GetComponentInChildren<Button>().onClick.AddListener(OnLeaveGameBtnDown);

        SettingBtn = UIUtils.findChild(this.gameObject, "SettingGame");

        if (SettingBtn == null)
        {
            return;
        }

        SettingBtn.GetComponentInChildren<Button>().onClick.AddListener(OnSettingBtnDown);

        BackToGameBtn = UIUtils.findChild(this.gameObject, "BackToGame");

        if (SettingBtn == null)
        {
            return;
        }

        BackToGameBtn.GetComponentInChildren<Button>().onClick.AddListener(OnBackToGameBtnDown);
    }

    #region 事件回调函数
    public void OnSettingBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        Debug.Log("暂未开放");
        //UIPanelManager.Instance.HideUIPanel(UIPanelID.ID_StopPanel);
        //UIPanelManager.Instance.ShowUIPanel(UIPanelID.ID_SettingPanel);
    }

    public void OnBackToGameBtnDown()
    {       
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UIPanelManager.Instance.HideUIPanel(UIPanelID.ID_StopPanel);
        UIPanelManager.Instance.ShowUIPanel(UIPanelID.ID_MainPanel);
    }

    public void OnLeaveGameBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UIPanelManager.Instance.HideUIPanel(UIPanelID.ID_StopPanel);
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}
