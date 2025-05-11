using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance
    {
        get;
        private set;
    }

    [SerializeField]
    private AudioSource bgm;

    [SerializeField]
    private AudioSource clickSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(float volume)
    {
        bgm.volume = volume;
        bgm.loop = true;
        bgm.Play();
    }

    public void PlayClickSound(float volume)
    {
        clickSound.PlayOneShot(clickSound.clip, volume);
    }

    public void SetBGMVolume(float v)
    {
        bgm.volume = v;
        bgm.Play();
    }

    public void SetClickVolume(float v)
    {
        clickSound.volume = v;
    }
}
