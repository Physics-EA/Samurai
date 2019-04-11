using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlayAnim : ActionBase 
{
	public string AnimName;
    public ActionPlayAnim() : base(ActionFactory.E_Type.E_PLAY_ANIM) { }
}
