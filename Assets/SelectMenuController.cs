using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class SelectMenuController : MonoBehaviour
{
    // Start is called before the first frame update

    public CarasolController stageSelect;
    public CarasolController characterSelect;
    public Animator animator;
    public TransitionManager transitionManager;
    public AudioManager audioManager;
    public GameObject startModal;
    [SerializeField]
    float errorStrength = 1f;
    void Awake()
    {

    }
    void Start()
    {
        StartCoroutine(Initialize());
    }
    IEnumerator Initialize()
    {
        yield return new WaitForSeconds(0.1f);
        transitionManager.EnterSceneTransition(() => Cursor.lockState = CursorLockMode.Confined);
    }

    public void OnClick_OpenModal()
    {
        CarasolData activeStage = stageSelect.GetSelectedMember().GetData();
        CarasolData activeChar = characterSelect.GetSelectedMember().GetData();
        //shake the 
        if (!activeStage.selectable)
            stageSelect.GetSelectedMember().transform.DOShakePosition(1,errorStrength);
        if (!activeChar.selectable)
            characterSelect.GetSelectedMember().transform.DOShakePosition(1,errorStrength);

        if (!activeStage.selectable || !activeChar.selectable)
        {
            audioManager.PlayManagerClip("interface_exit");
            return;
        }
        audioManager.PlayManagerClip("button_press");
        startModal.SetActive(true);
    }

    public void OnClick_ConfirmModal()
    {
        audioManager.PlayManagerClip("interface_confirm");
        CarasolData activeStage = stageSelect.GetSelectedMember().GetData();
        CarasolData activeChar = characterSelect.GetSelectedMember().GetData();
        transitionManager.SweepRight(() => EnterRace(activeStage.data_name, activeChar.data_name));
    }

    public void OnClick_CloseModal()
    {
        audioManager.PlayManagerClip("interface_exit");
        startModal.SetActive(false);
    }

    public void OnClick_Exit()
    {
        transitionManager.SweepRight(Exit);
        //play the transition 
    }

    public void Exit()
    {
        audioManager.PlayManagerClip("interface_exit");
         StartCoroutine(LoadScene("Scene_Menu_Main"));
    }

    public void EnterRace(string stage, string character)
    {
        //get the race info and the character we're using from the carasoels
        string id = "1";
        switch (stage)
        {
            case "The Meat Grinder":
                id = "3";
                break;
            case "Jerma Park":
                id = "1";
                break;
            default:
                id = "1";
                break;
        }
        PlayerPrefs.SetString("CurrentRacer", character);
        StartCoroutine(LoadScene("Scene_Race_" + id));
    }

    IEnumerator LoadScene(string scene)
    {
        yield return new WaitForEndOfFrame();
        DOTween.KillAll(true);
        SceneManager.LoadScene(scene);
    }


    private static void SceneManagerOnSceneUnloaded(Scene scene)
    {
        DOTween.KillAll();
    }
}
