using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
public class TransitionManager : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform sweep_right;
    public RectTransform sweep_left;
    public float transitionSpeed = 0.65f;
    private void Awake()
    {
        sweep_left.gameObject.SetActive(true);
        sweep_right.gameObject.SetActive(true);
    }
    public void SweepRight(Action callback)
    {
        //do transition in dotween
        sweep_right.DOAnchorPos(Vector3.zero, transitionSpeed).SetEase(Ease.Linear).OnComplete(() => callback());

    }

    public void SweepLeft(Action callback)
    {
        //do transition in dotween
        sweep_left.DOAnchorPos(Vector3.zero, transitionSpeed).SetEase(Ease.Linear).OnComplete(() => callback());

    }

    public void EnterSceneTransition(Action callback = null)
    {
        //sweep_right.anchoredPosition = Vector3.zero;
        if (callback == null)
            sweep_left.DOAnchorPos(new Vector3(1280, 0, 0), transitionSpeed).SetEase(Ease.Linear);
        else
        {
            sweep_left.DOAnchorPos(new Vector3(1280, 0, 0), transitionSpeed).SetEase(Ease.Linear).OnComplete(() => callback());
        }
    }
}
