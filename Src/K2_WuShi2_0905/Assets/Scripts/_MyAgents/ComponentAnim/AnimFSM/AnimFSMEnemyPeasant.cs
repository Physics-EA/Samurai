using UnityEngine;
using System.Collections;

// add enum
// add new  - in Initialize
// add if  - in DoAction

public class AnimFSMEnemyPeasant: AnimFSM
{
	enum E_MyAnimState
	{
		idle,
		gotoPos,
        combatMove,
        Rotate,
		attackMelee,
        playAnim,
        injury,
        knockdown,
        death,
	}

    public AnimFSMEnemyPeasant(Animation anims, Agent owner) : base(anims, owner) 
    {
        AnimStates.Add(new AnimStateIdle(AnimEngine, Owner)); //E_MyAnimState.E_IDLE
        AnimStates.Add(new AnimStateGoTo(AnimEngine, Owner)); //E_MyAnimState.E_GOTO
        AnimStates.Add(new AnimStateCombatMove(AnimEngine, Owner)); //E_MyAnimState.combatMove
        AnimStates.Add(new AnimStateRotate(AnimEngine, Owner)); //E_MyAnimState.Rotate
        AnimStates.Add(new AnimStateAttackMelee(AnimEngine, Owner)); //E_MyAnimState.E_ATTACK_MELEE
        AnimStates.Add(new AnimStatePlayAnim(AnimEngine, Owner)); //E_MyAnimState.E_PLAY_ANIM
        AnimStates.Add(new AnimStateInjury(AnimEngine, Owner)); //E_MyAnimState.E_INJURY
        AnimStates.Add(new AnimStateKnockdown(AnimEngine, Owner)); //E_MyAnimState.Knockdown
        AnimStates.Add(new AnimStateDeath(AnimEngine, Owner)); //E_MyAnimState._E_DEATH
    }

	public override void Initialize()
	{
		DefaultAnimState = AnimStates[(int)E_MyAnimState.idle];
		base.Initialize();
	}

	public override void DoAction(ActionBase action)
	{
		if (CurrentAnimState.HandleNewAction(action) == true)
		{
			//Debug.Log("AC - Do Action " + action.ToString());
			NextAnimState = null;
		}
		else
		{
            if (action is ActionGoTo)
                NextAnimState = AnimStates[(int)E_MyAnimState.gotoPos];
            if (action is ActionCombatMove)
                NextAnimState = AnimStates[(int)E_MyAnimState.combatMove];
            else if (action is ActionRotate)
                NextAnimState = AnimStates[(int)E_MyAnimState.Rotate];
            else if (action is ActionAttack)
                NextAnimState = AnimStates[(int)E_MyAnimState.attackMelee];
            else if (action is ActionWeaponShow)
                NextAnimState = AnimStates[(int)E_MyAnimState.idle];
            else if (action is ActionPlayAnim || action is ActionPlayIdleAnim)
                NextAnimState = AnimStates[(int)E_MyAnimState.playAnim];
            else if (action is ActionInjury)
                NextAnimState = AnimStates[(int)E_MyAnimState.injury];
            else if (action is ActionKnockdown)
                NextAnimState = AnimStates[(int)E_MyAnimState.knockdown];
            else if (action is ActionDeath)
                NextAnimState = AnimStates[(int)E_MyAnimState.death];

            if (NextAnimState != null)
            {
                ProgressToNextStage(action);
            }        
		}
	}
}