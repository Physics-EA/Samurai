using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour 
{
    private float Damage;
    private bool Hit;
    private float Timer;
    private Vector3 Direction;
    private float Speed;

    public void Init(Agent owner,Vector3 pos,Vector3 dir,float speed,float damage)
    {
        transform.position = pos;

        transform.rotation = owner.transform.rotation;

        Damage = damage;
        Direction = dir;
        Speed = speed;

        Hit = false;
        Timer = 0;
    }

    void Update()
    {
         transform.Translate(Direction * Speed * Time.deltaTime,Space.World);
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ActionInjury injury = ActionFactory.Create(ActionFactory.E_Type.E_INJURY) as ActionInjury;
            injury.Impuls = (Direction).normalized;

            Player.Instance.GetComponent<AnimComponent>().HandleAction(injury);
            Player.Instance.comboHitNum = 0;
            Player.Instance.Owner.BlackBoard.Health -= 10;
            MainPanelCtrl.Instance.ShowComboMessage(Player.Instance.comboHitNum);

            Destroy(this.gameObject,0.2f);
        }
        else
        {
            Destroy(this.gameObject,2);
        }

    }
}
