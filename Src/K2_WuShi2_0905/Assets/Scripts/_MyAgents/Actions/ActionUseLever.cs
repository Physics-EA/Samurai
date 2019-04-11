using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionUseLever : ActionBase 
{
	public TriggerObject InterObj;              //  作用对象
    public E_InteractionType Interaction;       //  作用类型

    public ActionUseLever() : base(ActionFactory.E_Type.E_USE_LEVER) { }
}
