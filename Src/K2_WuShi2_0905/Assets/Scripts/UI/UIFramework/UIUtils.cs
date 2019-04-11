using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts;

//UI框架中，常用的方法，汇总在UI工具类中，作为静态函数
class UIUtils
{
    public static void addChildToParent(Transform parent, Transform child)
    {
        child.SetParent(parent);
        //封装的原因
        child.localPosition = Vector3.zero;
        child.localScale = Vector3.one;
        child.localRotation = Quaternion.identity;
    }

    // 查找父物体下面的子物体
    //this.tansform.find("") //this是激活的.this下未激活的子节点
    //GameObject.find()      //不要放在UPDATE,只能查找激活东节点
    public static GameObject findChild(GameObject goParent, string childName)
    {
        Transform transChild = goParent.transform.Find(childName);
        if (transChild == null)//如果没有找到
        {
            foreach (Transform t in goParent.transform)//父物体下面的所有子物体
            {
                transChild = findChild(t.gameObject, childName).transform;
                if (transChild != null)
                {
                    return transChild.gameObject;
                }
            }
        }
        return transChild.gameObject;
    }

    public static void clearMemory()
    {
        GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    public static void OpenLoadSceneHelper()
    {
        GameObject uiRoot = GameObject.FindGameObjectWithTag("UIRoot");
        if (uiRoot != null)
        {
            GameObject loadSceneHelper = uiRoot.transform.Find("LoadSceneHelper").gameObject;

            if (loadSceneHelper.activeInHierarchy == false)
            {
                loadSceneHelper.SetActive(true);
            }
        }
    }
}
