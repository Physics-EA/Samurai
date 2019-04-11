using UnityEngine;

public class AnimStateInjury : AnimState
{
    Quaternion FinalRotation;
    Quaternion StartRotation;

    float RotationProgress;
    float MoveTime;
    float CurrentMoveTime;
    bool PositionOK = false;
    bool RotationOk = false;

    Vector3 Impuls;
    ActionInjury Action = null;

    float EndOfStateTime;

    public AnimStateInjury(Animation anims, Agent owner): base(anims, owner)
	{
	}


	override public void OnActivate(ActionBase action)
	{
        base.OnActivate(action);

        Owner.BlackBoard.MotionType = E_MotionType.None;
        Owner.BlackBoard.MoveDir = Vector3.zero;
        Owner.BlackBoard.Speed = 0;
	}

	override public void OnDeactivate()
	{
      //  Time.timeScale = 1;

		Action.SetSuccess();
		Action = null;

        Owner.BlackBoard.MotionType = E_MotionType.None;

        base.OnDeactivate();
	}

    override public void Update()
    {
 //       //Debug.DrawLine(OwnerTransform.position + new Vector3(0, 1, 0), OwnerTransform.position + Action.Direction + new Vector3(0, 1, 0));
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

            
            float progress = Mathf.Max(0, Mathf.Min(1.0f, CurrentMoveTime / MoveTime));

            Vector3 impuls =  Vector3.Lerp(Impuls, Vector3.zero, progress);

            if (MoveEx(impuls * Time.deltaTime) == false)
            {
                //Debug.Log("move false");
                PositionOK = true;
            }
        }

        if (EndOfStateTime <= Time.timeSinceLevelLoad)
            Release();
    }

	override public bool HandleNewAction(ActionBase action)
	{
        if (action is ActionInjury)
		{
            if (Action != null)
                Action.SetSuccess();

            SetFinished(false); // just for sure

            Initialize(action);

			return true;
		}
    	return false;
	}

    protected override void Initialize(ActionBase action)
    {
        base.Initialize(action);

        Action = action as ActionInjury;

        // play owner anims
        string animName = Owner.AnimSet.GetInjuryAnim(Action.FromWeapon, Action.DamageType);

        CrossFade(animName, 0.1f);
        //end of state
        EndOfStateTime = AnimEngine[animName].length + Time.timeSinceLevelLoad;

       // Debug.Log(Action.AnimName + " " + EndOfStateTime );
        Owner.BlackBoard.MotionType = E_MotionType.None;

        MoveTime = AnimEngine[animName].length * 0.5f;
        CurrentMoveTime = 0;

        if (Action.Attacker != null && Owner.IsPlayer == false)
        {
            Vector3 dir = Action.Attacker.Position - Transform.position;
            dir.Normalize();
            FinalRotation.SetLookRotation(dir);
            StartRotation = Transform.rotation;
            RotationProgress = 0;
            RotationOk = false;
        }
        else
        {
            RotationOk = true;
        }

        Impuls = Action.Impuls * 10;

        PositionOK = Impuls == Vector3.zero;

       // Debug.Log("recieve injury : " + animName + " end time " + EndOfStateTime + " start time "  + Time.timeSinceLevelLoad);

        Owner.BlackBoard.MotionType = E_MotionType.Injury;
    }
}
