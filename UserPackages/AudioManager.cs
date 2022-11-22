using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class AudioInfo
{
    [Range(0f, 1f)]
    public float volume;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource managerSource;
    public AudioSource musicSource;
    [SerializeField]
    public List<AudioInfo> audioClips;
    private float sfxVol;
    private float musicVol;
    public string music_name;

    private void Start()
    {
        UpdateAudio();
    }

    private void Update()
    {
    }


    public float GetSFXVolume()
    {
        UpdateAudio();
        return PlayerPrefs.GetFloat("SFX");
    }

    public float GetMusicVolume()
    {
        UpdateAudio();
        return PlayerPrefs.GetFloat("Music");
    }

    public void UpdateAudio()
    {
        sfxVol = PlayerPrefs.GetFloat("SFX");
        musicVol = PlayerPrefs.GetFloat("Music");
        try
        {
            musicSource.volume = GetClip(musicSource.clip.name).volume * musicVol;
        }
        catch
        {
            //catch nothing and continue
        }
    }

    public AudioInfo GetClip(string clip)
    {
        //UpdateAudio();
        return audioClips.FirstOrDefault(a => a.clip.name == clip);

    }

    public void PlayManagerClip(string clip, bool sfx = true)
    {
        float factor = sfxVol;
        if (sfx == false)
            factor = musicVol;
        //UpdateAudio();
        AudioInfo sound = audioClips.FirstOrDefault(a => a.clip.name == clip);
        managerSource.PlayOneShot(sound.clip,sound.volume * factor);
    }

    public void PlayMusic(string clip)
    {
        //UpdateAudio();
        AudioInfo sound = audioClips.FirstOrDefault(a => a.clip.name == clip);
        musicSource.clip = (sound.clip);
        musicSource.volume = sound.volume * musicVol;
        musicSource.Play();
    }

}
