using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

[System.Serializable]
public struct CarasolData
{
    public string data_name;
    public bool selectable;
    public Sprite icon;
}
public class CarasolController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public List<CarasolMember> members;
    [SerializeField]
    private float distance = 200f;
    public float speed = 1f;
    [SerializeField]
    public List<CarasolData> data = new List<CarasolData>();
    [SerializeField]
    int rightData;
    [SerializeField]
    int leftData;
    [SerializeField]
    AudioManager audioManager;


    public CarasolMember GetCurrent()
    {
        return members.Where(a => a.place == (int)((members.Count - 1) / 2)).FirstOrDefault();
    }
    void Awake()
    {
        for(int i = 0; i < members.Count; i++)
        {
            members[i].Initialize(this,i);
        }
        for(int i = 0; i < members.Count; i++)
        {
            rightData = i % data.Count;
            int j = ((int)((members.Count-1)/2) + i) % members.Count;
            members[j].AssignData(data[rightData]);
        }

        rightData = (rightData + 1) % data.Count;
        leftData = rightData;
    }

    // Update is called once per frame
    void Update()
    {
     
        
    }

    public CarasolMember GetSelectedMember()
    {
        return transform.GetChild(2).GetComponent<CarasolMember>(); //1 is in the center;
    }

    public void Move_Right()
    {
        //lerp everything right 
        audioManager.PlayManagerClip("button_press");
        for (int i = 0; i < members.Count; i++) {
            if (!members[i].canMove)
                return;
            if (members[i].place == members.Count - 1)
            {
                members[i].nextPos = members.FirstOrDefault(a => a.place == 0).rectTransform.position;
                members[i].nextPlace = 0;
                //assign data
                if (data.Count > members.Count)
                {
                    members[i].AssignData(data[rightData]);
                    rightData = (rightData + 1) % data.Count;
                    leftData = (leftData + 1) % data.Count;
                }
            }
            else
            {
                members[i].nextPos = members.FirstOrDefault(a => a.place == members[i].place+1).rectTransform.position;
                members[i].nextPlace = members.FirstOrDefault(a => a.place == members[i].place + 1).place;
            }
        }

        for (int i = 0; i < members.Count; i++)
        {
                members[i].Move(members[i].nextPos, members[i].nextPlace);
        }
    }

    public void Move_Left()
    {
        audioManager.PlayManagerClip("button_press");
        //lerp everything right 
        for (int i = 0; i < members.Count; i++)
        {
            if (!members[i].canMove)
                return;
            if (members[i].place == 0)
            {
                members[i].nextPos = members.FirstOrDefault(a => a.place == members.Count - 1).rectTransform.position;
                members[i].nextPlace = members.Count - 1;
                if (data.Count > members.Count)
                {
                    members[i].AssignData(data[leftData]);
                    leftData = (leftData - 1) % data.Count;
                    rightData = (rightData - 1) % data.Count;
                    if (leftData < 0)
                        leftData = data.Count - 1;
                    if (rightData < 0)
                        rightData = data.Count - 1;
                }
            }
            else
            {
                members[i].nextPos = members.FirstOrDefault(a => a.place == members[i].place-1).rectTransform.position;
                members[i].nextPlace = members.FirstOrDefault(a => a.place == members[i].place - 1).place;
            }
        }

        for (int i = 0; i < members.Count; i++)
        {
            members[i].Move(members[i].nextPos, members[i].nextPlace);
        }
    }

}
