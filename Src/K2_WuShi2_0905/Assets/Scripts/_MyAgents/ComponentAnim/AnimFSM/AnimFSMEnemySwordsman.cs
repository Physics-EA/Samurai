using UnityEngine;
using System.Collections;

// add enum
// add new  - in Initialize
// add if  - in DoAction

public class AnimFSMEnemySwordsman: AnimFSM
{
	enum E_MyAnimState
	{
		E_IDLE,
		E_GOTO,
        E_COMBAT_MOVE,
        Rotate,
		E_ATTACK_MELEE,
        E_PLAY_ANIM,
        E_INJURY,
        Knockdown,
        E_DEATH,
        E_BLOCK,
	}

    public AnimFSMEnemySwordsman(Animation anims, Agent owner) : base(anims, owner)
    {
		AnimStates.Add(new AnimStateIdle(AnimEngine, Owner)); //E_MyAnimState.E_IDLE
		AnimStates.Add(new AnimStateGoTo(AnimEngine, Owner)); //E_MyAnimState.E_GOTO
        AnimStates.Add(new AnimStateCombatMove(AnimEngine, Owner)); //E_MyAnimState.E_GOTO
        AnimStates.Add(new AnimStateRotate(AnimEngine, Owner)); //E_MyAnimState.Rotate
		AnimStates.Add(new AnimStateAttackMelee(AnimEngine, Owner)); //E_MyAnimState.E_ATTACK_MELEE
        AnimStates.Add(new AnimStatePlayAnim(AnimEngine, Owner)); //E_MyAnimState.E_PLAY_ANIM
        AnimStates.Add(new AnimStateInjury(AnimEngine, Owner)); //E_MyAnimState.E_INJURY
        AnimStates.Add(new AnimStateKnockdown(AnimEngine, Owner)); //E_MyAnimState.Knockdown
        AnimStates.Add(new AnimStateDeath(AnimEngine, Owner)); //E_MyAnimState._E_DEATH
        AnimStates.Add(new AnimStateBlocking(AnimEngine, Owner)); //E_MyAnimState.E_BLOCK
    }

	public override void Initialize()
	{
		DefaultAnimState = AnimStates[(int)E_MyAnimState.E_IDLE];
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
                NextAnimState = AnimStates[(int)E_MyAnimState.E_GOTO];
            if (action is ActionCombatMove)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_COMBAT_MOVE];
            else if (action is ActionRotate)
                NextAnimState = AnimStates[(int)E_MyAnimState.Rotate];
            else if (action is ActionAttack)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_ATTACK_MELEE];
            else if (action is ActionWeaponShow)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_IDLE];
            else if (action is ActionPlayAnim || action is ActionPlayIdleAnim)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_PLAY_ANIM];
            else if (action is ActionInjury)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_INJURY];
            else if (action is ActionKnockdown)
                NextAnimState = AnimStates[(int)E_MyAnimState.Knockdown];
            else if (action is ActionDeath)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_DEATH];
            else if (action is ActionBlock)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_BLOCK];

            ProgressToNextStage(action);

		}
	}
}