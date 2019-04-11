using UnityEngine;
using System.Collections;

// add enum
// add new  - in Initialize
// add if  - in DoAction

public class AnimFSMPlayer: AnimFSM
{
	enum E_MyAnimState
	{
		E_IDLE,
		E_GOTO,
        E_MOVE,
		E_ATTACK_MELEE,
        E_ROLL,
        E_USE_LEVER,
        E_Teleport,
        E_INJURY,
        E_DEATH,
	}

	public AnimFSMPlayer(Animation anims, Agent owner) : base(anims, owner) { }

	public override void Initialize()
	{

		AnimStates.Add(new AnimStateIdle(AnimEngine, Owner)); //E_MyAnimState.E_IDLE
		AnimStates.Add(new AnimStateGoTo(AnimEngine, Owner)); //E_MyAnimState.E_GOTO
        AnimStates.Add(new AnimStateMove(AnimEngine, Owner)); //E_MyAnimState.E_MOVE
        AnimStates.Add(new AnimStateAttackMelee(AnimEngine, Owner)); //E_MyAnimState.E_ATTACK_MELEE
        AnimStates.Add(new AnimStateRoll(AnimEngine, Owner)); //E_MyAnimState.E_ROLL
        AnimStates.Add(new AnimStateUseLever(AnimEngine, Owner)); //use lever
        AnimStates.Add(new AnimStateTeleport(AnimEngine, Owner)); // teleport
        AnimStates.Add(new AnimStateInjury(AnimEngine, Owner)); //E_MyAnimState.E_INJURY
        AnimStates.Add(new AnimStateDeath(AnimEngine, Owner)); //E_MyAnimState._EDEATHM

		DefaultAnimState = AnimStates[(int)E_MyAnimState.E_IDLE];

       // DefaultAnimState = AnimStates[(int)E_MyAnimState.E_MOVE];
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
            if (action is ActionMove)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_MOVE];
            else if (action is ActionAttack)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_ATTACK_MELEE];
            else if (action is ActionRoll)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_ROLL];
            else if (action is ActionWeaponShow)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_IDLE];
            else if (action is ActionUseLever)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_USE_LEVER];
            else if (action is ActionTeleport)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_Teleport];
            else if (action is ActionInjury)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_INJURY];
            else if (action is ActionDeath)
                NextAnimState = AnimStates[(int)E_MyAnimState.E_DEATH];

            if (NextAnimState != null)
            {
                ProgressToNextStage(action);
            }
                
		}
	}
}