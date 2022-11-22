using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIViewController : MonoBehaviour
{
    // Start is called before the first frame update
    public AI_BikeController controller;
    [Header("Front Sprites")]
    [SerializeField]
    private Sprite Front_Normal;
    [SerializeField]
    private Sprite Front_Turn_Left;
    [SerializeField]
    private Sprite Front_Turn_Right;
    [SerializeField]
    private Sprite Front_Drift_Right;
    [SerializeField]
    private Sprite Front_Drift_Left;
    [SerializeField]
    private Sprite Front_Hard_Drift_Right;
    [SerializeField]
    private Sprite Front_Hard_Drift_Left;

    [Header("Back Sprites")]
    [SerializeField]
    private Sprite Back_Normal;
    [SerializeField]
    private Sprite Back_Turn_Left;
    [SerializeField]
    private Sprite Back_Turn_Right;
    [SerializeField]
    private Sprite Back_Drift_Right;
    [SerializeField]
    private Sprite Back_Drift_Left;
    [SerializeField]
    private Sprite Back_Hard_Drift_Right;
    [SerializeField]
    private Sprite Back_Hard_Drift_Left;

    [Header("West Sprites")]
    [SerializeField]
    private Sprite West_Normal;
    [SerializeField]
    private Sprite West_Turn_Left;
    [SerializeField]
    private Sprite West_Turn_Right;
    [SerializeField]
    private Sprite West_Drift_Right;
    [SerializeField]
    private Sprite West_Drift_Left;
    [SerializeField]
    private Sprite West_Hard_Drift_Right;
    [SerializeField]
    private Sprite West_Hard_Drift_Left;

    [Header("East Sprites")]
    [SerializeField]
    private Sprite East_Normal;
    [SerializeField]
    private Sprite East_Turn_Left;
    [SerializeField]
    private Sprite East_Turn_Right;
    [SerializeField]
    private Sprite East_Drift_Right;
    [SerializeField]
    private Sprite East_Drift_Left;
    [SerializeField]
    private Sprite East_Hard_Drift_Right;
    [SerializeField]
    private Sprite East_Hard_Drift_Left;

    private Transform mainCam;
    [SerializeField]
    private Transform parent;
    private SpriteRenderer sprite;
    public BillboardAngle angle;

    void Start()
    {
        mainCam = Camera.main.transform;
        sprite = GetComponent<SpriteRenderer>();
        parent = transform.parent.parent.parent.parent;
    }

    // Update is called once per frame
    void Update()
    {
        DriftState driftState = controller.GetDriftState();
        int driftDirection = controller.driftDirection;
        int turnDirection = controller.GetMoveDirection();
        if (mainCam == null)
            mainCam = Camera.main.transform;
        if (parent == null)
            parent = transform.parent.parent.parent.parent;


        if (BillboardHelper.AngleIsBehind(parent, mainCam))
        {
            angle = BillboardAngle.Back;
        }
        else
        {
            angle = BillboardAngle.Front;
        }

        //if(lastState = state && lastDir == dir)
        //return;
        switch (angle)
        {
            case BillboardAngle.Back:
                if (driftState < DriftState.Zero)
                {
                    sprite.sprite = Back_Normal;
                    if (turnDirection > 0)
                    {
                        sprite.sprite = Back_Turn_Left;
                    }
                    else if (turnDirection < 0)
                    {
                        sprite.sprite = Back_Turn_Right;
                    }
                }
                else
                {
                    if (driftDirection > 0)
                    {
                        sprite.sprite = Back_Drift_Left;
                    }
                    else
                    {
                        sprite.sprite = Back_Drift_Right;
                    }
                }
                break;
            case BillboardAngle.Front:
                if (driftState < DriftState.Zero)
                {
                    sprite.sprite = Front_Normal;
                    if (turnDirection > 0)
                    {
                        sprite.sprite = Front_Turn_Left;
                    }
                    else if (turnDirection < 0)
                    {
                        sprite.sprite = Front_Turn_Right;
                    }
                }
                else
                {
                    if (driftDirection > 0)
                    {
                        sprite.sprite = Front_Drift_Left;
                    }
                    else
                    {
                        sprite.sprite = Front_Drift_Right;
                    }
                }
                break;
        }
    }
}
