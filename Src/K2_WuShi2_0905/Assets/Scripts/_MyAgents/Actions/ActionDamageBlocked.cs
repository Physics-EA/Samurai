using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDamageBlocked : ActionBase 
{
    public Agent Attacker;
    public E_WeaponType AttackerWeapon;
    public bool BreakBlock;

    public ActionDamageBlocked() : base(ActionFactory.E_Type.E_DAMAGE_BLOCKED) { }

    public override void Reset() 
    {
        BreakBlock = false;
    }
}
