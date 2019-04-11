using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionWeaponShow : ActionBase 
{
    public bool Show = true;

    public ActionWeaponShow() : base(ActionFactory.E_Type.E_WEAPON_SHOW) { }

}
