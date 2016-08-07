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
    
    void Update()
    {
        if(Time.timeScale == 1)
        {
            audioSource.pitch = 1;
        }
        else
        {
            audioSource.pitch = 0.8f;
        }
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
