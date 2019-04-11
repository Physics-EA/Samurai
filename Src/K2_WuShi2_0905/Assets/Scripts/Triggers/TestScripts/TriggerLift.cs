using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLift : MonoBehaviour 
{
    public Animation liftAnim;
    public ParticleEmitter emit;
    public AudioSource leverClip;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Debug.Log("播放电梯上升动画");
            CameraBehaviour.Instance.PlayCameraAnim("acamlift", false, false);
            liftAnim.Play();

            playerController.Instance.transform.position = transform.position;
            playerController.Instance.status = playerController.Player_Status.PS_UseLever;
            playerController.Instance.GetComponent<Animation>().CrossFade("useLever");

            leverClip.Play(44100);
            emit.emit = false;
        }

        //  当动画播放完成后，将人物状态切换为Idle
        if (!liftAnim.isPlaying)
        {
            playerController.Instance.status = playerController.Player_Status.PS_Idle;
        }
    }
}
