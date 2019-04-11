using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRoll : ActionBase 
{
    public Vector3 Direction;
    public Agent ToTarget;

    public ActionRoll() : base(ActionFactory.E_Type.E_ROLL) { }

    public override void Reset() 
    {
        ToTarget = null;
    }
}
