using UnityEngine;

public class AnimStateUseLever : AnimState
{
    enum E_State
    {
        E_PREPARING_FOR_USE,
        E_USING,
    }

    ActionUseLever Action = null;
    TriggerObject InterObj; 

    Quaternion FinalRotation;
	Quaternion StartRotation;
    Vector3 StartPosition;
    Vector3 FinalPosition;
	float RotationProgress;
    float MoveTime;
    float CurrentMoveTime;
    float EndOfStateTime;

    bool RotationOk = false;
    bool PositionOK = false;

    E_State State;


	public AnimStateUseLever(Animation anims, Agent owner): base(anims, owner)
	{
	}


	override public void OnActivate(ActionBase action)
	{
        base.OnActivate(action);

        Owner.BlackBoard.MotionType = E_MotionType.None;
        Owner.BlackBoard.MoveDir = Vector3.zero;
        Owner.BlackBoard.Speed = 0;
     
       //Time.timeScale = .1f;
	}

	override public void OnDeactivate()
	{
        //Time.timeScale = 1;

		Action.SetSuccess();
		Action = null;
        base.OnDeactivate();
	}

    override public void Update()
    {
        //Debug.DrawLine(OwnerTransform.position + new Vector3(0, 1, 0), OwnerTransform.position + Action.Direction + new Vector3(0, 1, 0));

        //Debug.Log("Update");

        if (State == E_State.E_PREPARING_FOR_USE)
        {
            if (RotationOk == false)
            {
                //Debug.Log("rotate");
                RotationProgress += Time.deltaTime * Owner.BlackBoard.RotationSmooth;

                if (RotationProgress >= 1)
                {
                    RotationProgress = 1;
                    RotationOk = true;
                }

                RotationProgress = Mathf.Min(RotationProgress, 1);
                Quaternion q = Quaternion.Lerp(StartRotation, FinalRotation, RotationProgress);
                Owner.Transform.rotation = q;
            }

            if (PositionOK == false)
            {
                CurrentMoveTime += Time.deltaTime;
                if (CurrentMoveTime >= MoveTime)
                {
                    CurrentMoveTime = MoveTime;
                    PositionOK = true;
                }

                float progress = Mathf.Min(1.0f, CurrentMoveTime / MoveTime);
                Vector3 finalPos = Mathfx.Sinerp(StartPosition, FinalPosition, progress);
                //MoveTo(finalPos);
                if (Move(finalPos - Transform.position) == false)
                    PositionOK = true;
            }
        }

        if (State == E_State.E_PREPARING_FOR_USE && RotationOk && PositionOK)
        {
            State = E_State.E_USING;
            PlayAnim();
        }

        if ( State == E_State.E_USING && EndOfStateTime <= Time.timeSinceLevelLoad)
            Release();
    }

	override public void Release()
	{
        Transform.parent = null;

        base.Release();
	}

	override public bool HandleNewAction(ActionBase action)
	{
        if (action is ActionUseLever)
		{
            if (Action != null)
                action.SetFailed();
		}
    	return false;
	}

	private void PlayAnim()
	{
        string animName = Owner.AnimSet.GetUseAnim(E_InteractionObjects.UseLever, Action.Interaction);
        
        float time = AnimEngine[animName].length;
        // play owner anims
        CrossFade(animName, 0.1f);

        //  物体的动画时长
        float time2 = Action.InterObj.ObjectAnim.length;
        //end of state
        EndOfStateTime = Time.timeSinceLevelLoad + Mathf.Max(time, time2);

        //Debug.Log(animName + " " + Mathf.Max(time, time2));
        Owner.BlackBoard.MotionType = E_MotionType.None;
	}

    protected override void Initialize(ActionBase action)
    {
        base.Initialize(action);

        Action = action as ActionUseLever;

        StartPosition = Owner.transform.position;
        StartRotation.SetLookRotation(Owner.transform.forward);

        FinalPosition = Action.InterObj.GetEntryTransform().position;
        FinalRotation.SetLookRotation(Action.InterObj.GetEntryTransform().forward);


        RotationProgress = 0;
        CurrentMoveTime = 0;
        MoveTime = 0.1f;

        RotationOk = false;
        PositionOK = false;
        State = E_State.E_PREPARING_FOR_USE;
    }
}
