using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class KartAudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource motorSource;
    public AudioSource driftSource;
    public AudioSource sfxSource;
    public AudioManager audioManager;
    [SerializeField]
    private AudioInfo hop;
    [SerializeField]
    private AudioInfo jump;
    [SerializeField]
    private AudioInfo grind;
    [SerializeField]
    private AudioInfo drift;
    [SerializeField]
    private AudioInfo motor;
    [SerializeField]
    private AudioInfo win;
    [SerializeField]
    private AudioInfo lose;
    [SerializeField]
    private AudioInfo hurt;
    [SerializeField]
    private AudioInfo boost;
    [SerializeField]
    private AudioInfo dash;
    private float sfxModifier;
    public void Initialize()
    {
        //subscribe to the audio settings changed   
        //startMotor
        audioManager = Game_Manager._Instance.audioManager;
        audioManager.m_AudioSettingsEvent.AddListener(UpdateSettings);
        UpdateSettings();
    }

    void UpdateSettings()
    {
        sfxModifier = audioManager.GetSFXVolume();
        driftSource.volume = drift.volume * sfxModifier;
        motorSource.volume = motor.volume * sfxModifier;
    }


    public void Sound_Dash()
    {
        sfxSource.PlayOneShot(dash.clip, dash.volume * sfxModifier);
    }
    public void Sound_Jump(int numJumps = 0)
    {
        sfxSource.PlayOneShot(jump.clip, jump.volume * sfxModifier);
        //float pitch = 1f + (0.25f * numJumps);
        //Action action = new Action(delegate { pitch = 1; sfxSource.volume = 1;});
        //sfxSource.pitch = pitch;
        //sfxSource.clip = jump.clip;
        //sfxSource.volume = jump.volume * sfxModifier;
        //sfxSource.Play();
        //StartCoroutine(CoroutineUtils.DoOnSoundFinished(action, sfxSource));
    }
    public void Sound_Hop()
    {
        sfxSource.PlayOneShot(hop.clip, hop.volume * sfxModifier);
    }

    public void Sound_Win()
    {
        sfxSource.PlayOneShot(win.clip, win.volume * sfxModifier);
    }
    public void Sound_Lose()
    {
        sfxSource.PlayOneShot(lose.clip, lose.volume * sfxModifier);
    }

    public void Sound_Hurt()
    {
        sfxSource.PlayOneShot(hurt.clip, hurt.volume * sfxModifier);
    }
    public void Sound_Boost()
    {
        sfxSource.PlayOneShot(boost.clip, boost.volume * sfxModifier);
    }

    public void ToggleMotor(bool on)
    {
        if (on)
        {
            motorSource.clip = motor.clip;
            motorSource.volume = motor.volume * sfxModifier;
            motorSource.Play();
        }
        else
        {
            motorSource.Pause();
        }
    }

    public void Toggle_Drift(bool on)
    {
        if (on)
        {
            driftSource.clip = drift.clip;
            driftSource.volume = drift.volume * sfxModifier;
            driftSource.Play();
        }
        else
        {
            driftSource.Pause();
        }
    }
    public void Toggle_Grind(bool on)
    {

        if (on)
        {
            driftSource.clip = grind.clip;
            driftSource.volume = grind.volume;
            driftSource.Play();
        }
        else
        {
            driftSource.Pause();
        }
    }

    public void ModifyPitch(float factor)
    {
        motorSource.pitch = 1+ (1 * factor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
