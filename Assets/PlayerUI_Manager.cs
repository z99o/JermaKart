using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public enum UI_State
{
    Uninitialized,
    Inititalized,
    Countdown,
    Normal,
    FinishedRace,
    ItemShuffle,

}
public class PlayerUI_Manager : MonoBehaviour
{
   
    private Game_Manager manager;
    public Racer_Player player;
    [SerializeField]
    private Animator animator;
    public UI_State uiState;
    [SerializeField]
    private TextMeshProUGUI UI_Timer;
    [SerializeField]
    private TextMeshProUGUI UI_Split1;
    [SerializeField]
    private TextMeshProUGUI UI_Split2;
    [SerializeField]
    private TextMeshProUGUI UI_Split3;
    [SerializeField]
    private TextMeshProUGUI UI_LapNum;
    [SerializeField]
    private TextMeshProUGUI UI_Place;
    [SerializeField]
    private TextMeshProUGUI UI_PlaceFlavor;
    [SerializeField]
    private Image UI_ItemFrame;
    [SerializeField]
    private Image UI_Speedo_Img;
    [SerializeField]
    private TextMeshProUGUI UI_Speedo_Text;
    [SerializeField]
    private Image UI_PlayerIcon;
    private Animator UI_SpeedLines;
    [SerializeField]
    private Sprite normalFace;
    [SerializeField]
    private Sprite boostFace;
    private Item curItem;
    private int curPlace;
    private int curLapNum;
    private float timer;
    private float lapTimer1;
    private float lapTimer2;
    private float lapTimer3;
    private float bestSplit;
    private float bestTime;
    [SerializeField]
    private List<Item> shuffleItems = new List<Item>();
    private AudioManager audioManager;
    public void Initialize(Racer_Player player) //called by player
    {
        this.player = player;
        manager = Game_Manager._Instance;
        UI_Timer = GameObject.FindGameObjectWithTag("UI_Timer").GetComponent<TextMeshProUGUI>();
        UI_Split1 = GameObject.FindGameObjectWithTag("UI_Split1").GetComponent<TextMeshProUGUI>();
        UI_Split2 = GameObject.FindGameObjectWithTag("UI_Split2").GetComponent<TextMeshProUGUI>();
        UI_Split3 = GameObject.FindGameObjectWithTag("UI_Split3").GetComponent<TextMeshProUGUI>();
        UI_LapNum = GameObject.FindGameObjectWithTag("UI_LapNum").GetComponent<TextMeshProUGUI>();
        UI_ItemFrame = GameObject.FindGameObjectWithTag("UI_ItemFrame").GetComponent<Image>();
        UI_PlayerIcon = GameObject.FindGameObjectWithTag("UI_PlayerIcon").GetComponent<Image>();
        UI_Place = GameObject.FindGameObjectWithTag("UI_Place").GetComponent<TextMeshProUGUI>();
        UI_PlaceFlavor = GameObject.FindGameObjectWithTag("UI_PlaceFlavor").GetComponent<TextMeshProUGUI>();
        UI_Speedo_Img = GameObject.FindGameObjectWithTag("UI_SpeedoImage").GetComponent<Image>();
        UI_Speedo_Text =  GameObject.FindGameObjectWithTag("UI_SpeedoText").GetComponent<TextMeshProUGUI>();
        UI_SpeedLines = GameObject.FindGameObjectWithTag("UI_Speedlines").GetComponent<Animator>();
        audioManager = Game_Manager._Instance.audioManager;
        bestSplit = PlayerPrefs.GetFloat(manager.raceName + "BestSplit");
        bestTime = PlayerPrefs.GetFloat(manager.raceName + "BestTime");
        DisplayItem(null);
        uiState = UI_State.Inititalized;
        Debug.Log(this.name + "Initialized");
        //get all the 
    }

    public float GetTimer()
    {
        return timer;
    }

    public float[] GetSplits()
    {
        return new float[] {lapTimer1, lapTimer2, lapTimer3};
    }

    public void ToggleSpeedLines(bool on)
    {
        if (on)
            UI_SpeedLines.Play("On");
        else
            UI_SpeedLines.Play("Off");
    }

    public void ItemShuffle()
    {
        //play item shuffle animation
        if(shuffleItems.Count <= 0)
        {
            Debug.LogError("No items to give!");
        }
        //shuffleItems.Shuffle();
        StartCoroutine(Routine_Shuffle());
        uiState = UI_State.ItemShuffle;
        
    }

    IEnumerator Routine_Shuffle()
    {
        int counter = 0;
        float speed = 0.05f; //decreases by a lesser factor
        float pitchInc = 0.5f;
        float pitchMax = 1 + (pitchInc * 3);
        float curPitch = 1;
        UI_ItemFrame.transform.parent.gameObject.SetActive(true);
        for (int i = 0; i < shuffleItems.Count * 2; i++)
        {
            counter++;
            if (counter > shuffleItems.Count - 1)
                counter = 0;
            UI_ItemFrame.sprite = shuffleItems[counter].icon;
            speed += 0.01f;
            curPitch += pitchInc;
            if (curPitch > pitchMax)
                curPitch = 1;
            audioManager.PlayManagerClip("bell_1");
            audioManager.SetManagerPitch(curPitch);
            yield return new WaitForSeconds(speed);
        }
        audioManager.SetManagerPitch(1);
        curItem = shuffleItems[counter];
        player.GiveItem(curItem);
        uiState = UI_State.Normal;
    }

