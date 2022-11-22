using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject main_buttons;
    public Button startButton;
    public GameObject option_buttons;
    public Animator animator;
    public Slider music_slider;
    public Slider sfx_slider;
    public TMPro.TextMeshProUGUI music_percent;
    public TMPro.TextMeshProUGUI sfx_percent;
    public AudioManager audioManager;
    public AudioSource musicSource;
    public GameObject credits;
    public TransitionManager transitionManager;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("SFX"))
            sfx_slider.value = PlayerPrefs.GetFloat("SFX");
        else
            sfx_slider.value = 0.5f;
        if (PlayerPrefs.HasKey("Music"))
            music_slider.value = PlayerPrefs.GetFloat("Music");
        else
            music_slider.value = 0.5f;
        PlayerPrefs.SetInt("PlayOpening", 0);
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start()
    {
        if (transitionManager == null)
            return;
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return new WaitForSeconds(0.1f);
        transitionManager.EnterSceneTransition(PlayIntro);
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
        audioManager.PlayManagerClip("interface_confirm");
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
        startButton.interactable = false;
        audioManager.PlayManagerClip("startup_motor");
        //Event_StartGame();
        animator.Play("StartGame");
    }
    public void OC_EndGame()
    {
        audioManager.PlayManagerClip("button_press");
        transitionManager.SweepRight(Event_EndGame);
        //animator.Play("EndGame");
    }

    public void Slider_Music()
    {
        PlayerPrefs.SetFloat("Music", music_slider.value);
        audioManager.UpdateAudio();
    }

    public void Slider_SFX()
    {
        PlayerPrefs.SetFloat("SFX", sfx_slider.value);
        if((int)(100 *sfx_slider.value) % 3 == 0)
            audioManager.PlayManagerClip("button_press");
        audioManager.UpdateAudio();
    }

    public void Event_StartGame()
    {
        audioManager.managerSource.DOFade(0, transitionManager.transitionSpeed);
        audioManager.musicSource.DOFade(0, transitionManager.transitionSpeed);
        transitionManager.SweepRight(() => StartCoroutine(LoadScene("Scene_Menu_Select")));

        //get our current save state;

    }

    IEnumerator LoadScene(string scene)
    {
        DOTween.KillAll(true);
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(scene);
    }

    public void Event_EndGame()
    {
        //on next open  we play the opening.
        PlayerPrefs.SetInt("PlayOpening", 1);
        Application.Quit();
    }

    public void PlayIntro()
    {
        animator.Play("Intro");
    }

    public void PlayIntroSound()
    {
        audioManager.PlayManagerClip("jermkart");
    }

    public void StartMusic()
    {
        musicSource.Play();
    }

    private static void SceneManagerOnSceneUnloaded(Scene scene)
    {
        DOTween.KillAll();
    }
}
