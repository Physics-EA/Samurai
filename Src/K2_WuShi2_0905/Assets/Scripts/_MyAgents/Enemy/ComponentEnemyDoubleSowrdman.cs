using UnityEngine;using System.Collections;
using Assets.Scripts.GameData;[RequireComponent(typeof(Agent))][RequireComponent(typeof(Animation))][RequireComponent(typeof(AnimSetEnemyDoubleSwordsman))][RequireComponent(typeof(AudioSource))][RequireComponent(typeof(AnimComponent))]
public class ComponentEnemyDoubleSowrdman : MonoBehaviour, IActionHandler{
    Agent Owner;

    public float EyeRange = 6;
    public float FieldOfView = 120;
    public float alertTime;
    private float alertStartTime = 0;
    public float injuryTime = 0.3f;
    private float injuryStartTime = 0;
    private Vector3 targetDir;
    private Vector3 targetPos;
    private float stepTime;
    private bool isDeath = false;
    private AnimComponent animComponent;
    float sqrEyeRange { get { return EyeRange * EyeRange; } }
    public Agent Agent { get { return Owner; } }    void Awake()    {        Owner = GetComponent("Agent") as Agent;
        animComponent = GetComponent<AnimComponent>();
    }

    void Start()
    {
        Owner.BlackBoard.ActionHandlerAdd(this);
        Activate(Owner.Transform);
    }

    void Update()
    {
        if (Owner.BlackBoard.Health <= 0)
        {
            if (!isDeath)
            {
                StartCoroutine(PlayTeleport());
                isDeath = true;
            }         
            return;
        }
        else
        {
            Owner.CharacterController.SimpleMove(Vector3.zero);
        }

        //  只有当在警觉状态下，才会进行警戒计时
        if (Alert() && Owner.Status == E_CurrentStatus.E_Idle)
        {
            alertStartTime += Time.deltaTime;
            ActionRotate actionRotate = ActionFactory.Create(ActionFactory.E_Type.E_Rotate) as ActionRotate;
            actionRotate.Direction=targetDir;
            animComponent.HandleAction(actionRotate);
            Owner.BlackBoard.WeaponState = E_WeaponState.Ready;
        }
        else
        {
            ActionIdle actionIdle = ActionFactory.Create(ActionFactory.E_Type.E_IDLE) as ActionIdle;
            animComponent.HandleAction(actionIdle);
            alertStartTime = 0;
            Owner.BlackBoard.WeaponState = E_WeaponState.NotInHands;
        }

        //  当超过警觉时间，切换为移动状态 
        if (alertStartTime > alertTime)
        {
            Owner.Status = E_CurrentStatus.E_Move;
            alertStartTime = 0;
        }

        if (Owner.Status == E_CurrentStatus.E_Move)
        {
            //  当进入移动状态时，判断敌人是否在视野范围内，如果是追击敌人，如果不在，返回到Idl状态，接着之前的行为
             if (InView())
            {
                ActionGoTo actionGoto = ActionFactory.Create(ActionFactory.E_Type.E_GOTO) as ActionGoTo;

                Owner.BlackBoard.MoveDir = targetDir.normalized; //  更新移动方向
                Owner.BlackBoard.WeaponState = E_WeaponState.Ready;
                
                actionGoto.FinalPosition = targetPos;
                actionGoto.MoveType = E_MoveType.Forward;
                actionGoto.Motion = E_MotionType.Sprint;
                animComponent.HandleAction(actionGoto);             
            }
            else
            {
                Owner.Status = E_CurrentStatus.E_Idle;
                Owner.BlackBoard.WeaponState = E_WeaponState.NotInHands;
            }
        }

        //  只要在攻击距离内，就直接切换到攻击状态
        if (InAttackView() && Owner.Status == E_CurrentStatus.E_Move)
        {
            //  随机攻击方式
            int random=Random.Range(0,10);

            if (random < 2)
            {
                ActionAttack actionAttack = ActionFactory.Create(ActionFactory.E_Type.E_ATTACK) as ActionAttack;
                actionAttack.Target = Player.Instance.Agent;
                actionAttack.AttackDir = Player.Instance.transform.position - transform.position;
                actionAttack.Data = GetComponent<AnimSetEnemyDoubleSwordsman>().GetFirstAttackAnim(E_WeaponType.Katana,E_AttackType.X);

                animComponent.HandleAction(actionAttack);
            }
            else if(random < 5)
            {
                ActionAttack actionAttack = ActionFactory.Create(ActionFactory.E_Type.E_ATTACK) as ActionAttack;
                actionAttack.Target = Player.Instance.Agent;
                actionAttack.AttackDir = Player.Instance.transform.position - transform.position;
                actionAttack.Data = GetComponent<AnimSetEnemyDoubleSwordsman>().GetFirstAttackAnim(E_WeaponType.Katana, E_AttackType.O);

                animComponent.HandleAction(actionAttack);             
            }
            else
            {
                ActionAttackWhirl actionAttackWhirl = ActionFactory.Create(ActionFactory.E_Type.E_ATTACK_WHIRL) as ActionAttackWhirl;
                actionAttackWhirl.Data = GetComponent<AnimSetEnemyDoubleSwordsman>().GetWhirlAttackAnim();
                animComponent.HandleAction(actionAttackWhirl);
            }

            Owner.WorldState.SetWSProperty(E_PropKey.E_IDLING, false);
            Owner.Status = E_CurrentStatus.E_Attack;
            
        }

        if (Owner.Status == E_CurrentStatus.E_Attack && Owner.WorldState.GetWSProperty(E_PropKey.E_IDLING).GetBool())
        {
            Owner.Status = E_CurrentStatus.E_Idle;
        }

        //  如果在受伤或者格挡状态，隔0.3s回Idle
        if (Owner.Status == E_CurrentStatus.E_Injury || Owner.Status == E_CurrentStatus.E_Block)
        {
            injuryStartTime += Time.deltaTime;
        }

        if (injuryStartTime > injuryTime)
        {
            Owner.Status = E_CurrentStatus.E_Idle;
            injuryStartTime = 0;
        }
    }


