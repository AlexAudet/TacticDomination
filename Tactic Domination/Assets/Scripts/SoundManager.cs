using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    SoundManager()
    {
        Instance = this;
    }

    AudioSource audioSource;

    public List<AudioClass> UiAudio = new List<AudioClass>();


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayUISound(string clipName) 
    {
        AudioClass clipClass = null;

        foreach (var clip in UiAudio)
            if (clip.name == clipName)
                clipClass = clip;

        if(clipClass == null)
        {
            Debug.LogError("There's no clip with this name");
            return;
        }

        if(clipClass.audioClip != null)
        {
            audioSource.clip = clipClass.audioClip;
            audioSource.volume = clipClass.volume;
            audioSource.pitch = clipClass.pitch;
        }
        else
        {
            Debug.LogError("There's no clip to play");
            return;
        }



        audioSource.Play();
    }
}

[System.Serializable]
public class AudioClass
{
    [PropertySpace(10, 0)]
    public string name;
    public AudioClip audioClip;
    [PropertySpace(10,0)]
    [Range(0,1)]
    public float volume = 1;
    [PropertySpace(0, 10)]
    [Range(-3, 3)]
    public float pitch = 1;
}
