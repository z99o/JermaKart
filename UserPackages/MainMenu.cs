using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject main_buttons;
    public GameObject option_buttons;
    public Animator animator;
    public Slider music_slider;
    public Slider sfx_slider;
    public TMPro.TextMeshProUGUI music_percent;
    public TMPro.TextMeshProUGUI sfx_percent;
    public AudioManager audioManager;
    public GameObject credits;
    private void Awake()
    {
        //if doesnt have key or the opening is set to 1
     if(!PlayerPrefs.HasKey("PlayOpening") ||PlayerPrefs.GetInt("PlayOpening") == 1)
        {
            PlayerPrefs.SetFloat("SFX", 1f);
            PlayerPrefs.SetFloat("Music", 1f);
            animator.Play("Intro");
        }
        sfx_slider.value = PlayerPrefs.GetFloat("SFX");
        music_slider.value = PlayerPrefs.GetFloat("Music");
        PlayerPrefs.SetInt("PlayOpening", 0);
    }

    public void Update()
    {
        music_percent.text = ((int)(100 * music_slider.value)).ToString() + "%";
        sfx_percent.text = ( (int)(100 *sfx_slider.value)).ToString() + "%";
    }

    public void OC_EnterOptions()
    {
        audioManager.PlayManagerClip("button_press");
        main_buttons.SetActive(false);
        option_buttons.SetActive(true);
    }
    public void OC_ExitOptions()
    {
        audioManager.PlayManagerClip("button_press");
        main_buttons.SetActive(true);
        option_buttons.SetActive(false);
    }

    public void OC_EnterCredits()
    {
        main_buttons.SetActive(false);
        option_buttons.SetActive(false);
        audioManager.PlayManagerClip("button_press");
        credits.SetActive(true);
    }

    public void OC_ExitCredits()
    {
        main_buttons.SetActive(true);
        audioManager.PlayManagerClip("button_press");
        credits.SetActive(false);
    }
    public void OC_StartGame()
    {
        audioManager.PlayManagerClip("button_press");
        animator.Play("StartGame");
    }
    public void OC_EndGame()
    {
        audioManager.PlayManagerClip("button_press");
        animator.Play("EndGame");
    }

    public void Slider_Music()
    {
        PlayerPrefs.SetFloat("Music", music_slider.value);
    }

    public void Slider_SFX()
    {
        PlayerPrefs.SetFloat("SFX", sfx_slider.value);
        if((int)(100 *sfx_slider.value) % 3 == 0)
            audioManager.PlayManagerClip("button_press");
    }

    public void Event_StartGame()
    {
        int inBattle = PlayerPrefs.GetInt("InBattle");
        int currentRun = PlayerPrefs.GetInt("CurrentRun");
        int dialogueNum = PlayerPrefs.GetInt("DialogueNum");
        int battleNum = PlayerPrefs.GetInt("BattleNum");
        string head = PlayerPrefs.GetString("Head" + currentRun);

        if (head.Contains("miku"))
            head = "miku";
        if (head.Contains("beepboy"))
            head = "beepboy";
        if (head.Contains("soldier"))
            head = "soldier";
        string scene_name = "Scene_";
        if (!PlayerPrefs.HasKey("RunStarted") || PlayerPrefs.GetInt("RunStarted") == 0)
        {
            scene_name += "CharacterCreator";
            SceneManager.LoadScene(scene_name);
            return;
        }
        else if(inBattle == 0)
        {
            //campfire
            scene_name += "Campfire_" + head + "_" + dialogueNum;
        }
        else
        {
            scene_name += "Battle_" + battleNum;
        }
        SceneManager.LoadScene(scene_name);
        //get our current save state;

    }

    public void Event_EndGame()
    {
        //on next open  we play the opening.
        PlayerPrefs.SetInt("PlayOpening", 1);
        Application.Quit();
    }
}
