using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAttackRoll : ActionBase 
{
    public AnimAttackData Data;
    public Agent Target;

    public ActionAttackRoll() : base(ActionFactory.E_Type.E_ATTACK_ROLL) { }
}
