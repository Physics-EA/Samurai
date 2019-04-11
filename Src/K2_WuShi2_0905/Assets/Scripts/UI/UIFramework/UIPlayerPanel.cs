using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.GameData;

public class UIPlayerPanel : UIBasePanel 
{
    private GameObject CancelBtn;

    public override void OnInit()
    {
        base.OnInit();

        this.id = UIPanelID.ID_MainPanel;
        this.showMode = UIShowMode.M_HideAll;
        this.IsAlwaysAbove = true;

        CancelBtn = UIUtils.findChild(this.gameObject, "CancelBtn");

        if (CancelBtn == null)
        {
            return;
        }
        CancelBtn.GetComponentInChildren<Button>().onClick.AddListener(OnCanCelBtnDown);
    }

    #region 事件回调函数
    public void OnCanCelBtnDown()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
        UIPanelManager.Instance.HideUIPanel(UIPanelID.ID_PlayerPanel);
        UIPanelManager.Instance.ShowUIPanel(UIPanelID.ID_MainPanel);
    }
    #endregion
}
