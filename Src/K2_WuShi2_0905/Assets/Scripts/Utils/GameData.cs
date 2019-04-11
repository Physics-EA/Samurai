using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameData
{
    //  游戏UI音乐模式
    public enum MusicType
    {
        MT_MenuBackground,
        MT_MainmenuIn,
        MT_MainmenuOut,
        MT_MainmenuBtn,
        MT_MenuBtn,
        MT_MenuLogoIn,
        MT_MenuLogoOut,
        MT_SubmenuIn,
        MT_SubmenuOut
    }

    //  游戏模式及难度选择
    public enum GameType
    {
        //  开始选择，4个选项
        GT_Continue,
        GT_Easy,
        GT_Middle,
        GT_Hard,
        //  道场模式，2个选项
        DT_Continue,
        DT_NewGame
    }
    public enum E_CurrentStatus
    {
        E_Idle,
        E_Alert,
        E_Move,
        E_Block,
        E_Attack,
        E_Injury,
        E_Death,
    }

    public class GameDate
    {
        //  按照枚举类型，游戏难度分为1.简单;2.中等;3.困难
        //  还有新开游戏的基本数据模板，当前游戏存档记录，当前道场模式存档记录
        public static void LoadGamePropsOnXML(GameType _type = GameType.GT_Easy)
        {
            switch (_type)
            {
                case GameType.GT_Continue:
                    break;
                case GameType.GT_Easy:
                    break;
                case GameType.GT_Middle:
                    break;
                case GameType.GT_Hard:
                    break;
                case GameType.DT_Continue:
                    break;
                case GameType.DT_NewGame:
                    break;
                default:
                    break;
            }
        }
    }
}
