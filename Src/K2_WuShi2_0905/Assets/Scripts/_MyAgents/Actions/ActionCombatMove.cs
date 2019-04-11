using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCombatMove : ActionBase 
{
    public Agent Target;  //  目标敌人
    public float DistanceToMove;
    public float MinDistanceToTarget;

    public E_MoveType MoveType;
    public E_MotionType MotionType = E_MotionType.Walk;

    public ActionCombatMove() : base(ActionFactory.E_Type.E_COMBAT_MOVE) { }

    public override void Reset()
    {
        base.Reset();
        DistanceToMove = 0;
        MinDistanceToTarget = 0;
        MoveType = E_MoveType.Forward;
        MotionType = E_MotionType.Walk;
    }
}
