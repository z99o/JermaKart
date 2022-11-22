using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum SparkState
{
    Off,
    Right,
    Left,
    Grind
}
public class SparkController : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer leftSpark;
    public Color curColor;
    public SpriteRenderer rightSpark;
    public SpriteRenderer grindSpark;
    public Animator animator;
    public SparkState sparkState;


    void Start()
    {
        leftSpark.gameObject.SetActive(false);
        rightSpark.gameObject.SetActive(false);
        grindSpark.gameObject.SetActive(false);
        animator.Play("Sparks");
    }

    public void SetSparkState(SparkState sparkState)
    {
        this.sparkState = sparkState;
    }

    // Update is called once per frame
    void Update()
    {
        //Color.Lerp(leftSpark.color, curColor, Time.deltaTime);
        //Color.Lerp(rightSpark.color, curColor, Time.deltaTime);
        switch (sparkState)
        {
            case SparkState.Off:
                rightSpark.gameObject.SetActive(false);
                leftSpark.gameObject.SetActive(false);
                grindSpark.gameObject.SetActive(false);
                rightSpark.color = Color.white;
                leftSpark.color = Color.white;
                grindSpark.color = Color.white;
                break;
            case SparkState.Right:
                grindSpark.gameObject.SetActive(false);
                leftSpark.gameObject.SetActive(false);
                rightSpark.gameObject.SetActive(true);
                break;
            case SparkState.Left:
                grindSpark.gameObject.SetActive(false);
                rightSpark.gameObject.SetActive(false);
                leftSpark.gameObject.SetActive(true);
                break;
            case SparkState.Grind:
                rightSpark.gameObject.SetActive(false);
                leftSpark.gameObject.SetActive(false);
                grindSpark.gameObject.SetActive(true);
                break;

        }
    }

    public void SetSparkColor(Color c)
    {
        leftSpark.color = Color.white; //flash of color
        rightSpark.color = Color.white;
        leftSpark.DOColor(c, 0.5f);
        rightSpark.DOColor(c, 0.5f);
        curColor = c;
    }
}
