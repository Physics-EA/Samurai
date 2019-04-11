using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBedna : MonoBehaviour 
{
    public Animation bedna;
    public ParticleEmitter splouch;
    public ParticleEmitter kolo;
    public ParticleEmitter hvezdicky;
    public ParticleEmitter kruhy;

    public AudioSource splouchClip;

    public void OnTriggerEnter(Collider other)
    {
        //  如何玩家碰到该木块就触发对应的事件
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("将箱子推落到水中");
           /* 1. 播放箱子掉落的动画，同时配上对应的粒子特效
            * 2. 采用协程方法来进行播放粒子特效
           */
            //  将当前控制对象禁用掉
            this.GetComponent<BoxCollider>().enabled = false;

            bedna.Play();
            StartCoroutine(PlayEffect());
        }
    }

    IEnumerator PlayEffect()
    {
        hvezdicky.emit = false;
        yield return new WaitForSeconds(0.2f);
        kolo.emit = true;
        kruhy.emit = true;
        yield return new WaitForSeconds(0.1f);
        splouch.emit = true;
        splouchClip.Play();
        yield return new WaitForSeconds(0.5f);
        kolo.emit = false;
        splouch.emit = false;
        yield return new WaitForSeconds(10.5f);    
        kruhy.emit = false;
      
    }
}
