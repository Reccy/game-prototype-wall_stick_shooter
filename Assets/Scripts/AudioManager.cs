using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    public List<AudioClip> audioClipList;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShot(string name, float volume)
    {
        for(int i = 0; i < audioClipList.Count; i++)
        {
            if(audioClipList[i].name == name)
            {
                audioSource.PlayOneShot(audioClipList[i], volume);
            }
        }
    }
}
