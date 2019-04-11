using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionHandler
{
    void HandleAction(ActionBase a);
}

[System.Serializable]
public class BlackBoard 
{
    //////////////// AGENT ACTIONS ///////////////////////
    private List<ActionBase> m_ActiveActions = new List<ActionBase>();
    private List<IActionHandler> m_ActionHandlers = new List<IActionHandler>();

    [System.NonSerialized]
    public Agent Owner;
    [System.NonSerialized]
    public GameObject myGameObject;// { get { return myGameObject; } private set { myGameObject = value; } }

    /////////////// STATES ////////////////////////////
    [System.NonSerialized]
    public bool _Stop = false;

    public bool Stop { get { return _Stop; } set { _Stop = value; if (IsPlayer) Player.Instance.StopMove(value); } }
    [System.NonSerialized]
    public E_MotionType MotionType = E_MotionType.None;
    [System.NonSerialized]
    public E_WeaponState WeaponState = E_WeaponState.NotInHands;
    [System.NonSerialized]
    public E_WeaponType WeaponSelected = E_WeaponType.Katana;
    [System.NonSerialized]
    public E_WeaponType WeaponToSelect = E_WeaponType.None;
    [System.NonSerialized]
    public bool IsPlayer = false;

    public float WeaponRange = 2;
    public float sqrWeaponRange { get { return WeaponRange * WeaponRange; } }

    public float CombatRange = 4;
    public float sqrCombatRange { get { return CombatRange * CombatRange; } }

    //////////////// INIT STATS /////////////////////////
    public float MaxSprintSpeed = 8;
    public float MaxRunSpeed = 4;
    public float MaxWalkSpeed = 1.5f;
    public float MaxCombatMoveSpeed = 1;
    public float MaxHealth = 100;
    public float MaxKnockdownTime = 4;

    ///////////////// SETTINGS /////////////////////////////
    public float SpeedSmooth = 2.0f;
    public float RotationSmooth = 2.0f;
    public float RotationSmoothInMove = 8.0f;
    public float RollDistance = 4.0f;
    public float MoveSpeedModifier = 1;

    public float DontAttackTimer = 2.0f;

    // Damage settings
    public bool KnockDown = true;
    public bool KnockDownDamageDeadly = true;
    public bool Invulnerable = false;
    public bool CriticalAllowed = true;
    public bool DamageOnlyFromBack = false;
    public float CriticalHitModifier = 1;

    /////////////// COMBAT SETTINGS ///////////////////////
    public float RageMin = 0; //0 = no attack
    public float RageMax = 0;// 100 % chance is do do attack
    public float RageModificator = 0;//per second
    public float DodgeMin = 0;
    public float DodgeMax = 0;
    public float DodgeModificator = 0; //per second
    public float FearMin = 0;
    public float FearMax = 0;
    public float FearModificator = 0; //per second
    public float BerserkMin = 0; //0 = no attack
    public float BerserkMax = 0;// 100 % chance is do do attack
    public float BerserkModificator = 0;//per second

    public float RageInjuryModificator = 0; // each injury increase rage
    public float DodgeInjuryModificator = 0;  // each injury increase dodge
    public float FearInjuryModificator = 0;
    public float BerserkInjuryModificator = 0;

    public float RageBlockModificator = 0; // each block increase rage
    public float FearBlockModificator = 0; // each block increase rage
    public float BerserkBlockModificator = 0; // each block increase rage

    public float DodgeAttackModificator = 0; // each attack increase rage
    public float FearAttackModificator = 0; // each attack increase rage


    ///////////////// STATS /////////////////////////////
    [System.NonSerialized]
    public float Speed = 0;
    [System.NonSerialized]
    public float Health = 100;

    //main AI parameters
    [System.NonSerialized]
    public float Rage = 0;
    [System.NonSerialized]
    public float Fear = 0;
    [System.NonSerialized]
    public float Dodge = 0;
    [System.NonSerialized]
    public float Berserk = 0;
    [System.NonSerialized]
    public float IdleTimer = 0;
    [System.NonSerialized]
    public Vector3 MoveDir;


