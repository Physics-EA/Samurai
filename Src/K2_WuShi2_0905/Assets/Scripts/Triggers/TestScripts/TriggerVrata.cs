using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerVrata : MonoBehaviour 
{
    public Animation vrataAnim;
    public ParticleEmitter emit;
    public AudioSource leverClip;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Debug.Log("播放阀门动画");
            CameraBehaviour.Instance.PlayCameraAnim("acamvrata1", false, false);
            vrataAnim.Play();

            playerController.Instance.transform.position = transform.position;
            playerController.Instance.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            playerController.Instance.status = playerController.Player_Status.PS_UseLever;
            playerController.Instance.GetComponent<Animation>().CrossFade("useLever");

            leverClip.Play(44100);
            emit.emit = false;
        }

        //  当动画播放完成后，将人物状态切换为Idle
        if (!vrataAnim.isPlaying)
        {
            playerController.Instance.status = playerController.Player_Status.PS_Idle;
        }
    }
}
