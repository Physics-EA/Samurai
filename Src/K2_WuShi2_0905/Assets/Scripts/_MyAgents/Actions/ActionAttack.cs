using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAttack : ActionBase 
{
    public Agent Target;  //  攻击目标
    public AnimAttackData Data;  //   攻击动画数据，暂无
    public E_AttackType AttackType;
    public Vector3 AttackDir;

	public bool Hit;
    public bool AttackPhaseDone;

    public ActionAttack() : base(ActionFactory.E_Type.E_ATTACK) { }

    public override void Reset()
    {
        base.Reset();
        Target = null;
        Hit = false;
        AttackPhaseDone = false;
        Data = null;
        AttackType = E_AttackType.None;
    }
    public override string ToString() { return "HumanActionAttack " + (Target != null ? Target.name : "no target") + " " + AttackType.ToString() + " " + Status.ToString(); }
}
