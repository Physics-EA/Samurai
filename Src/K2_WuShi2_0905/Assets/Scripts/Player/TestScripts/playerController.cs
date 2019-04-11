using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animation))]
public class playerController : MonoBehaviour 
{
    /*  设计思路:   目前暂时先做一个PC版操作的玩家控制
                     （控制操作快捷键可以自定义）
        1.  W,S控制人物前进和后退
        2.  A,D控制人物的当前方向
        3.  J,K,L分别控制轻攻，重攻，闪避（J,K各种组合键可以实现组合攻击效果），针对手游，所有按键操作相对简单。
            （第二套操作方式，键盘和鼠标配合操作，空格闪避，左键轻攻，右键重攻）
        4.  配音完善
        人物的一些操作属性：移动方向，当前状态，组合键判断时间间隔(先实现基本的攻击方式，然后再去考虑如何判断组合)
     */

    //  玩家的当前状态
    public enum Player_Status
    {
        PS_None = -1,
        PS_Idle,
        PS_Walk,
        PS_UseLever,
        PS_AttackX,
        PS_AttackO,
        PS_Roll,
    }

    [Header("玩家参数面板")]
    [HideInInspector]
    public Vector3 direction;
    [Tooltip("人物当前状态")]
    public Player_Status status;
    [Tooltip("人物移动速度")]
    public float moveSpeed = 3;
    [Tooltip("人物旋转速度")]
    public float rotateSpeed = 5;
    [Tooltip("人物判断时间")]
    public float judgeTime = 0.2f;
    [Tooltip("人物翻滚位移")]
    public float evadeDistance = 200;

    public static playerController Instance;

    private bool isStartCount = false;
    private float startCount;

    private CharacterController controller;
    private Animation anim;
	// Use this for initialization
	void Start () 
	{
        Instance = this;
        direction = transform.forward;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (isStartCount)
	    {
		    startCount+=Time.deltaTime;
	    }

        if (startCount >= judgeTime && !anim.isPlaying)
        {
            anim.CrossFadeQueued("idle");
            status = Player_Status.PS_Idle;
            isStartCount = false;
            startCount = 0;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
        direction = transform.TransformDirection(Vector3.forward);
        float curSpeed = moveSpeed * Input.GetAxis("Vertical");
        controller.SimpleMove(direction * curSpeed);

        if (Input.GetKeyDown(KeyCode.J) && anim.IsPlaying("idle"))
        {
            Debug.Log("左键攻击");
            status = Player_Status.PS_AttackX;
            anim.CrossFade("showSword");
            anim.CrossFadeQueued("attackX");
            anim.CrossFadeQueued("hideSword");
          
            isStartCount = true;
        }

        if (Input.GetKeyDown(KeyCode.K) && anim.IsPlaying("idle"))
        {
            Debug.Log("右键攻击");
            status = Player_Status.PS_AttackO;
            anim.CrossFade("showSword");
            anim.CrossFadeQueued("attackO");
            anim.CrossFadeQueued("hideSword");        

            isStartCount = true;
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("翻滚动作");
        //    anim.CrossFade("evadeSword");
        //    transform.Translate(transform.forward*Time.deltaTime*evadeDistance,Space.Self);
        //}

        if (status != Player_Status.PS_AttackO && status != Player_Status.PS_AttackX && status != Player_Status.PS_UseLever)
        {
            if (v > 0)
            {
                anim.Play("walk");
                status = Player_Status.PS_Walk;
            }
            if (v == 0)
            {
                anim.Play("idle");
                status = Player_Status.PS_Idle;
            }
            if (v < 0)
            {
                anim.Play("walk");
                status = Player_Status.PS_Walk;
            }
        }  

	}
}
