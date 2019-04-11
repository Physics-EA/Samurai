using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBlock : ActionBase 
{
    public Agent Attacker;
    public E_WeaponType FromWeapon;
    public float Time;

    public ActionBlock() : base(ActionFactory.E_Type.E_BLOCK) { }
}