    public void SetState(UI_State state)
    {
        uiState = state;
    }


    // Update is called once per frame
    public void UpdateUI(int lapNum, int place, float speed, Item item, bool boost)
    {
        switch (uiState)
        {
            case UI_State.Uninitialized:
                break;
            case UI_State.Inititalized:
                DisplayItem(null);
                break;
            case UI_State.Countdown:
                break;
            case UI_State.Normal:
                DisplayTime(timer,lapNum);
                DisplayItem(item);
                break;
            case UI_State.ItemShuffle:
                //DisplayItem(item);
                DisplayTime(timer,lapNum);
                break;
            case UI_State.FinishedRace:
                break;
            default:
                break;
        }
        DisplayBoost(boost);
        //DisplayTime(timer,lapNum);
        DisplayLapNum(lapNum);
        DisplayPlace(place);
        DisplaySpeedo(speed);
        //display lapNum if we have one
        //display item if we have one
    }

    void DisplayBoost(bool boost)
    {
        UI_PlayerIcon.sprite = normalFace;
        if (boost)
        {

            UI_PlayerIcon.sprite = boostFace;
            ToggleSpeedLines(true);
        }
        else
            ToggleSpeedLines(false);
    }
    void DisplayTime(float timeToDisplay,int lap)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60f);
        float seconds = Mathf.FloorToInt((timeToDisplay % 60f));
        float milliseconds = Mathf.FloorToInt((timeToDisplay * 1000f) % 1000);
        UI_Timer.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        if(bestTime != 0 && timeToDisplay <= bestTime)
        {
            UI_Timer.color = Color.yellow;
        }
        else if (bestTime != 0 && timeToDisplay > bestTime)
        {
            UI_Timer.color = Color.red;
        }
        TextMeshProUGUI GUIToDisplay = null;
        switch (lap)
        {
            case 1:
                timeToDisplay = lapTimer1;
                GUIToDisplay = UI_Split1;
                break;
            case 2:
                timeToDisplay = lapTimer2;
                GUIToDisplay = UI_Split2;
                break;
            case 3:
                timeToDisplay = lapTimer3;
                GUIToDisplay = UI_Split3;
                break;
        }
        minutes = Mathf.FloorToInt(timeToDisplay / 60f);
        seconds = Mathf.FloorToInt((timeToDisplay % 60f));
        milliseconds = Mathf.FloorToInt((timeToDisplay * 1000f) % 1000);
        if (bestSplit != 0 && timeToDisplay <= bestSplit )
        {
            //bestSplit = timeToDisplay;
            GUIToDisplay.color = Color.green;
        }
        else if(bestSplit != 0 && timeToDisplay > bestSplit)
        {
            GUIToDisplay.color = Color.red;
        }
        GUIToDisplay.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        switch (lap)
        {
            case 1:
                lapTimer1 += Time.deltaTime;
                break;
            case 2:
                lapTimer2 += Time.deltaTime;
                break;
            case 3:
                lapTimer3 += Time.deltaTime;
                break;
        }
        timer += Time.deltaTime;
    }

    void DisplayItem(Item newItem)
    {
        if (newItem == null) {
            UI_ItemFrame.transform.parent.gameObject.SetActive(false);
        }
        else if(curItem == null && newItem != null)
        {
            UI_ItemFrame.transform.parent.gameObject.SetActive(true);
            UI_ItemFrame.sprite = newItem.icon;
        }
        else if (newItem != null && curItem != null && curItem.itemName != newItem.itemName) {
            UI_ItemFrame.transform.parent.gameObject.SetActive(true);
            UI_ItemFrame.sprite = newItem.icon;
        }
    }

    void DisplayLapNum(int lapNum)
    {
        if(lapNum != curLapNum)
        {
            switch (curLapNum)
            {
                case 1:
                    if(lapTimer1 < bestSplit)
                        bestSplit = lapTimer1;
                    break;
                case 2:
                    if (lapTimer2 < bestSplit)
                        bestSplit = lapTimer2;
                    break;
                case 3:
                    if (lapTimer3 < bestSplit)
                        bestSplit = lapTimer3;
                    break;
            }
            UI_LapNum.text = lapNum.ToString() + '/' + Game_Manager._Instance.lapNum;
            curLapNum = lapNum;
        }
    }
    void DisplayPlace(int place)
    {
        if (curPlace != place)
        {
            string suffix = "th";
            if (place == 1)
                suffix = "st";
            else if (place == 2)
                suffix = "nd";
            else if (place == 3)
                suffix = "rd";
            UI_Place.text = place.ToString();
            UI_PlaceFlavor.text = suffix;
            curPlace = place;
        }
    }

    public void Countdown()
    {
        //animator.Play("Countdown");
        Event_Countdown();
    }
   
    public void Event_Countdown()
    {
        //do countdown
        uiState = UI_State.Normal;
        player.StartRacing();
    }

    void DisplaySpeedo(float speed)
    {
        float maxSpeed = 60f;
        float fraction = (speed / maxSpeed)/4f;
        UI_Speedo_Img.fillAmount = fraction;
        UI_Speedo_Text.text = ((int)speed*4).ToString();
        //radial fill this asset
    }
}
