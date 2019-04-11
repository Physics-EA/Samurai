using System;
using System.Collections.Generic;
using UnityEngine;

public static class ActionFactory
{
    public enum E_Type
    {
        E_IDLE,    
        E_MOVE,
        E_GOTO,
        E_COMBAT_MOVE,  //  战斗移动
        E_ATTACK,
        E_ATTACK_ROLL,
        E_ATTACK_JUMP,
        E_ATTACK_WHIRL, //  攻击回旋
        E_INJURY,
        E_DAMAGE_BLOCKED,   //  受伤格挡
        E_BLOCK,
        E_ROLL,
        E_INCOMMING_ATTACK,
        E_WEAPON_SHOW,
        E_Rotate,
        E_USE_LEVER,
        E_PLAY_ANIM,
        E_PLAY_IDLE_ANIM,
        E_DEATH,
        E_KNOCKDOWN,    //  击倒
        E_Teleport,     //  人物消亡动画
        E_COUNT     //  动作数量
    }

    //  记录暂时没有使用的动作
    static private Queue<ActionBase>[] m_UnusedActions = new Queue<ActionBase>[(int)E_Type.E_COUNT];

    // DEBUG !!!!!!!
    // 记录执行的动作   
    static private List<ActionBase> m_ActionsInAction = new List<ActionBase>();

    static ActionFactory()
    {
        //  初始化动作
        for (E_Type i = 0; i < E_Type.E_COUNT; i++)
        {
            m_UnusedActions[(int)i] = new Queue<ActionBase>();
        }
    }

    //  生成动作
    static public ActionBase Create(E_Type type)
    {
        int index = (int)type;

        ActionBase a;
        if (m_UnusedActions[index].Count > 0)
        {
            a = m_UnusedActions[index].Dequeue();
        }
        else
        {
            switch (type)
            {
                case E_Type.E_IDLE:
                    a = new ActionIdle();
                    break;
                case E_Type.E_MOVE:
                    a = new ActionMove();
                    break;
                case E_Type.E_GOTO:
                    a = new ActionGoTo();
                    break;
                case E_Type.E_COMBAT_MOVE:
                    a = new ActionCombatMove();
                    break;
                case E_Type.E_ATTACK:
                    a = new ActionAttack();
                    break;
                case E_Type.E_ATTACK_ROLL:
                    a = new ActionAttackRoll();
                    break;
                case E_Type.E_ATTACK_WHIRL:
                    a = new ActionAttackWhirl();
                    break;
                case E_Type.E_INJURY:
                    a = new ActionInjury();
                    break;
                case E_Type.E_DAMAGE_BLOCKED:
                    a = new ActionDamageBlocked();
                    break;
                case E_Type.E_BLOCK:
                    a = new ActionBlock();
                    break;
                case E_Type.E_ROLL:
                    a = new ActionRoll();
                    break;
                case E_Type.E_INCOMMING_ATTACK:
                    a = new ActionIncommingAttack();
                    break;
                case E_Type.E_WEAPON_SHOW:
                    a = new ActionWeaponShow();
                    break;
                case E_Type.E_Rotate:
                    a = new ActionRotate();
                    break;
                case E_Type.E_USE_LEVER:
                    a = new ActionUseLever();
                    break;
                case E_Type.E_PLAY_ANIM:
                    a = new ActionPlayAnim();
                    break;
                case E_Type.E_PLAY_IDLE_ANIM:
                    a = new ActionPlayIdleAnim();
                    break;
                case E_Type.E_DEATH:
                    a = new ActionDeath();
                    break;
                case E_Type.E_KNOCKDOWN:
                    a = new ActionKnockdown();
                    break;
                case E_Type.E_Teleport:
                    a = new ActionTeleport();
                    break;
                default:
                    Debug.LogError("no Action to create");
                    return null;
            }
        }
        a.Reset();
        a.SetActive();

        m_ActionsInAction.Add(a);
        return a;
    }

    //  回收动作
    static public void Return(ActionBase action)
    {
        action.SetUnused();

        m_UnusedActions[(int)action.Type].Enqueue(action);
        m_ActionsInAction.Remove(action);
    }
    //  提交显示当前动作列表
    static public void Report()
    {
        Debug.Log("Action Factory m_ActionsInAction " + m_ActionsInAction.Count);

        for (int i = 0; i < m_ActionsInAction.Count; i++)
        {
            Debug.Log(m_ActionsInAction[i].Type);
        }         
    }
}


