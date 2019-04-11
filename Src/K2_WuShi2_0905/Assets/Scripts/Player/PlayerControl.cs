using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl 
{
    /*  该脚本只实现玩家战斗中的按键操作记录
     *  1. 按键可以玩家自定义设置；有一套默认的按键配置
     *  2. 记录每个按键的当前状态：通过按键的执行顺序和当前的状态来对应不同的玩家行为状态
     *  3. 监听当前的玩家按键操作，根据玩家的不同按键操作来执行不同的动画和行为
     */

    public enum E_ButtonStatus
    {
        UpFirst,
        Up,
        DownFirst,
        Down
    }

    // 按键名字，一共7个按键控制玩家操作
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

    public class MyButton
    {
        public E_ButtonStatus status = E_ButtonStatus.Up;   //  按键的初始状态
        public E_ButtonName name;     //  按键名字
        public KeyCode key;                 //  每个按键对应的键值
        public bool isEnable = true;        //  按键当前可用状态

        public MyButton(KeyCode _key,E_ButtonName _name)
        {
            key = _key;
            name=_name;
        }
    }

    public bool enableInput = true;
    public MyButton[] buttons;

	// Use this for initialization
	public void Start ()
    {
        //  按键信息初始化，后续也可以人为自定义
        buttons = new MyButton[7]{
            new MyButton(KeyCode.W,E_ButtonName.Up),
            new MyButton(KeyCode.S,E_ButtonName.Down),
            new MyButton(KeyCode.A,E_ButtonName.Left),
            new MyButton(KeyCode.D,E_ButtonName.Right),
            new MyButton(KeyCode.J,E_ButtonName.AttackX),
            new MyButton(KeyCode.K,E_ButtonName.AttackO),
            new MyButton(KeyCode.Space,E_ButtonName.Roll)
        };
    }
	
	// Update is called once per frame
	public void Update () 
	{
        if (enableInput == false)
        {
            return;
        }

        if (Input.anyKey)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (Input.GetKeyDown(buttons[i].key))
                {
                    buttons[i].status = E_ButtonStatus.DownFirst;
                }

                if (Input.GetKey(buttons[i].key))
                {
                    buttons[i].status = E_ButtonStatus.Down;
                }

                if (Input.GetKeyUp(buttons[i].key))
                {
                    buttons[i].status = E_ButtonStatus.UpFirst;
                }

                //  攻击也可以鼠标左右键操作（左键X，右键O）

                //if (buttons[i].name == E_ButtonName.AttackX)
                //{
                //    if (Input.GetMouseButtonDown(0))
                //    {
                //        buttons[i].status = E_ButtonStatus.DownFirst;
                //    }
                //    if (Input.GetMouseButton(0))
                //    {
                //        buttons[i].status = E_ButtonStatus.Down;
                //    }
                //    if (Input.GetMouseButtonUp(0))
                //    {
                //        buttons[i].status = E_ButtonStatus.Up;
                //    }
                //}

                //if (buttons[i].name == E_ButtonName.AttackO)
                //{
                //    if (Input.GetMouseButtonDown(1))
                //    {
                //        buttons[i].status = E_ButtonStatus.DownFirst;
                //    }
                //    if (Input.GetMouseButton(1))
                //    {
                //        buttons[i].status = E_ButtonStatus.Down;
                //    }
                //    if (Input.GetMouseButtonUp(1))
                //    {
                //        buttons[i].status = E_ButtonStatus.Up;
                //    }
                //}
            }
        }
        else
        {
            foreach (var item in buttons)
            {
                item.status = E_ButtonStatus.Up;
            }
        }
	}

    public void Reset()
    {
        enableInput = true;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].status = E_ButtonStatus.Up;
        }
    }
}
