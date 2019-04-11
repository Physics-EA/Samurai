using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GameData;

public class MainMenuMusicCtrl : MonoBehaviour 
{
    public void PlayMainMenuBtn()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MainmenuBtn);
    }

    public void PlayMainMenuIn()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MainmenuIn);
    }
	
    public void PlayMainMenuOut()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MainmenuOut);
    }

    public void PlaymenuBtn()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_MenuBtn);
    }

    public void PLaySubMenuIn()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_SubmenuIn);
    }

    public void PlaySubMenuOut()
    {
        UISoundManager.Instance.MusicOn(MusicType.MT_SubmenuOut);
    }
}