    /// <summary>
    /// Aditional informations
    /// </summary>
    /// 
    [System.NonSerialized]
    public Vector3 DesiredPosition;
    [System.NonSerialized]
    public Vector3 DesiredDirection;
    [System.NonSerialized]
    public TriggerObject InteractionObject;
    [System.NonSerialized]
    public E_InteractionType Interaction;
    [System.NonSerialized]
    public string DesiredAnimation;
    [System.NonSerialized]
    public Agent DesiredTarget;
    [System.NonSerialized]
    public E_AttackType DesiredAttackType;
    [System.NonSerialized]
    public AnimAttackData DesiredAttackPhase;
    [System.NonSerialized]
    public Agent DesiredAttacker;

    [System.NonSerialized]
    public Agent Attacker;
    [System.NonSerialized]
    public E_WeaponType AttackerWeapon;
    [System.NonSerialized]
    public E_DamageType DamageType;
    [System.NonSerialized]
    public Vector3 Impuls;
    [System.NonSerialized]
    public E_LookType LookType;
    [System.NonSerialized]
    public E_MoveType MoveType;
    [System.NonSerialized]
    public float DistanceToTarget;
    [System.NonSerialized]
    public Teleport TeleportDestination;
    [System.NonSerialized]
    public Agent DangerousFriend;
    [System.NonSerialized]
    public float DistanceToDangerousFriend;

    [System.NonSerialized]
    public bool DontUpdate = true;
    [System.NonSerialized]
    public bool ReactOnHits = true;

    public void Reset()
    {
        m_ActiveActions.Clear();

        //  Stop = false;
        MotionType = E_MotionType.None;
        WeaponState = E_WeaponState.NotInHands;
        WeaponToSelect = E_WeaponType.None;

        Speed = 0;

        Health = MaxHealth;

        Rage = RageMin;
        Dodge = DodgeMin;
        Fear = FearMin;
        IdleTimer = 0;

        MoveDir = Vector3.zero;

        DesiredPosition = Vector3.zero;
        DesiredDirection = Vector3.zero;

        InteractionObject = null;
        Interaction = E_InteractionType.None;

        DesiredAnimation = "";

        DesiredTarget = null;
        DesiredAttackType = E_AttackType.None;

        DontUpdate = false;

    }

    //////////////// ORDERS /////////////////////////

    public bool IsOrderAddPossible(AgentOrder.E_OrderType orderType)
    {
        //  添加的一个判断，如果是停止动作，就可以直接打断
        //if (orderType == AgentOrder.E_OrderType.E_STOPMOVE)
        //{
        //    return true;
        //}
        //  获取到记录的order状态
        AgentOrder.E_OrderType currentOrder = Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER).GetOrder();

