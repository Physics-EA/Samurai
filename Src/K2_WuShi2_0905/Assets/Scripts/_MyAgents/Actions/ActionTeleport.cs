using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTeleport : ActionBase 
{
	public bool ShowParticle = false;
    public bool FadeGui = false;
    public Teleport Teleport;

    public ActionTeleport() : base(ActionFactory.E_Type.E_Teleport) { }

    public override void Reset()
    {
        ShowParticle = false;
        FadeGui = false;
    }
}
