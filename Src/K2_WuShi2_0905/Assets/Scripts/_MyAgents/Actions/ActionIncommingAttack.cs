using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIncommingAttack : ActionBase 
{
	public Agent Attacker;
	public float HitTime;

    public ActionIncommingAttack() : base(ActionFactory.E_Type.E_INCOMMING_ATTACK) { }
}