        //  当前状态不是闪避，攻击，和使用道具，返回真
        //  当前状态是闪避，记录的状态不是闪避和使用道具，返回真
        if(orderType == AgentOrder.E_OrderType.E_DODGE && currentOrder != AgentOrder.E_OrderType.E_DODGE && currentOrder != AgentOrder.E_OrderType.E_USE)
            return true;
        else if (currentOrder != AgentOrder.E_OrderType.E_ATTACK && currentOrder != AgentOrder.E_OrderType.E_DODGE && currentOrder != AgentOrder.E_OrderType.E_USE)
            return true;
        else
            return false;
    }

    public void OrderAdd(AgentOrder order)
    {
        //  Debug.Log(Time.timeSinceLevelLoad + " order arrived " + order.Type);

        if (IsOrderAddPossible(order.Type))
        {
            Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, order.Type);

            switch (order.Type)
            {
                case AgentOrder.E_OrderType.E_STOPMOVE:
                    Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
                    DesiredPosition = Owner.Position;
                    break;
                case AgentOrder.E_OrderType.E_GOTO:
                    Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, false);
                    DesiredPosition = order.Position;
                    DesiredDirection = order.Direction;
                    MoveSpeedModifier = order.MoveSpeedModifier;
                    break;
                case AgentOrder.E_OrderType.E_DODGE:
                    DesiredDirection = order.Direction;
                    //  Debug.Log(Time.timeSinceLevelLoad + " order arrived " + order.Type);
                    break;
                case AgentOrder.E_OrderType.E_USE:
                    Owner.WorldState.SetWSProperty(E_PropKey.E_USE_WORLD_OBJECT, true);

                    if ((order.Position - Owner.Position).sqrMagnitude <= 1)
                        Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
                    else
                        Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, false);
						DesiredPosition = order.Position;
						InteractionObject = order.InteractionObject;
						Interaction = order.Interaction;
                    break;
                case AgentOrder.E_OrderType.E_ATTACK:
                    if (order.Target == null || (order.Target.Position - Owner.Position).magnitude <= (WeaponRange + 0.2f))
                        Owner.WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, true);
                    else
                        Owner.WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, false);

						DesiredAttackType = order.AttackType;
						DesiredTarget = order.Target;
						DesiredDirection = order.Direction;
						DesiredAttackPhase = order.AnimAttackData;
                    break;
            }

           // Debug.Log(Time.timeSinceLevelLoad + " order arrived " + order.Type);
        }
        else if (order.Type == AgentOrder.E_OrderType.E_ATTACK)
        {
            // Debug.Log(Time.timeSinceLevelLoad +  " " +order.Type + " is nto allowed because " + currentOrder);
        }
        AgentOrderFactory.Return(order);
    }

    //////////////// ACTIONS /////////////////////////

    public void ActionAdd(ActionBase action)
    {
        IdleTimer = 0;
        m_ActiveActions.Add(action);

        for (int i = 0; i < m_ActionHandlers.Count; i++)
            m_ActionHandlers[i].HandleAction(action);
    }

    public ActionBase ActionGet(int index)
    {
        return m_ActiveActions[index];
    }

    public ActionAttack ActionGetAttackAction()
    {
        for (int i = 0; i < m_ActiveActions.Count; i++)
            if (m_ActiveActions[i] is ActionAttack)
                return m_ActiveActions[i] as ActionAttack;

        return null;
    }

    public int ActionCount()
    {
        return m_ActiveActions.Count;
    }

    public void ActionHandlerAdd(IActionHandler handler)
    {
        for (int i = 0; i < m_ActionHandlers.Count; i++)
            if (m_ActionHandlers[i] == handler)
                return;

        m_ActionHandlers.Add(handler);
    }

    public void ActionHandlerRemove(IActionHandler handler)
    {
        m_ActionHandlers.Remove(handler);
    }

    public void Update()
    {
        IdleTimer += Time.deltaTime;

        for (int i = 0; i < m_ActiveActions.Count; i++)
        {
            if (m_ActiveActions[i].IsActive())
                continue;

            ActionDone(m_ActiveActions[i]);
            m_ActiveActions.RemoveAt(i);

            return;
        }

        if(DesiredTarget && DesiredTarget.IsAlive == false)
            DesiredTarget = null;

        if (DesiredTarget == null)
            Owner.WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, true);
        else if ((DesiredTarget.Position - Owner.Position).magnitude <= (WeaponRange + 0.2f))
            Owner.WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, true);
        else
            Owner.WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, false);
    }

    private void ActionDone(ActionBase action)
    {
        if(action.IsSuccess())
        {
            if (action is ActionGoTo && (action as ActionGoTo).FinalPosition == DesiredPosition)
            {
                Debug.Log(action.ToString() + "is done, setting E_AT_TARGET_POS to true"); 
                Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
            }
            else if (action is ActionWeaponShow)
            {
                Debug.Log(action.ToString() + "is done, setting E_WEAPON_IN_HANDS to " + (action as ActionWeaponShow).Show.ToString()); 
                Owner.WorldState.SetWSProperty(E_PropKey.E_WEAPON_IN_HANDS, (action as ActionWeaponShow).Show);
            }
            else if (action is ActionUseLever)
            {
                Owner.WorldState.SetWSProperty(E_PropKey.E_USE_WORLD_OBJECT, false);
                InteractionObject = null;
                Interaction = E_InteractionType.None;
                
            }
            else if (action is ActionPlayAnim)
            {
                Owner.WorldState.SetWSProperty(E_PropKey.E_PLAY_ANIM, false);
                DesiredAnimation = null;
            }
        }

        ActionFactory.Return(action);
    }


    public void SetFear(float value)
    {
        Fear = value;
        if (Fear > FearMax)
            Fear = FearMax;
        else if (Fear < FearMin)
            Fear = FearMin;
    }

    public void SetRage(float value)
    {
        Rage = value;
        if (Rage > RageMax)
            Rage = RageMax;
        else if (Rage < RageMin)
            Rage = RageMin;
    }

    public void SetBerserk(float value)
    {
        Berserk = value;
        if (Berserk > BerserkMax)
            Berserk = BerserkMax;
        else if (Berserk < BerserkMin)
            Berserk = BerserkMin;
    }

    public void SetDodge(float value)
    {
        Dodge = value;
        if (Dodge > DodgeMax)
            Dodge = DodgeMax;
        else if (Dodge < DodgeMin)
            Dodge = DodgeMin;
    }

    public void UpdateCombatSetting()
    {
        //if (Game.Instance.GameDifficulty == E_GameDifficulty.Hard && IsPlayer == false)
        //{
        //    SetRage(Rage + RageModificator * 1.2f * Time.fixedDeltaTime);
        //    SetBerserk(Berserk + BerserkModificator * 1.2f * Time.fixedDeltaTime);
        //}
        //else if (Game.Instance.GameDifficulty == E_GameDifficulty.Easy && IsPlayer == false)
        //{
        //    SetRage(Rage + RageModificator * 0.8f * Time.fixedDeltaTime);
        //    SetBerserk(Berserk + BerserkModificator * 0.8f * Time.fixedDeltaTime);
        //}
        //else
        //{
            SetRage(Rage + RageModificator * Time.fixedDeltaTime);
            SetBerserk(Berserk + BerserkModificator * Time.fixedDeltaTime);
        //}


        if (DesiredTarget && Owner.WorldState.GetWSProperty(E_PropKey.E_AHEAD_OF_ENEMY).GetBool())
            SetFear(Fear + FearModificator * Time.fixedDeltaTime);
        else
            SetFear(Fear - FearModificator * Time.fixedDeltaTime);

        if (Owner.WorldState.GetWSProperty(E_PropKey.E_IN_BLOCK).GetBool() != true)
            SetDodge(Dodge + Owner.BlackBoard.DodgeModificator * Time.fixedDeltaTime);
    }

    public void UpdateCombatSetting(ActionBase a)
    {
        if (a is ActionDamageBlocked)
        {
            if ((a as ActionDamageBlocked).BreakBlock)
            {
                SetFear(Fear + FearInjuryModificator * 0.5f);
                SetRage(Rage + RageInjuryModificator * 0.5f);
            }
            else
            {
                SetFear(Fear + FearBlockModificator);
                SetRage(Rage + RageBlockModificator);
                SetBerserk(Berserk + BerserkBlockModificator);

            }
        }
        else if (a is ActionInjury)
        {
            SetFear(Fear + FearInjuryModificator);
            SetRage(Rage + RageInjuryModificator);
            SetDodge(Dodge + DodgeInjuryModificator);
            SetBerserk(Berserk + BerserkInjuryModificator);
        }
        else if (a is ActionAttackWhirl || a is ActionAttackRoll)
        {
            SetBerserk(BerserkMin);
        }
        else if (a is ActionAttack)
        {
            SetRage(RageMin);// reset

            SetDodge(Dodge + DodgeAttackModificator);
            SetFear(Fear + FearAttackModificator);
            //SetBerserk(Berserk + BerserkAttackModificator);
        }
        else if (a is ActionBlock) // reset
        {
            SetDodge(DodgeMin);
        }
        else if (a is ActionCombatMove) //reset
        {
            if ((a as ActionCombatMove).MoveType == E_MoveType.Backward)
                SetFear(Owner.BlackBoard.FearMin);
        }
    }

}
