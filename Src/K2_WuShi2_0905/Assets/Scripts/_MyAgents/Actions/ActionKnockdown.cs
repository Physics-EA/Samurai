using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionKnockdown : ActionBase 
{
    public Agent Attacker;
    public E_WeaponType FromWeapon;
    public Vector3 Impuls; 
    public float Time;

    public ActionKnockdown() : base(ActionFactory.E_Type.E_KNOCKDOWN) { }
}
