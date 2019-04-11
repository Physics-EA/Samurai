using System;
using System.Collections.Generic;

public enum UIPanelID//所有UI的ID定义
{
    ID_None,
    ID_MainPanel,
    ID_PlayerPanel,
    ID_SettingPanel,
    ID_StopPanel,
    //ID_MainUIPanel
}

public enum UIShowMode
{
    M_Nothing,            //当前UI显示出来的时,其它UI不做任何操作
    M_HideAllExceptAbove, //当前UI显示出来的时,除了最前面的主UI外，都隐藏
    M_HideAll             //当前UI显示出来的时,隐藏所有的UI，包括最前面的
}

public class UIPrefabPath
{
    public static Dictionary<UIPanelID, string> prefabPathDict = new Dictionary<UIPanelID, string>
    {
        {UIPanelID.ID_MainPanel,"Prefabs/UIPanelPrefabs/MainPanel"},
        {UIPanelID.ID_PlayerPanel,"Prefabs/UIPanelPrefabs/PlayerPanel"},    
      //  {UIPanelID.ID_SettingPanel,"Prefabs/UIPanelPrefabs/SettingPanel"},
        {UIPanelID.ID_StopPanel,"Prefabs/UIPanelPrefabs/StopPanel"},
    };

    public static string getUIPrefabPath(UIPanelID id)
    {
        if (prefabPathDict.ContainsKey(id))
        {
            return prefabPathDict[id];
        }
        return null;
    }
}
