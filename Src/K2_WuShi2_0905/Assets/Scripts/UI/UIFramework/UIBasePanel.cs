using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;//引入动画控制的命名空间

//补充 UI面板属性和行为 的基类
//在基类中，定义公共的属性和行为，以及定义好 面板继承链下 行为接口等。
//1、UI面板的ID
//2、UI面板的初始化
//3、UI面板的显示模式 --- 出现后，其他UI怎么办

//4、UI面板的出现和隐藏的效果方法a
//5、UI面板是否始终 最前前面

/// 2：显示UI
/// 3：隐藏UI
/// 4：数据和UI控件的初始化
/// 5：UI的层级关系
/// 6：UI的显示模式（当前UI显示的时候其它UI要显示还是隐藏）
/// 7：UI显示的动画效果（进来，出去）
/// 

public class UIBasePanel : MonoBehaviour
{
    public UIPanelID id = UIPanelID.ID_None;
    public UIShowMode showMode = UIShowMode.M_Nothing;

    //用于设置UIPanel始终在前面的C#属性

    public bool IsAlwaysAbove { get; set; }

    //类似继承链 下的构造函数,用来初始化工作
    protected virtual void Awake()
    {
        OnInit();
    }
    public virtual void OnInit()//1：用来初始化一些数据和UI组件
    {

    }

    //接口
    public virtual void Show()//2：显示UI时候调用的方法
    {
        this.gameObject.SetActive(true);//直接隐藏
    }
    public virtual void Hide()//3：隐藏UI时候调用的方法
    {
        this.gameObject.SetActive(false);//直接隐藏
    }
}
