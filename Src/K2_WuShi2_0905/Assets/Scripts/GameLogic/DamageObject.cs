using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour 
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //  玩家受到伤害
            ActionInjury injury = ActionFactory.Create(ActionFactory.E_Type.E_INJURY) as ActionInjury;
            injury.Impuls = (other.gameObject.transform.position - transform.position).normalized * 2;

            Player.Instance.GetComponent<AnimComponent>().HandleAction(injury);
            Player.Instance.comboHitNum = 0;
            Player.Instance.Owner.BlackBoard.Health -= 10;
            MainPanelCtrl.Instance.ShowComboMessage(Player.Instance.comboHitNum);

        }
    }
}
