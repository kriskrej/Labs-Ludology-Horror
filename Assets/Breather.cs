using System.Collections.Generic;
using UnityEngine;

public class Breather : MonoBehaviour {
    public List<AudioClip> mp3s = new List<AudioClip>();
    public AudioSource audioSource;
    
    
    
    void Update() {
        if (!audioSource.isPlaying)
            PlayRandomMp3();
    }

    void PlayRandomMp3() {
        audioSource.PlayOneShot( SelectRandomMp3() );
        audioSource.volume = Random.Range(0f, 2f);
    }

    AudioClip SelectRandomMp3() {
        var id = Random.Range(0, mp3s.Count);
        return mp3s[id];
    }
}