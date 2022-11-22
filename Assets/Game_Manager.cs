using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Linq;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public enum GameState
{
    Unitialized,
    PreRace,
    Running,
    Finished,
}
public class Game_Manager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private Racer_Player player;
    [SerializeField]
    private SplineComputer main_spline;
    public static Game_Manager _Instance;
    [SerializeField]
    public List<Racer> racers;
    [SerializeField]
    public int lapNum = 3;
    public TransitionManager transitionManager;
    public PauseMenu pauseMenu;
    public Slider sfx_slider;
    public Slider music_slider;
    public bool paused;
    public bool exiting;
    public AudioManager audioManager;
    public Cinemachine.CinemachineVirtualCamera mainCam;
    public float timeScale = 1f;
    public GatePlacer progressGate;
    public GameState gameState;

    public RectTransform startLight;
    public Image startRed;
    public Image startYellow;
    public Image startGreen;
    public string raceName = "NoName";
    public GameObject racerJerma;
    public GameObject racerOtto;
    public GameObject racer3;

    public RectTransform finishText;
    private Inputs inputs;
    [SerializeField]
    private List<Item> itemPool;

    public Item GetRandomItem()
    {
        int count = itemPool.Count;
        if (count == 0)
            return null;
        int rand = Random.Range(0, count);
        return itemPool[rand];
    }
    public Vector3 GetNearestPointOnSpline(Vector3 pos)
    {
        return main_spline.Project(pos).position;
    }
    public int GetNumGates()
    {
        return progressGate.fidelity - 1;
    }

    public SplineComputer GetMainSpline()
    {
        return main_spline;
    }

    void Awake()
    {
        _Instance = this;
        string curRacer = PlayerPrefs.GetString("CurrentRacer");
        switch (curRacer)
        {
            case "Jermario":
                racerJerma.SetActive(true);
                player = racerJerma.GetComponent<Racer_Player>();
                racers.Add(racerJerma.GetComponent<Racer>());
                break;
            case "Otto":
                racerOtto.SetActive(true);
                player = racerOtto.GetComponent<Racer_Player>();
                racers.Add(racerOtto.GetComponent<Racer>());
                break;
            case "Racer3":
                racer3.SetActive(true);
                player = racer3.GetComponent<Racer_Player>();
                racers.Add(racer3.GetComponent<Racer>());
                break;
            default:
                racerJerma.SetActive(true);
                player = racerJerma.GetComponent<Racer_Player>();
                racers.Add(racerJerma.GetComponent<Racer>());
                break;
        }
        player.UI_Manager.Initialize(player);
        inputs = player.inputs;
        inputs.SetCursorState(CursorLockMode.Locked);
        PlayerBikeController pController = (PlayerBikeController)player.GetController();
        mainCam.m_Follow = pController.GetFollow();
        mainCam.m_LookAt = pController.GetLookAt();
        StartCoroutine(Detrans());
    }

    IEnumerator Detrans()
    {
        transitionManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        transitionManager.EnterSceneTransition(Initialize);
    }


    private void Initialize()
    {
        //define ourselves as a scene singletons
        _Instance = this;
        //Initialize Scene components that need to be in order
        //racers = GameObject.FindGameObjectsWithTag("Racer").Select(a => a.GetComponent<Racer>()).ToList();
        sfx_slider.value = PlayerPrefs.GetFloat("SFX");
        music_slider.value = PlayerPrefs.GetFloat("Music");
        
        Debug.Log(this.name + "Initialized");

        for (int i = 0; i < racers.Count; i++)
        {
            racers[i].Initialize();
        }
        var rails = GameObject.FindObjectsOfType<RailScript>();
        for (int i = 0; i < rails.Count(); i++)
            rails[i].GetComponent<RailScript>().GenerateColliders();
        gameState = GameState.PreRace;
        StartCoroutine(RaceCountdown());
    }

    IEnumerator RaceCountdown()
    {
        for (int i = 0; i < racers.Count; i++)
        {
            racers[i].SetState(RacerState.Countdown);
        }
        yield return startLight.DOLocalMove(new Vector3(0, 150, 0),1f).WaitForCompletion();
        yield return new WaitForSeconds(.25f);
        audioManager.PlayManagerClip("bell_2");
        yield return new WaitForSeconds(1f);
        audioManager.PlayManagerClip("bell_2");
        startRed.color = Color.white;
        yield return new WaitForSeconds(1f);
        audioManager.PlayManagerClip("bell_2");
        startYellow.color = Color.white;
        yield return new WaitForSeconds(1f);
        audioManager.PlayManagerClip("bell_2",pitch:2);
        startGreen.color = Color.white;
        for (int i = 0; i < racers.Count; i++)
        {
            racers[i].StartRace();
        }
        yield return startLight.DOLocalMove(new Vector3(0, 500, 0), 1f);
        gameState = GameState.Running;
        player.UI_Manager.SetState(UI_State.Normal);
        switch (raceName)
        {
            case "JermaHouse":
             audioManager.PlayMusic("unnamed song 2");
                break;
            default:
                audioManager.PlayMusic("Derby Dash");
                break;
        }

    }

    private void Start()
    {
        //transition into game
        
    }

    public void EndGame()
    {
        //Save Splits To PlayerPrefs
        float[] splits = player.UI_Manager.GetSplits();
        float bestSplit = splits[0];
        for(int i = 0; i < splits.Length; i++)
        {
            if (splits[i] < bestSplit)
                bestSplit = splits[i];
        }
        PlayerPrefs.SetFloat(raceName + "BestSplit", bestSplit);
        PlayerPrefs.SetFloat(raceName + "BestTime", player.UI_Manager.GetTimer());
        gameState = GameState.Finished;
        audioManager.PlayManagerClip("finish");
        StartCoroutine(Event_EndGame());
    }

    

    IEnumerator Event_EndGame()
    {
        yield return finishText.DOLocalMove(new Vector3(0, 50, 0), 1f).WaitForCompletion();
        audioManager.PlayMusic("MainMenu");
        yield return new WaitForSeconds(7f);
        transitionManager.SweepRight(() => SceneManager.LoadScene("Scene_Menu_Select"));
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
        UpdatePlace();
        if (inputs.input_pause && !exiting && gameState == GameState.Running){
            paused = !paused;
            if (paused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        pauseMenu.gameObject.SetActive(true);
        audioManager.PlayManagerClip("sweep_short");
        inputs.SetCursorState(CursorLockMode.None);
        inputs.input_pause = false;
        for (int i = 0; i < racers.Count; i++)
        {
            racers[i].TogglePaused(true);
        }
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        PlayerPrefs.Save();
        audioManager.UpdateAudio();
        audioManager.PlayManagerClip("interface_exit");
        inputs.SetCursorState(CursorLockMode.Locked);
        inputs.input_pause = false;
        pauseMenu.gameObject.SetActive(false);
        for (int i = 0; i < racers.Count; i++)
        {
            racers[i].TogglePaused(false);
        }
        Time.timeScale = 1;
    }

    public void ExitToMenu()
    {
        exiting = true;
        audioManager.PlayManagerClip("interface_confirm");
        Time.timeScale = 1;
        transitionManager.SweepRight(Event_Exit);
        PlayerPrefs.Save();
    }

    public void Event_Exit()
    {
        DOTween.KillAll(true);
        SceneManager.LoadScene("Scene_Menu_Main");
    }

    public void Slider_Music()
    {
        PlayerPrefs.SetFloat("Music", music_slider.value);
        if ((int)(100 * sfx_slider.value) % 7 == 0)
        {
            audioManager.UpdateAudio();
        }
    }

    public void Slider_SFX()
    {
        PlayerPrefs.SetFloat("SFX", sfx_slider.value);
        //expensive, only use when paused
        if ((int)(100 * sfx_slider.value) % 7 == 0)
        {
            audioManager.UpdateAudio();
            audioManager.PlayManagerClip("button_press");
        }
    }

    public Racer_Player GetPlayer()
    {
        return player;
    }

    public void EscapeGBJ()
    {
        player.Respawn();
    }

    private void UpdatePlace()
    {
        //OrderByDescending(a => main_spline.Project(a.GetController().transform.position).percent)
        racers = racers.OrderByDescending(a => a.GetRacePosition().lap)
                       .ThenByDescending(a => a.numProgress).ToList();
                        
              
        for(int i = 0; i < racers.Count; i++)
        {
            racers[i].SetPlace(i + 1);
        }
    }

    public int GetPlace(RacePosition racePos)
    {
        return 1;
    }
}
