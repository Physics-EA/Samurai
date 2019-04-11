using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGoTo : ActionBase 
{
    public Vector3 FinalPosition;
    public E_MoveType MoveType;
    public E_MotionType Motion;

    public ActionGoTo() : base(ActionFactory.E_Type.E_GOTO) { }
}
