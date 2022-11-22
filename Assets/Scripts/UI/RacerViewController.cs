using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RacerViewController : MonoBehaviour
{
    // Start is called before the first frame update
    public BikeController controller;
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
    [SerializeField]
    private Sprite Front_Grind_Right;
    [SerializeField]
    private Sprite Front_Grind_Left;


    [Header("Back Sprites")]
    [SerializeField]
    private Sprite Back_Normal;
    [SerializeField]
    private Sprite Back_Alt;
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
    [SerializeField]
    private Sprite Back_Grind_Right;
    [SerializeField]
    private Sprite Back_Grind_Left;

    [Header("Left Sprites")]
    [SerializeField]
    private Sprite Left_Normal;
    [SerializeField]
    private Sprite Left_Alt;
    [SerializeField]
    private Sprite Left_Turn_Left;
    [SerializeField]
    private Sprite Left_Turn_Right;
    [SerializeField]
    private Sprite Left_Drift_Right;
    [SerializeField]
    private Sprite Left_Drift_Left;
    [SerializeField]
    private Sprite Left_Hard_Drift_Right;
    [SerializeField]
    private Sprite Left_Hard_Drift_Left;

    [Header("Right Sprites")]
    [SerializeField]
    private Sprite Right_Normal;
    [SerializeField]
    private Sprite Right_Alt;
    [SerializeField]
    private Sprite Right_Turn_Left;
    [SerializeField]
    private Sprite Right_Turn_Right;
    [SerializeField]
    private Sprite Right_Drift_Right;
    [SerializeField]
    private Sprite Right_Drift_Left;
    [SerializeField]
    private Sprite Right_Hard_Drift_Right;
    [SerializeField]
    private Sprite Right_Hard_Drift_Left;
    [SerializeField]
    private Sprite Error;

    private Transform mainCam;
    [SerializeField]
    private Transform parent;
    private SpriteRenderer sprite;
    public BillboardAngle angle;

    void Start()
    {
        mainCam = Camera.main.transform;
        sprite = GetComponent<SpriteRenderer>();
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            MoveState moveState = controller.GetMoveState();
            DriftState driftState = controller.GetDriftState();
            int driftDirection = controller.driftDirection;
            bool grindDirection = controller.GetGrindDirection();
            int turnDirection = controller.GetMoveDirection();
            if (mainCam == null)
                mainCam = Camera.main.transform;
            if (parent == null)
                parent = transform.parent.parent.parent.parent;


            if (BillboardHelper.AngleIsBehind(parent, mainCam))
            {
                angle = BillboardAngle.Back;
            }
            else if((BillboardHelper.AngleIsFront(parent, mainCam)))
            {
                angle = BillboardAngle.Front;
            }
            else if((BillboardHelper.AngleIsRight(parent, mainCam)))
            {
                angle = BillboardAngle.Right;
            }
            else if ((BillboardHelper.AngleIsLeft(parent, mainCam)))
            {
                angle = BillboardAngle.Left;
            }
            //if(lastState = state && lastDir == dir)
            //return;
            switch (angle)
            {
                case BillboardAngle.Back:
                    if (moveState == MoveState.Grinding)
                    {
                        if (grindDirection)
                        {
                            sprite.sprite = Back_Grind_Left;
                        }
                        else
                        {
                            sprite.sprite = Back_Grind_Right;
                        }
                    }
                    else if (driftState < DriftState.Zero)
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
                    if (moveState == MoveState.Grinding)
                    {
                        if (grindDirection)
                        {
                            sprite.sprite = Front_Grind_Left;
                        }
                        else
                        {
                            sprite.sprite = Front_Grind_Right;
                        }
                    }
                    else if (driftState < DriftState.Zero)
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
                case BillboardAngle.Left:
                    sprite.sprite = Left_Normal;
                    break;
                case BillboardAngle.Right:
                    sprite.sprite = Right_Normal;
                    break;
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
            sprite.sprite = Error;
        }
    }

}
