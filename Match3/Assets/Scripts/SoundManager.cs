using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource source;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        source = GetComponent<AudioSource>();
    }
    public void PlaySound(AudioClip sound)
    {
        source.clip = sound;
        source.Play();
    }
}
