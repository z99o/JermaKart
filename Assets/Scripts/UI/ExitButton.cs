using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ExitButton : MonoBehaviour 
{
    // Start is called before the first frame update

    public Vector3 focusPos;
    public Vector3 unfocusPos;
    public Button exitButton;
    public RectTransform rectTransform;
    
    public void OnPointerEnter()
    {
        rectTransform.DOAnchorPos(focusPos, .25f).OnComplete(OnFocus);
    }

    public void OnPointerExit()
    {
        exitButton.interactable = false;
        rectTransform.DOAnchorPos(unfocusPos, .25f);
    }

    private void OnFocus()
    {
        exitButton.interactable = true;
    }
}
