using UnityEngine;
using System.Collections;

public class AnimStateTeleport : AnimState
{
    ActionTeleport Action;

    public AnimStateTeleport(Animation anims, Agent owner) : base(anims, owner)
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
        if (Action != null)
            Action.SetSuccess();

        Action = null;

        base.OnDeactivate();
    }


    override public bool HandleNewAction(ActionBase action)
    {
        return false;
    }

    override public void Update()   
    {
    }

    protected override void Initialize(ActionBase action)
    {
        base.Initialize(action);

        Action = action as ActionTeleport;

        Owner.BlackBoard.MotionType = E_MotionType.None;
        Owner.BlackBoard.MoveDir = Vector3.zero;
        Owner.BlackBoard.Speed = 0;

        string s = Owner.AnimSet.GetIdleAnim(Owner.BlackBoard.WeaponSelected, Owner.BlackBoard.WeaponState);
        CrossFade(s, 0.2f);

        Owner.StartCoroutine(Teleport());
    }

    IEnumerator Teleport()
    {
        yield return new WaitForSeconds(Action.Teleport.TeleportDelay);

        Vector3 offset = Vector3.zero;
        if (Owner.IsPlayer)
             offset = Owner.Transform.InverseTransformPoint(Camera.main.transform.position); 

        if (Action.Teleport.Sound)
            Owner.SoundPlay(Action.Teleport.Sound);

        //  播放人物灵魂的淡出效果
        if(Action.FadeGui)
            //GuiManager.Instance.FadeOut(Action.Teleport.FadeOUtTime);

        yield return new WaitForSeconds(Action.Teleport.FadeOUtTime + 0.1f);

        Owner.Teleport(Action.Teleport.Destination);

        if (Owner.IsPlayer)
        {
            CameraBehaviour.Instance.Activate(Owner.Transform.position + offset, Owner.Transform.position);
        }

        //  播放人物灵魂的淡入效果
        if (Action.FadeGui)
            //GuiManager.Instance.FadeIn(Action.Teleport.FadeInTime);

        Release();
    }
}