    //  判断是否进入警觉距离
    private bool Alert()
    {
        //  获得目标位置
        Transform target = Player.Instance.transform;

        if ((transform.position - target.position).sqrMagnitude < sqrEyeRange)  //  使用平方减少开根号的计算
        {
            targetDir = target.position - transform.position;
            return true;
        }

        return false;
    }

    //  判断是否在攻击视野内
    private bool InView()
    {
        //  获得目标位置
        Transform target = Player.Instance.transform;

        if ((transform.position - target.position).sqrMagnitude < sqrEyeRange)  //  使用平方减少开根号的计算
        {
            float angle = Vector3.Angle(transform.forward, target.position-transform.position);

            if (angle < FieldOfView/2)
            {
                targetDir = target.position - transform.position;
                targetPos = target.position;
                return true;
            }
            else
            {
                targetDir = Vector3.zero;
                return false;
            }
        }

        return false;
    }

    //  判断是否进入攻击范围内,在敌人视野范围内，并且在攻击距离内
    private bool InAttackView()
    {
        if (InView())
        {
            if ((transform.position-Player.Instance.transform.position).magnitude < Owner.BlackBoard.WeaponRange + 0.2f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    void FixedUpdate()
    {
       // Debug.Log("Paesant Rage " + Owner.BlackBoard.Rage + " Fear " + Owner.BlackBoard.Fear + " Dodge " + Owner.BlackBoard.Dodge + " Berserk " + Owner.BlackBoard.Berserk);
        Owner.BlackBoard.UpdateCombatSetting();
    }

    void LateUpdate()
    {
        if (stepTime < Time.timeSinceLevelLoad)
        {
            if (Owner.BlackBoard.MotionType == E_MotionType.Run)
            {
                Owner.SoundPlayStep();
                stepTime = Time.timeSinceLevelLoad + Random.Range(0.25f, 0.28f);
            }
            else if (Owner.BlackBoard.MotionType == E_MotionType.Walk)
            {
                Owner.SoundPlayStep();
                stepTime = Time.timeSinceLevelLoad + Random.Range(0.40f, 0.43f);
            }
        }
    }

    public void HandleAction(ActionBase a)
    {
        if (a is ActionInjury)
        {
            Owner.BlackBoard.SetFear(Owner.BlackBoard.Fear + Owner.BlackBoard.FearInjuryModificator);
            Owner.BlackBoard.SetRage(Owner.BlackBoard.Rage + Owner.BlackBoard.RageInjuryModificator);
        }
        else if (a is ActionAttack)
        {
            Owner.BlackBoard.SetRage(Owner.BlackBoard.RageMin);
            Owner.BlackBoard.SetFear(Owner.BlackBoard.Fear + Owner.BlackBoard.FearAttackModificator);
        }
        else if (a is ActionCombatMove)
        {
            if ((a as ActionCombatMove).MoveType == E_MoveType.Backward)
                Owner.BlackBoard.SetFear(Owner.BlackBoard.FearMin);
        }
    }    void Activate(Transform t)    {
        Owner.BlackBoard.Rage = Owner.BlackBoard.RageMin;

        Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);

        Owner.WorldState.SetWSProperty(E_PropKey.E_IDLING, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
        Owner.WorldState.SetWSProperty(E_PropKey.E_ATTACK_TARGET, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_LOOKING_AT_TARGET, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_USE_WORLD_OBJECT, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_PLAY_ANIM, false);

        Owner.WorldState.SetWSProperty(E_PropKey.E_IN_DODGE, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_WEAPON_IN_HANDS, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_IN_BLOCK, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_IN_COMBAT_RANGE, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_AHEAD_OF_ENEMY, false);
        Owner.WorldState.SetWSProperty(E_PropKey.E_BEHIND_ENEMY, false);
        Owner.WorldState.SetWSProperty(E_PropKey.MoveToRight, false);
        Owner.WorldState.SetWSProperty(E_PropKey.MoveToLeft, false);

        Owner.WorldState.SetWSProperty(E_PropKey.E_EVENT, E_EventTypes.None);    }
    IEnumerator PlayTeleport()
    {
        yield return new WaitForSeconds(4);

        CombatEffectsManager.Instance.PlayDisappearEffect(Owner.Transform.position, Owner.Forward);
        Destroy(Owner.gameObject, 0.1f);
    }}