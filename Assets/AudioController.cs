using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioSource OneShotPlayer;
    public AudioClip[] audioClips;
    public AudioMixerSnapshot[] audioMixerSnapshots;
    //0 - Chicken Death, 1 - Chicken Transform, 2 - Jump, 3 - Chicken Walk, 4 - Fox Walk

    // Start is called before the first frame update
    public void PlayOneShot(int OneShotID)
    {
        OneShotPlayer.PlayOneShot(audioClips[OneShotID]);
    }

    public void ChangeMusic(int musicMixID)
    {
        //0 - Main, 1 - Chill, 2 - Scary
        audioMixerSnapshots[musicMixID].TransitionTo(5f);
    }
}
