using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioInfo clip;
    [SerializeField]
    [Range(0f, 10f)]
    private float boostStrength = 2f;
    [SerializeField]
    [Range(0f, 10f)]
    private float boostDuration = 0.75f;
    [SerializeField]
    [Range(0f, 1f)]
    private float forceDirectionDuration = 0.25f;
    [SerializeField]
    private bool isLaunchPad = false;
    private void Start()
    {
        clip.volume *= Game_Manager._Instance.audioManager.GetSFXVolume();
    }

    public bool getIsLaunchPad()
    {
        return isLaunchPad;
    }

    public float getBoostDuration()
    {
        return boostDuration;
    }
    public float getBoostStrength()
    {
        return boostStrength;
    }
    public float getForceDirectionDuration()
    {
        return forceDirectionDuration;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Racer")
        {
            audioSource.PlayOneShot(clip.clip,clip.volume);
        }
    }
}
