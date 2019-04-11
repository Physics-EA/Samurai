using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTocna : MonoBehaviour 
{
    public Animation tocnaAnim;
    public ParticleEmitter emit;
    public AudioSource leverClip;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log("播放移动门动画");
            CameraBehaviour.Instance.PlayCameraAnim("acamtocna", false, false);
            tocnaAnim.Play();

            playerController.Instance.transform.position = transform.position;
            playerController.Instance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            playerController.Instance.status = playerController.Player_Status.PS_UseLever;
            playerController.Instance.GetComponent<Animation>().CrossFade("useLever");

            leverClip.Play(44100); 
            emit.emit = false;
        }

        //  当动画播放完成后，将人物状态切换为Idle
        if (!tocnaAnim.isPlaying)
        {
            playerController.Instance.status = playerController.Player_Status.PS_Idle;
        }
    }
}
