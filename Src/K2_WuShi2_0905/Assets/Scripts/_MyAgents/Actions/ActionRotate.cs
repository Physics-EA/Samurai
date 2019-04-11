using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRotate : ActionBase 
{
    public Vector3 Direction = Vector3.zero;
    public Agent Target = null;
    public float RotationModifier = 1;

    public ActionRotate() : base(ActionFactory.E_Type.E_Rotate) { }

    public override void Reset() 
    { 
        Target = null; 
        Direction = Vector3.zero; 
        RotationModifier = 1; 
    }
}
