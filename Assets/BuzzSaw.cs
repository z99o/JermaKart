using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BuzzSaw : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform saw;
    public Rigidbody sawRB;
    public Transform rail;
    public float leftPos;
    public float rightPos;
    public bool startLeft;
    public bool monoDirectional = false;
    public float speed = 5f;
    public float waitTime = 0;
    public float startDelay = 0;
    public bool doSpin = true;

    public Ease easeMode;
    void Start()
    {
        //calculate the left and right pos
        leftPos = -rail.localScale.z / 2f;
        rightPos = rail.localScale.z / 2f;
        DOVirtual.DelayedCall(startDelay, () => {
            if (startLeft)
                GoLeft();
            else
                GoRight();
        });

    }

    private void GoRight()
    {
        saw.DOLocalMoveZ(rightPos, speed)
            .SetEase(easeMode)
            .SetUpdate(UpdateType.Fixed)
            .OnComplete(()=>  DOVirtual.DelayedCall(waitTime, () =>  Reset(false) ) );
    }
    private void GoLeft()
    {
        saw.DOLocalMoveZ(leftPos, speed)
            .SetEase(easeMode)
            .SetUpdate(UpdateType.Fixed)
            .OnComplete(() => DOVirtual.DelayedCall(waitTime, () => Reset(true)));
    }

    private void Reset(bool endedLeft)
    {
        if (monoDirectional)
        {
            Vector3 newPos = saw.transform.localPosition;
            if (startLeft)
            {
                newPos.z = leftPos;
                saw.transform.localPosition = newPos;
                GoRight();
            }
            else
            {
                newPos.z = rightPos;
                saw.transform.localPosition = newPos;
                GoLeft();
            }
        }
        else
        {
            if (endedLeft)
                GoRight();
            else
                GoLeft();
        }
    }

    private void Update()
    {
        float spinSpeed = Time.deltaTime * 1000;
        if(doSpin)
            saw.Rotate(Vector3.right, spinSpeed);
    }
}
