using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CarasolMember : MonoBehaviour
{
    public int place;
    public CarasolController controller;
    public RectTransform rectTransform;
    public TMPro.TextMeshProUGUI textView;
    public Vector3 nextPos;
    public int nextPlace;
    public Image image;
    public bool canMove;
    private CarasolData data;

    public CarasolData GetData()
    {
        return data;
    }

    public void Initialize(CarasolController controller, int place)
    {
        this.place = place;
        this.controller = controller;
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        ChangeColor();
        ChangeSize();
        OnMoveDone();
    }
    public void Move(Vector3 newPos, int newPlace)
    {
        if (!canMove)
            return;
        float speed = controller.speed;
        place = newPlace;
        rectTransform.DOMove(newPos, speed).OnComplete(OnMoveDone);
        ChangeColor();
        ChangeSize();
        int numMembers = controller.members.Count - 1;
        if (Mathf.Abs(1 - (float)((float)place / (float)numMembers)) == 0.5f)
        {
            transform.SetAsLastSibling();//bring to front
        }
        canMove = false;
    }

    public void ChangeColor()
    {
        float speed = controller.speed;
        int numMembers = controller.members.Count - 1;
        Color curColor = image.color;
        if (place == 0 || place == numMembers)
        {
            curColor.a = 0;
        }
        else// 1 - 0.5 / 0.5 = 1  1 - .75 /0.5 = .5, 1 - .25 /0.5 = .75/.5
        {
            float distance = Mathf.Abs(1 - (float)((float)place / (float)numMembers));
            if (distance < 0.5f)
                curColor.a = distance / 0.5f;
            else
                curColor.a = 0.5f / distance;
            curColor.a += .5f;
        }
        if (curColor.a == 0)
        {
            curColor.a = 0.1f;
        }

        image.DOColor(curColor, speed / 2f);
    }
    public void ChangeSize()
    {

        float speed = controller.speed;
        int numMembers = controller.members.Count - 1;
        Vector3 newScale = rectTransform.localScale;
        float distance = Mathf.Abs(1 - (float)((float)place / (float)numMembers));
        float scaleVal = 1;
        if (distance < 0.5f)
            scaleVal = distance / 0.5f;
        else
            scaleVal = 0.5f / distance;
        if(scaleVal == 0)
        {
            scaleVal = 0.25f;
        }
        newScale = new Vector3(scaleVal, scaleVal, scaleVal);
        rectTransform.DOScale(newScale, speed);
    }

    public void AssignData(CarasolData data)
    {
        this.data = data;
        textView.text = this.data.data_name;
        image.sprite = data.icon;
    }

    public void OnMoveDone()
    {
        canMove = true;
    }
}
