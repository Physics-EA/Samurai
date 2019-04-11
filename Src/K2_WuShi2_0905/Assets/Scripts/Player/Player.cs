using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Agent))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(AnimSetPlayer))]
[RequireComponent(typeof(CameraOffsetBehaviour))]
[RequireComponent(typeof(AnimComponent))]
public class Player : MonoSingleton<Player>, IActionHandler
{
    //  状态机的参数面板（int,float,bool,trigger之类的参数类型），负责玩家的输入指令（不同的操作来修改参数面板的值）与状态机的交互过程  
    //  需要编写每种动画状态的条件控制，不同的人物状态对应不同的人物状态（可以一个基类，然后派生各种需要的动作）
    //  需要实现动画之间的自动切换机制，就是状态机中的过渡条件编写
    //  还需要抽象每一个动画片段

    /*  人物的动画控制系统的设计思路：（参考源码的实现方法，学习框架结构）
     *  1.  需要实时获取有效的玩家按键，并进行记录（可以存放在一个List容器中）
     *  2.  根据玩家的操作来响应不同的玩家动画，音效及状态切换（连招的触发）
     *  3.  需要一个状态机来实时记录当前的玩家状态，下一个要切换的状态，默认的状态，及相应的一些切换条件处理
     *  4.  对每个角色需要有动画状态的初始参数设置，及其所拥有的所有状态
     *  5.  对动画状态类进行抽象和实现，包括动画的播放，粒子特效，状态情况，激活状态等
     *  6.  需要考虑角色的视野范围，视野距离等
     *  7.  玩家设计成单例，方便后面的数据交互
     */

    //  玩家的基本属性：当前的血量，最大血量值，金币值，当前已学的攻击方式，可以使用的所有攻击方式
    //  技能攻击学习的条件，连招的等级，武器的等级

    //  连招步骤
    public class ComboStep
    {
        public E_AttackType AttackType;
        public E_ComboLevel ComboLevel;
        public AnimAttackData Data;    //  动画的攻击参数
    }

    //  组合连招
    public class Combo
    {
        public E_SwordLevel SwordLevel;
        public ComboStep[] ComboSteps;
    }

    public Combo[] playerComboAttacks = new Combo[6];
    //  连招的组合方式记录
    private List<E_AttackType> comboProgress = new List<E_AttackType>();
    private Queue<AgentOrder> bufferedOrders = new Queue<AgentOrder>();     //   缓存队列
    private Agent lastAttacketTarget;

    //   当前的玩家参数
    [HideInInspector]
    public Agent Owner;             //  当前玩家对象
    [HideInInspector]
    public bool useMode;            //  是否可用状态
    [HideInInspector]
    public int comboHitNum = 0;     //  玩家的连击次数
    [HideInInspector]
    public SpawnZone currentSpawnZone;  //  玩家当前所在的生成池位置
    [HideInInspector]
    public GameObject currentGameZone;  //  玩家当前坐在的游戏区域,用于可交互的对象管理

    private float stepTime;         //  间隔步长时间
    private float weaponTime = 2.5f;
    private float weaponStartTime;
    private Vector3 lastDir;
    public int coin;                //  当前的金币数量
    public float moveSpeed;

    public E_SwordLevel swordLevel = E_SwordLevel.Five; //  玩家当前剑的等级
    public E_ComboLevel[] comboLevels = new E_ComboLevel[6] { E_ComboLevel.Three, E_ComboLevel.Three, E_ComboLevel.Three ,
                                                              E_ComboLevel.Three,E_ComboLevel.Three,E_ComboLevel.Three   };  //  玩家当前的连招等级
    //  玩家的按键控制操作
    private PlayerControl control = new PlayerControl();
    //  玩家的动画设置
    private AnimSetPlayer AnimSet;
    private ActionBase currentAttackAction;  //  玩家当前的攻击行为
    public Agent Agent { get { return Owner; } }

