using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource MyAudio;

    public AudioClip[] Clips;

    public void PlaySound(int i)
    {
        MyAudio.clip = Clips[i];
        MyAudio.Play();
    }
}
