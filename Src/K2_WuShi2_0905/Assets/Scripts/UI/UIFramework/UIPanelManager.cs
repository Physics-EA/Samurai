using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class UIPanelManager : MonoSingleton<UIPanelManager>
{
    //当前正在显示UI面板
    [HideInInspector]
    public UIBasePanel curDisplayUIPanel;

    //管理 - 所有UIPanel面板对象,不用来缓冲对象，慢慢收集出现的UIPanel对象
    public Dictionary<UIPanelID, UIBasePanel> allUIPanelDict = new Dictionary<UIPanelID, UIBasePanel>();

    //管理 - 当前正在显示的所有UI面板
    public Dictionary<UIPanelID, UIBasePanel> showedUIPanelDict = new Dictionary<UIPanelID, UIBasePanel>();

    //为了实现主UIPanel能始终显示在最前面,需要进行设计Canvas UI对象结构
    [HideInInspector]
    public Transform transUIRoot;                 //UI根节点---- 当前Canvas
    private Transform transUIRootForKeepAbove;   //保持在最上面UI的根节点
    private Transform transUIRootForNormal;      //普通UI的根节点

    //在Mono类下, 实现类似构造函数,初始化的逻辑
    protected override void Awake()
    {
        base.Awake();

        transUIRoot = this.transform;

        OnInit();
    }

    void OnInit()
    {
        // DontDestroyOnLoad(this.gameObject);//场景切换时,不销毁脚本所在的对象

        if (allUIPanelDict != null) allUIPanelDict.Clear();
        if (showedUIPanelDict != null) showedUIPanelDict.Clear();

        //Canvas下保持显示始终在前 的根节点
        if (transUIRootForKeepAbove == null)
        {
            transUIRootForKeepAbove = new GameObject("UIRootForKeepAbove").transform;
            UIUtils.addChildToParent(transUIRoot, transUIRootForKeepAbove);//把此对象放在父对象下面
        }

        //Canvas下普通UI元素的 根节点
        if (transUIRootForNormal == null)
        {
            transUIRootForNormal = new GameObject("UIRootForNormal").transform;
            UIUtils.addChildToParent(transUIRoot, transUIRootForNormal);
        }

        //  Debug.Log("显示主界面");

        //默认显示
        ShowUIPanel(UIPanelID.ID_MainPanel);//显示主界面UI
    }

    public void ShowUIPanel(UIPanelID id)
    {
        //1：判断是否是已显示的UI面板
        if (showedUIPanelDict.ContainsKey(id))//当前UI已经在显示列表中了，就直接返回
        {
            return;
        }

        //2：判断此面板是否已创建,没有则加载 预置体，创建面板，显示+管理起来

        //通过ID获取需要显示的UI，从 AllUIdict 容器中获取(打开过的面板,会在 AllUIdict 引用)
        UIBasePanel tmpUIPanel = GetUIPanelFromAllDict(id);

        if (tmpUIPanel == null)//如果在容器中没有此UI，就从资源中读取ui预制体,并创建
        {
            string prefabPath = UIPrefabPath.getUIPrefabPath(id);//通过ID，获取对应的路径

            if (!string.IsNullOrEmpty(prefabPath))
            {
                GameObject prefab = Resources.Load<GameObject>(prefabPath);//加载资源

                if (prefab != null)//资源加载成功
                {
                    GameObject goWillShowUIPanelObj = GameObject.Instantiate(prefab);//克隆游戏对象到层次面板上

                    tmpUIPanel = goWillShowUIPanelObj.GetComponent<UIBasePanel>();//获取此对象上的UI

                    prefab = null;
                }
                else
                {
                    Debug.LogError("资源" + prefabPath + "不存在");
                }
            }         
        }

        //3:更新显示其它的UI
        UpdateOtherUIPanelState(tmpUIPanel);

        // 将创建的新的UIPanel添加到字典中(如果有，就等于无此操作)
        allUIPanelDict[id] = tmpUIPanel;
        showedUIPanelDict[id] = tmpUIPanel;

        //  改变当前的显示层级，放在外面更加灵活，可能会修改面板的显示层级
        if (tmpUIPanel != null)
        {
            Transform root = GetRootNodeOfPanel(tmpUIPanel);//获取UI所对应的根节点

            //放入根节点下面
            UIUtils.addChildToParent(root, tmpUIPanel.transform);//放入根节点下面
        }

        //4:显示当前UI
        tmpUIPanel.Show();

        curDisplayUIPanel = tmpUIPanel;
    }

    public void HideUIPanel(UIPanelID id)
    {
        var tmpUIPanel = GetUIPanelFromShowedDict(id);

        if (tmpUIPanel != null)
        {
            showedUIPanelDict.Remove(id);
            tmpUIPanel.Hide();
        }
        else
        {
            Debug.LogError(id + "没有显示，无法隐藏");
            return;
        }
    }

    private void UpdateOtherUIPanelState(UIBasePanel tmpUIPanel)//更新其它UI的状态（显示或者隐藏）
    {
        if (tmpUIPanel == null)
        {
            return;
        }

        if (tmpUIPanel.showMode == UIShowMode.M_HideAll)
        {
            foreach (KeyValuePair<UIPanelID, UIBasePanel> item in showedUIPanelDict)//遍历所有正在显示的UI
            {
                item.Value.Hide();
            }

            showedUIPanelDict.Clear();
        }
        else if (tmpUIPanel.showMode == UIShowMode.M_HideAllExceptAbove)//剔除最前面UI
        {
            foreach (KeyValuePair<UIPanelID, UIBasePanel> item in showedUIPanelDict)
            {
                if (item.Value.IsAlwaysAbove)
                {
                    continue;
                }

                item.Value.Hide();
                showedUIPanelDict.Remove(item.Key);
                //  remove 后会使迭代器失效
                break;
            }        
        }
    }

    private UIBasePanel GetUIPanelFromAllDict(UIPanelID id)
    {
        if (allUIPanelDict.ContainsKey(id))
        {
            return allUIPanelDict[id];
        }
        else
        {
            return null;
        }

    }

    private UIBasePanel GetUIPanelFromShowedDict(UIPanelID id)
    {
        if (showedUIPanelDict.ContainsKey(id))
        {
            return showedUIPanelDict[id];
        }
        else
        {
            return null;
        }

    }

    //获取此UIPanel所在的根节点
    private Transform GetRootNodeOfPanel(UIBasePanel UIPanel)
    {
        if (UIPanel.IsAlwaysAbove)
        {
            return transUIRootForKeepAbove;
        }
        else
        {
            return transUIRootForNormal;
        }
    }
}

