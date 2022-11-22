using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreTracker : MonoBehaviour
{
    // Start is called before the first frame update
    public string raceName;
    public TMPro.TextMeshProUGUI view;
    void Start()
    {
        float bestLap = PlayerPrefs.GetFloat(raceName + "BestSplit");
        float bestTime = PlayerPrefs.GetFloat(raceName + "BestTime");
        float minutes = Mathf.FloorToInt(bestLap / 60f);
        float seconds = Mathf.FloorToInt((bestLap % 60f));
        float milliseconds = Mathf.FloorToInt((bestLap * 1000f) % 1000);
        string lapText = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
        minutes = Mathf.FloorToInt(bestTime / 60f);
        seconds = Mathf.FloorToInt((bestTime % 60f));
        milliseconds = Mathf.FloorToInt((bestTime * 1000f) % 1000);
        string timeText = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);

        view.text = string.Format("Best Time : {0} \nBest Lap: {1}",
            timeText,
            lapText);
    }

}
