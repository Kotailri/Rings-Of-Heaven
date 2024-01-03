using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brackey's Audio Manager
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.7f;

    [Range(0f, 1f)]
    public float pitch = 1f;

    [Header("Variance")]
    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;

    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    private AudioSource source;
    public bool loops = false;
    public bool isMusic = false;
    public bool playOnAwake = false;
    public bool muteOnAwake = false;
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }

    public void ChangeVolume(float vol)
    {
        if (source != null)
            source.volume = vol * volume;
    }

    public void Play()
    {
        if (isMusic)
        {
            source.volume = Config.musicVolume * volume;
        } 
        else
        {
            source.volume = Config.soundVolume * volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        }

        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.loop = loops;
        source.Play();
    }

    public void Stop()
    {
        if (source.isPlaying)
        {
            source.Stop();
        }
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    

    public static Sound CurrentMusic;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Audio Manager found");
        }    
        else
        {
            instance = this;
        }
    }

    [SerializeField]
    List<Sound> sounds = new List<Sound>();

    private void Start()
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
            if (sounds[i].isMusic) 
            { 
                CurrentMusic = sounds[i];
            }

            if (sounds[i].playOnAwake)
            {
                sounds[i].Play();
                if (sounds[i].muteOnAwake)
                {
                    sounds[i].ChangeVolume(0);
                }
            }
        }
        //StartCoroutine(WaitMusic());
    }

    private IEnumerator WaitMusic()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        PlaySound("Music");
    }

    public void Mute(string _name)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].ChangeVolume(0);
                return;
            }
        }
        Debug.LogWarning(_name + " not found in Audio Manager");
    }

    public void Unmute(string _name)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].ChangeVolume(1);
                return;
            }
        }
        Debug.LogWarning(_name + " not found in Audio Manager");
    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }
        Debug.LogWarning(_name + " not found in Audio Manager");
    }

    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Stop();
                return;
            }
        }
        Debug.LogWarning(_name + " not found in Audio Manager");
    }
}