    protected override void Awake()
    {
        lastDir = transform.forward;
        Instance = this;
        useMode = true;
        Owner = GetComponent<Agent>();
        AnimSet = GetComponent<AnimSetPlayer>();
    }
	// Use this for initialization
	void Start () 
	{
        //  初始化组合技能的按键方式，动作初始化参数，需要的等级信息
        playerComboAttacks[0] = new Combo() // FAST   Raisin Wave  浪翻
        {
            SwordLevel = E_SwordLevel.One,
            ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[0]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[1]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[2]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[3]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[4]},
            }
        };
        playerComboAttacks[1] = new Combo() // BREAK BLOCK  half moon   半月
        {
            SwordLevel = E_SwordLevel.One,
            ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[5]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[6]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[7]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[8]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[9]},
            }
        };
        playerComboAttacks[2] = new Combo() // CRITICAL  cloud cuttin   云切
        {
            SwordLevel = E_SwordLevel.Two,
            ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[5]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[6]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[17]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[18]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[19]},
            }
        };

        playerComboAttacks[3] = new Combo()  // flying dragon   飞龙
        {
            SwordLevel = E_SwordLevel.Three,
            ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[0]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[10]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[11]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[12]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[13]},
            }
        };
        playerComboAttacks[4] = new Combo() // KNCOK //walking death    踏死
        {
            SwordLevel = E_SwordLevel.Four,
            ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[0]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[1]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[14]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[15]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[16]},
            }
        };

        playerComboAttacks[5] = new Combo() // HEAVY, AREA  shogun death   破将
        {
            SwordLevel = E_SwordLevel.Five,
            ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[5]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[20]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[21]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[22]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[23]},
            }
        };

        //  玩家信息初始化
        Owner.BlackBoard.IsPlayer = true;
        Owner.BlackBoard.Rage = 0;
        Owner.BlackBoard.Dodge = 0;
        Owner.BlackBoard.Fear = 0;
        Owner.BlackBoard.Health = Owner.BlackBoard.MaxHealth;

        Owner.BlackBoard.ActionHandlerAdd(this);
        control.Start();
	}

    /*
     * // 按键名字，一共7个按键控制玩家操作
    public enum E_ButtonName
    {
        Up,
        Down,
        Left,
        Right,
        AttackX,
        AttackO,
        Roll,
        MAX
    }
     */

    void Activate(Transform t)
    {
        lastAttacketTarget = null;
        //LastTapTime = 0;
        stepTime = 0;

        Owner.BlackBoard.Reset();
        Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);

        Owner.WorldState.SetWSProperty(E_PropKey.E_IDLING, true);
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

        Owner.WorldState.SetWSProperty(E_PropKey.E_EVENT, E_EventTypes.None);

        comboProgress.Clear();
        ClearBufferedOrder();
    }

    void Deactivate()
    {
        ClearBufferedOrder();
    }

    // Update is called once per frame
	void Update () 
	{
        if (! Owner.IsAlive)
        {
            Debug.Log("Game Over");
            ActionDeath actionDeath = ActionFactory.Create(ActionFactory.E_Type.E_DEATH)as ActionDeath;
            GetComponent<AnimComponent>().HandleAction(actionDeath);

            StartCoroutine(BackToMainMenu());   
        }

        #region 测试代码

        //  Debug.Log(currentGameZone.name);

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    // 受伤动作

        //    ActionInjury actionInjury=ActionFactory.Create(ActionFactory.E_Type.E_INJURY)as ActionInjury;
        //    actionInjury.Attacker = null;
        //    actionInjury.DamageType = E_DamageType.Back;
        //    actionInjury.FromWeapon = E_WeaponType.Katana;

        //    GetComponent<AnimComponent>().HandleAction(actionInjury);

        //    //   死亡动作

        //    ActionDeath actionDeath = ActionFactory.Create(ActionFactory.E_Type.E_DEATH) as ActionDeath;
        //    actionDeath.Attacker = null;
        //    actionDeath.DamageType = E_DamageType.Back;
        //    actionDeath.FromWeapon = E_WeaponType.Katana;

        //    GetComponent<AnimComponent>().HandleAction(actionDeath);

        //    isDeath = true;
        //    Debug.Log("GameOver");

        //}

      	#endregion

        if (Owner.BlackBoard.Stop)
        {
            lastAttacketTarget = null;
            comboProgress.Clear();
            ClearBufferedOrder();
            CreateOrderStop();

            control.Update();
            return;
        }

        //  玩家操作更新
        control.Update();

        //  当玩家进行UI界面操作时，所有按键操作停止工作
        if (!useMode)
        {
            comboProgress.Clear();
            return;
        }

        //  方向控制不够灵活，后续再优化
        if (control.buttons[(int)PlayerControl.E_ButtonName.Up].status == PlayerControl.E_ButtonStatus.Down)
        {
            //  首先需要判断当前是否可以进行该操作
            //  玩家在进行闪避，攻击或者使用道具时，不能执行行走操作
            if (CanAddNewAction())
	        {
                //  Debug.Log("玩家向前移动");
                CreateOrderMove(Vector3.forward);
	        }     
        }

        if (control.buttons[(int)PlayerControl.E_ButtonName.Down].status == PlayerControl.E_ButtonStatus.Down)
        {        
            if (CanAddNewAction())
            {
                //  Debug.Log("玩家向后移动");
                CreateOrderMove(Vector3.back);
            }
        }
        if (control.buttons[(int)PlayerControl.E_ButtonName.Left].status == PlayerControl.E_ButtonStatus.Down)
        {
            if (CanAddNewAction())
            {
                //  Debug.Log("向左移动");
                CreateOrderMove(Vector3.left);
            }      
        }
        if (control.buttons[(int)PlayerControl.E_ButtonName.Right].status == PlayerControl.E_ButtonStatus.Down)
        {
            if (CanAddNewAction())
            {
                //  Debug.Log("向右移动");
                CreateOrderMove(Vector3.right);
            }
         
        }
        if (control.buttons[(int)PlayerControl.E_ButtonName.Roll].status == PlayerControl.E_ButtonStatus.Down)
        {
            //  Debug.Log("闪避动作");
            CreateOrderDodge();
        }
        if (control.buttons[(int)PlayerControl.E_ButtonName.AttackX].status == PlayerControl.E_ButtonStatus.Down)
        {
            //  Debug.Log("将X添加组合攻击链表");
            CreateOrderAttack(E_AttackType.X);
            
        }
        if (control.buttons[(int)PlayerControl.E_ButtonName.AttackO].status == PlayerControl.E_ButtonStatus.Down)
        {
            //  Debug.Log("将O添加组合攻击链表");
            CreateOrderAttack(E_AttackType.O);
        }

        //  当没有任何按键操作时，切回idle状态
        
        bool hasAnyKeyDown = false;
        for (int i = 0; i < control.buttons.Length; i++)
        {
            if (control.buttons[i].status == PlayerControl.E_ButtonStatus.Down)
            {
                hasAnyKeyDown = true;
            }
        }

        if (hasAnyKeyDown == false)
        {
            weaponStartTime += Time.deltaTime;
            if (weaponStartTime > weaponTime)
            {
                weaponStartTime = 0;
                ClearBufferedOrder();
                comboProgress.Clear();
                MainPanelCtrl.Instance.ComboProgressMessage(comboProgress);

                if (Owner.BlackBoard.WeaponState != E_WeaponState.NotInHands)
                {                 
                    GetComponent<Animation>().CrossFade("hideSword", 0.1f);
                    GetComponent<Animation>().CrossFadeQueued("idle", 0.1f);
                    Owner.SoundPlayWeaponOff();
                    Owner.BlackBoard.WeaponState = E_WeaponState.NotInHands;
                }
               
                CreateOrderStop();
            }
            else
            {
                CreateOrderStop();
            }
        }   
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

        //if (currentAttackAction != null && currentAttackAction.IsActive() == false)
        //{   // no continue in combos !!!
        //    if (bufferedOrders.Count == 0 && Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER).GetOrder() != AgentOrder.E_OrderType.E_ATTACK)
        //    {
        //        //Debug.Log("clear combo progress " + CurrentAttackAction.Data.AnimName);
        //        comboProgress.Clear();
        //    }
        //    currentAttackAction = null;
        //}
    }

    public void OnTriggerEnter(Collider other)
    {
        TriggerObject interaction = other.GetComponent<TriggerObject>();

        if (interaction != null)
        {
            if (interaction.GetEntryTransform() == null)
            {
                return;
            }
            
            //  当玩家碰到触发器，播放使用动画
            Owner.transform.position = interaction.GetEntryTransform().position;
            Owner.transform.rotation = interaction.GetEntryTransform().rotation;

            useMode = false;

            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_USE);

            order.InteractionObject = interaction;
            order.Position = order.InteractionObject.GetEntryTransform().position;
            order.Interaction = E_InteractionType.On;
            Owner.BlackBoard.OrderAdd(order);

            ActionUseLever actionUseLever = ActionFactory.Create(ActionFactory.E_Type.E_USE_LEVER) as ActionUseLever;
            actionUseLever.InterObj = interaction;
            actionUseLever.Interaction = E_InteractionType.On;

            GetComponent<AnimComponent>().HandleAction(actionUseLever);

            return;
        }
    }

    private bool CanAddNewAction()
    {
        if (useMode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CreateOrderMove(Vector3 direction)
    {
        if (CouldAddnewOrder() == false)
        {
            //  Debug.Log("不能添加动作");
            return;
        }

        if (lastDir != null)
        {
            direction = (direction + lastDir).normalized;
            lastDir = direction;
        }

        AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_GOTO);
        order.Direction = direction;
        order.MoveSpeedModifier = moveSpeed;
        Owner.BlackBoard.OrderAdd(order);
        Owner.BlackBoard.DesiredDirection = direction;

        ActionMove actionMove = ActionFactory.Create(ActionFactory.E_Type.E_MOVE) as ActionMove;

        GetComponent<AnimComponent>().HandleAction(actionMove); 
    }

    private void CreateOrderStop()
    {
        AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_STOPMOVE);
        Owner.BlackBoard.OrderAdd(order);

        GetComponent<AnimComponent>().HandleAction(ActionFactory.Create(ActionFactory.E_Type.E_IDLE));
    }

    //  攻击动作
    private void CreateOrderAttack(E_AttackType type)
    {
        if (CouldBufferNewOrder() == false && CouldAddnewOrder() == false)
        {
            //  Debug.Log(Time.timeSinceLevelLoad + " attack order rejected, already buffered one ");
            return;
        }

        AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_ATTACK);
        order.Direction = Owner.Transform.forward;
        order.AnimAttackData = ProcessCombo(type);

        //  查找最优的目标对象
        order.Target = GetBestTarget(false);

        if (CouldAddnewOrder())
        {
            //Debug.Log("order " + (order.Target != null ? order.Target.name : "no target") + " " + order.AnimAttackData.AnimName);
            Owner.BlackBoard.OrderAdd(order);
        }
        else
        {
            //Debug.Log("order to queue " + (order.Target != null ? order.Target.name : "no target") + " " + order.AnimAttackData.AnimName);
            bufferedOrders.Enqueue(order);
        }

        ActionAttack attack = ActionFactory.Create(ActionFactory.E_Type.E_ATTACK) as ActionAttack;
        attack.Data = order.AnimAttackData;
        attack.AttackDir = Owner.Transform.forward;
        attack.Target = order.Target;
        Owner.BlackBoard.WeaponState = E_WeaponState.Ready;

        //  currentAttackAction = attack;
        weaponStartTime = 0;
        GetComponent<AnimComponent>().HandleAction(attack);
    }

    private AnimAttackData ProcessCombo(E_AttackType attackType)
    {
        if (attackType != E_AttackType.O && attackType != E_AttackType.X)
            return null;

        comboProgress.Add(attackType);

        //  如果攻击方式与连招匹配，同时玩家的等级都达到要求（默认调成最高等级，都满足）
        for (int i = 0; i < playerComboAttacks.Length; i++)
        {       
            Combo combo = playerComboAttacks[i];

            //  如果当前连招需要的剑的等级高于玩家的剑等级，直接跳过
            if (combo.SwordLevel > swordLevel)
                continue; 

            bool valid = comboProgress.Count <= combo.ComboSteps.Length; 
            for (int j = 0; j < comboProgress.Count && j < combo.ComboSteps.Length; j++)
            {
                if (comboProgress[j] != combo.ComboSteps[j].AttackType || combo.ComboSteps[j].ComboLevel > comboLevels[i])
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                combo.ComboSteps[comboProgress.Count - 1].Data.LastAttackInCombo = NextAttackIsAvailable(E_AttackType.X) == false && NextAttackIsAvailable(E_AttackType.O) == false;            
                combo.ComboSteps[comboProgress.Count - 1].Data.FirstAttackInCombo = false;
                combo.ComboSteps[comboProgress.Count - 1].Data.ComboIndex = i;
                combo.ComboSteps[comboProgress.Count - 1].Data.FullCombo = comboProgress.Count == combo.ComboSteps.Length;
                combo.ComboSteps[comboProgress.Count - 1].Data.ComboStep = comboProgress.Count;

                MainPanelCtrl.Instance.ComboProgressMessage(comboProgress);
                return combo.ComboSteps[comboProgress.Count - 1].Data;
            }
        }

        //  如果不满足，将连招缓存清空，重新开始记录
        comboProgress.Clear();
        comboProgress.Add(attackType);

        for (int i = 0; i < playerComboAttacks.Length; i++)
        {
            if (playerComboAttacks[i].ComboSteps[0].AttackType == attackType)
            {
                // Debug.Log(Time.timeSinceLevelLoad + " New combo " + i + " step " + comboProgress.Count);
                playerComboAttacks[i].ComboSteps[0].Data.FirstAttackInCombo = true;
                playerComboAttacks[i].ComboSteps[0].Data.LastAttackInCombo = false;
                playerComboAttacks[i].ComboSteps[0].Data.ComboIndex = i;
                playerComboAttacks[i].ComboSteps[0].Data.FullCombo = false;
                playerComboAttacks[i].ComboSteps[0].Data.ComboStep = 0;

                MainPanelCtrl.Instance.ComboProgressMessage(comboProgress);
                return playerComboAttacks[i].ComboSteps[0].Data;
            }
        }

        Debug.LogError("Could not find any combo attack !!! Some shit happens");

        return null;
    }

    private bool NextAttackIsAvailable(E_AttackType attackType)
    {
        if (attackType != E_AttackType.O && attackType != E_AttackType.X)
            return false;

        if (comboProgress.Count == 5) 
            return false;

        List<E_AttackType> progress = new List<E_AttackType>(comboProgress);

        progress.Add(attackType);

        for (int i = 0; i < playerComboAttacks.Length; i++)
        {
            Combo combo = playerComboAttacks[i];

            if (combo.SwordLevel >swordLevel)
                continue;

            bool valid = true;
            for (int j = 0; j < progress.Count; j++)
            {
                if (progress[j] != combo.ComboSteps[j].AttackType || combo.ComboSteps[j].ComboLevel > comboLevels[i])
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
                return true;
        }
        return false;
    }

    //  闪避动作
    private void CreateOrderDodge()
    {
        if (Owner.BlackBoard.IsOrderAddPossible(AgentOrder.E_OrderType.E_DODGE) == false)
            return;

        AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_DODGE);
        order.Direction = Owner.Transform.forward;
        Owner.BlackBoard.OrderAdd(order);

        GetComponent<AnimComponent>().HandleAction(ActionFactory.Create(ActionFactory.E_Type.E_ROLL) as ActionRoll);

        //  清空连招
        comboProgress.Clear();
        ClearBufferedOrder();

        //  清空画面显示
        MainPanelCtrl.Instance.ComboProgressMessage(comboProgress);
    }

    public void HealToFullHealth()
    {
        StartCoroutine(HealingUp());
    }

    //  回血
    IEnumerator HealingUp()
    {
        yield return new WaitForSeconds(1.5f);

        float healingHP = Owner.BlackBoard.MaxHealth - Owner.BlackBoard.Health + 1;

        //  PlayHealingSound();

        while (healingHP > 0)
        {
            float h = 33 * Time.deltaTime;
            if (healingHP - h < 0)
                h = healingHP;

            healingHP -= h;
            Owner.BlackBoard.Health += h;

            yield return new WaitForEndOfFrame();
        }

        if (Owner.BlackBoard.Health > Owner.BlackBoard.MaxHealth)
            Owner.BlackBoard.Health = Owner.BlackBoard.MaxHealth;
    }

    IEnumerator BackToMainMenu()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("MainMenu");
    }

    public Agent GetBestTarget(bool hasToBeKnockdown)
    {
         /*  查找到最佳的攻击目标，逻辑思路
         *  1.  首先需要当前场景的活着的敌人的数量（所以需要知道当前的场景区域位置，同时记录并实时获得当前场景中存活的敌人情况）
         *  2.  遍历当前的所有存活的敌人，判断敌人的状态（敌人可以会在无敌状态，就直接跳过）,计算到敌人的方向和具体的距离
         *  3.  简单点的就可以直接攻击最近距离的敌人对象
         */
        if (currentSpawnZone == null)
        {
            return null;
        }

        List<Agent> enemies = currentSpawnZone.EnemiesAlive;

        float[] EnemyFactor = new float[enemies.Count];
        Agent enemy;
        Vector3 dirToEnemy;

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyFactor[i] = 0;
            enemy = enemies[i];

            //  判断敌人是否被击倒
            if (hasToBeKnockdown && enemy.BlackBoard.MotionType != E_MotionType.Knockdown)
                continue;
            //  判断敌人是否处于无敌状态
            if (enemy.BlackBoard.Invulnerable)  
                continue;

            dirToEnemy = (enemy.Position - Owner.Position);

            float distance = dirToEnemy.magnitude;

            //  敌人距离在5米以内猜考虑
            if (distance > 5.0f)
                continue;

            dirToEnemy.Normalize();

            float angle = Vector3.Angle(dirToEnemy, Owner.Forward);

            if (enemy == lastAttacketTarget)
                EnemyFactor[i] += 0.1f;

            EnemyFactor[i] += 0.2f - ((angle / 180.0f) * 0.2f);

            EnemyFactor[i] += 0.2f - ((distance / 5) * 0.2f);
        }

        float bestValue = 0;
        int best = -1;

        for (int i = 0; i < enemies.Count; i++)
        {
            //     Debug.Log(Mission.Instance.CurrentGameZone.GetEnemy(i).name + " : " + EnemyCoeficient[i]); 
            if (EnemyFactor[i] <= bestValue)
                continue;

            best = i;
            bestValue = EnemyFactor[i];
        }

        if (best >= 0)
            return enemies[best];

        return null;
    }

    public void StopMove(bool stop)
    {
        if (stop)
        {
            control.enableInput = false;
        }
    }

    public bool CouldAddnewOrder()
    {
        AgentOrder.E_OrderType order = Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER).GetOrder();

        if (order == AgentOrder.E_OrderType.E_DODGE || order == AgentOrder.E_OrderType.E_ATTACK || order == AgentOrder.E_OrderType.E_USE)
            return false;

        ActionBase action;

        for (int i = 0; i < Owner.BlackBoard.ActionCount(); i++)
        {
            action = Owner.BlackBoard.ActionGet(i);

            if (action is ActionAttack && (action as ActionAttack).AttackPhaseDone == false)
                return false;
            else if (action is ActionRoll)
                return false;
            else if (action is ActionUseLever)
                return false;
            else if (action is ActionGoTo && (action as ActionGoTo).Motion == E_MotionType.Sprint)
                return false;
        }
        return true;
    }

    //  专门用于判断攻击连招
    public bool CouldBufferNewOrder()
    {
        return bufferedOrders.Count <= 0 && currentAttackAction != null;
    }

    public void HandleAction(ActionBase a)
    {
        if (a is ActionAttack)
        {
            currentAttackAction = a as ActionAttack;
            Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, true);
        }
        else if (a is ActionInjury)
        {
            Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);
            comboProgress.Clear();
            ClearBufferedOrder();
        }
        else if (a is ActionDeath)
        {
            Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);
            comboProgress.Clear();
            ClearBufferedOrder();
        }
    }

    public void ClearBufferedOrder()
    {
        while (bufferedOrders.Count > 0)
            AgentOrderFactory.Return(bufferedOrders.Dequeue());
    }

    public void Teleport(Teleport teleport)
    {
        Owner.BlackBoard.Stop = true;
        Owner.BlackBoard.TeleportDestination = teleport;
        Owner.WorldState.SetWSProperty(E_PropKey.E_TELEPORT, true);
        control.Reset();
    }
}
