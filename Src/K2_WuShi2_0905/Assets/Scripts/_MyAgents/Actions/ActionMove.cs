using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMove : ActionBase
{
    public E_MoveType MoveType = E_MoveType.Forward;
    public ActionMove() : base(ActionFactory.E_Type.E_MOVE) { }
}
