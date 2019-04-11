using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDeath : ActionBase 
{
    public Agent Attacker;
    public E_CriticalHitType DecapType;
    public E_WeaponType FromWeapon;
    public E_DamageType DamageType;
    public Vector3 Impuls;

    public ActionDeath() : base(ActionFactory.E_Type.E_DEATH) { } 
}
