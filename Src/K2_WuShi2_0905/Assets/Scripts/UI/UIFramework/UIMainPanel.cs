using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.GameData;

public class UIMainPanel : UIBasePanel 
{
    private GameObject stopBtn;
    private GameObject playerBtn;

    public override void OnInit()
    {
        base.OnInit();

        this.id = UIPanelID.ID_MainPanel;
        this.showMode = UIShowMode.M_HideAll;
        this.IsAlwaysAbove = true;

        stopBtn = UIUtils.findChild(this.gameObject, "StopBtn");

        if (stopBtn == null)
        {
            return;
        }

        stopBtn.GetComponentInChildren<Button>().onClick.AddListener(OnStopBtnDown);

        playerBtn = UIUtils.findChild(this.gameObject, "playerBtn");

        if (playerBtn == null)
        {
            return;
        }

        playerBtn.GetComponentInChildren<Button>().onClick.AddListener(OnPlayerBtnDown);
    }

    #region 事件回调函数
    public void OnStopBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UIPanelManager.Instance.HideUIPanel(UIPanelID.ID_MainPanel);
        UIPanelManager.Instance.ShowUIPanel(UIPanelID.ID_StopPanel);
    }

    public void OnPlayerBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UIPanelManager.Instance.HideUIPanel(UIPanelID.ID_MainPanel);
        UIPanelManager.Instance.ShowUIPanel(UIPanelID.ID_PlayerPanel);
    }
    #endregion
}
