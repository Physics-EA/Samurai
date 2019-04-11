using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAttackWhirl : ActionBase 
{
    public AnimAttackData Data;
    public ActionAttackWhirl() : base(ActionFactory.E_Type.E_ATTACK_WHIRL) { }

    public override void Reset()
    {
        base.Reset();
    }
}
