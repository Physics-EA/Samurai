using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInjury : ActionBase 
{
    public Agent Attacker; //  攻击者
    public E_WeaponType FromWeapon;
    public E_DamageType DamageType;
    public Vector3 Impuls;

    public ActionInjury() : base(ActionFactory.E_Type.E_INJURY) { }
}
