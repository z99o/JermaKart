using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HopDirection
{
    Front,
    Back,
    Left,
    Right
}
public class HopMaker : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public Vector3 hopPos;
    public Vector3 backHopPos;
    public Transform hopTrans;
    public Transform sideHopTrans;
    

    public void PlayHop(HopDirection dir)
    {
        Vector3 scale = Vector3.zero;
        switch (dir)
        {
            case HopDirection.Front:
                hopTrans.localPosition = hopPos;
                scale = new Vector3(5, -5, 5);
                hopTrans.localScale = scale;
                animator.Play("Hop");
                break;
            case HopDirection.Back:
                hopTrans.localPosition = backHopPos;
                scale = new Vector3(5, 5, 5);
                hopTrans.localScale = scale;
                animator.Play("Hop");
                break;
            case HopDirection.Left:
                sideHopTrans.localPosition = new Vector3(2.35f, 0.66f, -1.7f);  
                scale = new Vector3(4, 4, 4);
                sideHopTrans.localScale = scale;
                animator.Play("SideHop");
                break;
            case HopDirection.Right:
                sideHopTrans.localPosition = new Vector3(-2.35f, 0.66f, -1.7f);
                scale = new Vector3(-4, 4, 4);
                sideHopTrans.localScale = scale;
                animator.Play("SideHop");
                break;
            default:
                break;
        }
    }
}
