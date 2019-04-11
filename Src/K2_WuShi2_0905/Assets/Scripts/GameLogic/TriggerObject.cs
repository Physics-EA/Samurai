using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TriggerObject : MonoBehaviour 
{
    //  播放过场动画，要同时结合音效，动画还有粒子特效一起，通过协程的方式可以处理的比较好
    [System.Serializable]
    public class InteractionParticle
    {
        public ParticleEmitter Emitter;
        public float Delay;
        public float Life;
        //  public bool LinkOnRoot;
    }

   [System.Serializable]
    public class InteractionSound
    {
        public AudioSource Music;
        public float Delay;
        public float Life;
        //  public Transform Parent;
    }

    public InteractionParticle[] Emitters;
    public InteractionSound[] Sounds;
    public ParticleEmitter ActiveEffect;

    public GameObject InteractionObject;
    public AnimationClip CameraAnimation;
    public AnimationClip ObjectAnim;

    public Transform entryTransform;
    public Transform root;
    public bool DisableAfterUse = true;
    public bool ChangePlayerPos = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //  机关一触发，立刻将触发器去激活，只能执行一次
            GetComponent<SphereCollider>().enabled = false;
            OnInteractionStart();

            if (Player.Instance.transform.parent != null)
            {
                Debug.Log(Player.Instance.transform.parent.name); 
            }    

            if (ChangePlayerPos && root != null)
            {
                Player.Instance.transform.SetParent(root);
                Player.Instance.transform.localPosition = Vector3.zero;
                Invoke("CancelParent", ObjectAnim.length);
            }
        }
    }

    private void CancelParent()
    {
        Player.Instance.transform.parent = null;
        Player.Instance.GetComponent<CharacterController>().Move(Vector3.down * 3);
    }
    public void OnInteractionStart()
    {
        //  关闭触发的粒子特效
        if (ActiveEffect != null)
        {
            ActiveEffect.emit = false;
        }
        //  播放摄像机视角动画
        if (CameraAnimation != null)
        {
            CameraBehaviour.Instance.PlayCameraAnim(CameraAnimation.name, true, true);        
        }
        //  播放物体的动画
        if (InteractionObject != null)
        {
            InteractionObject.GetComponent<Animation>().Play(ObjectAnim.name);           
        }

        //  播放粒子特效
        for (int i = 0; Emitters!=null && i < Emitters.Length; i++)
        {
            StartCoroutine(ParticleRun(Emitters[i].Emitter, Emitters[i].Delay));
            StartCoroutine(ParticleStop(Emitters[i].Emitter,Emitters[i].Delay+Emitters[i].Life));
        }
        //  播放音效
        for (int i = 0; Sounds != null && i < Sounds.Length; i++)
        {
            StartCoroutine(SoundRun(Sounds[i].Music, Sounds[i].Delay));
            StartCoroutine(SoundStop(Sounds[i].Music, Sounds[i].Delay + Sounds[i].Life));           
        }    
    }

    public void OnInteractionEnd()
    {
        if (DisableAfterUse)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator ParticleRun(ParticleEmitter emitter, float delay)
    {
        yield return new WaitForSeconds(delay);
        emitter.emit = true;

        //Debug.Log(Time.timeSinceLevelLoad + " ParticleRun"); 
    }

    private IEnumerator ParticleStop(ParticleEmitter emitter, float delay)
    {
        yield return new WaitForSeconds(delay);
        emitter.emit = false;

        // Debug.Log(Time.timeSinceLevelLoad + " ParticleStop");
    }

    private IEnumerator SoundRun(AudioSource audio, float delay)
    {
        yield return new WaitForSeconds(delay);
        audio.Play();
        //Debug.Log(Time.timeSinceLevelLoad + " " + audio.name + " audio start");
    }

    private IEnumerator SoundStop(AudioSource audio, float delay)
    {
        yield return new WaitForSeconds(delay);
        audio.Stop();

        Player.Instance.useMode = true;
    }

    public Transform GetEntryTransform()
    {
        if (entryTransform == null)
        {
            return null;
        }

        return entryTransform;
    }
}