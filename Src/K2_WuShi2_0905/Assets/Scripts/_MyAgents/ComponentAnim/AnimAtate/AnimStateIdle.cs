using UnityEngine;

public class AnimStateIdle : AnimState
{
    float TimeToFinishWeaponAction;
    ActionBase WeaponAction;

    public AnimStateIdle(Animation anims, Agent owner): base(anims, owner)
    {
    }

    override public void Release()
    {
        //if (m_Human.PlayerProperty != null)
        //Debug.Log(Time.timeSinceLevelLoad + " " + this.ToString() + " - release");

        SetFinished(true);
    }

    override public void OnActivate(ActionBase action)
    {
        base.OnActivate(action);

    }

    override public void OnDeactivate()
    {

        base.OnDeactivate();
    }

    override public bool HandleNewAction(ActionBase action)
    {
        //if (m_Human.PlayerProperty != null)
        //Debug.Log(Time.timeSinceLevelLoad + " " + this.ToString() + " - action " + action.ToString());

        if (action is ActionWeaponShow)
        {
            if ((action as ActionWeaponShow).Show == true)
            {
                //swhow weapon anim
                string s = Owner.AnimSet.GetShowWeaponAnim(Owner.BlackBoard.WeaponSelected);
                TimeToFinishWeaponAction = Time.timeSinceLevelLoad + AnimEngine[s].length * 0.8f;
                CrossFade(s, 0.1f);
//                Owner.ShowWeapon(true, 0.1f);
            }
            else
            {
                //hide weapon anim
                string s = Owner.AnimSet.GetHideWeaponAnim(Owner.BlackBoard.WeaponSelected);
                TimeToFinishWeaponAction = Time.timeSinceLevelLoad +( AnimEngine[s].length * 0.9f);
                CrossFade(s, 0.1f);
  //              Owner.ShowWeapon(false, 2.3f);
            }

            WeaponAction = action;
            return true;
        }
        return false;
    }

    override public void Update()
    {
        if (WeaponAction != null && TimeToFinishWeaponAction < Time.timeSinceLevelLoad)
        {
            WeaponAction.SetSuccess();
            WeaponAction = null;
            //Debug.Log(Owner.AnimSet.GetIdleAnim(Owner.BlackBoard.WeaponSelected, Owner.BlackBoard.WeaponState).ToString());
            //PlayIdleAnim();
            CrossFade(Owner.AnimSet.GetIdleAnim(Owner.BlackBoard.WeaponSelected, Owner.BlackBoard.WeaponState), 0.2f);
        }
    }

    void PlayIdleAnim()
    {
        //Debug.Log(Owner.AnimSet.GetIdleAnim(Owner.BlackBoard.WeaponSelected, Owner.BlackBoard.WeaponState).ToString());
        string s = Owner.AnimSet.GetIdleAnim(Owner.BlackBoard.WeaponSelected, Owner.BlackBoard.WeaponState);
        CrossFade(s, 0.2f);
    }

    protected override void Initialize(ActionBase action)
    {
        base.Initialize(action);

        Owner.BlackBoard.MotionType = E_MotionType.None;
        Owner.BlackBoard.MoveDir = Vector3.zero;
        Owner.BlackBoard.Speed = 0;

        if (WeaponAction == null)
            PlayIdleAnim();
    }

}