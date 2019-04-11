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

        //  ֻ�е��ھ���״̬�£��Ż���о����ʱ
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

        //  ����������ʱ�䣬�л�Ϊ�ƶ�״̬ 
        if (alertStartTime > alertTime)
        {
            Owner.Status = E_CurrentStatus.E_Move;
            alertStartTime = 0;
        }

        if (Owner.Status == E_CurrentStatus.E_Move)
        {
            //  �������ƶ�״̬ʱ���жϵ����Ƿ�����Ұ��Χ�ڣ������׷�����ˣ�������ڣ����ص�Idl״̬������֮ǰ����Ϊ
             if (InView())
            {
                ActionGoTo actionGoto = ActionFactory.Create(ActionFactory.E_Type.E_GOTO) as ActionGoTo;

                Owner.BlackBoard.MoveDir = targetDir.normalized; //  �����ƶ�����
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

        //  ֻҪ�ڹ��������ڣ���ֱ���л�������״̬
        if (InAttackView() && Owner.Status == E_CurrentStatus.E_Move)
        {
            //  ���������ʽ
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

        //  ��������˻��߸�״̬����0.3s��Idle
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


    //  �ж��Ƿ���뾯������
    private bool Alert()
    {
        //  ���Ŀ��λ��
        Transform target = Player.Instance.transform;

        if ((transform.position - target.position).sqrMagnitude < sqrEyeRange)  //  ʹ��ƽ�����ٿ����ŵļ���
        {
            targetDir = target.position - transform.position;
            return true;
        }

        return false;
    }

    //  �ж��Ƿ��ڹ�����Ұ��
    private bool InView()
    {
        //  ���Ŀ��λ��
        Transform target = Player.Instance.transform;

        if ((transform.position - target.position).sqrMagnitude < sqrEyeRange)  //  ʹ��ƽ�����ٿ����ŵļ���
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

    //  �ж��Ƿ���빥����Χ��,�ڵ�����Ұ��Χ�ڣ������ڹ���������
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