using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GameData;

[RequireComponent(typeof(AudioSource))]
public class UISoundManager :MonoSingleton<UISoundManager>
{ 
    [System.Serializable]
    public class Music
    {
        public MusicType type;
        public AudioSource sound;
        public float soundFadeOutTime = 0.2f;
        public float soundFadeInTime = 0.2f;
        public float soundVolume = 1;
        public bool isLoop = false;
    }

    //  UI界面所有的相关背景音乐及音效
    public Music[] musics;

    public void MusicOn(MusicType _type)
    {
        StartCoroutine(PlayMusic(_type));
    }

    public void MusicOff(MusicType _type)
    {
        StartCoroutine(StopMusic(_type));
    }

    private IEnumerator PlayMusic(MusicType _type)
    {
        foreach (Music music in musics)
        {
            if (music.type == _type)
            {
                yield return new WaitForSeconds(music.soundFadeInTime);
                InitProp(music);
                music.sound.Play();
                yield return new WaitForSeconds(music.sound.clip.length + music.soundFadeOutTime);
                break;
            }
        }

        yield return null;
    }

    private IEnumerator StopMusic(MusicType _type)
    {
        foreach (Music music in musics)
        {
            if (music.type== _type)
            {
                music.sound.Stop();
                break;
            }
        }

        yield return null;
    }

    private void InitProp(Music music)
    {
        music.sound.volume = music.soundVolume;
        music.sound.loop = music.isLoop;
    }
}
