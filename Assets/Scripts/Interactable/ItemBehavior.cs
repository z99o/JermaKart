using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gets a scriptable object for the item given, and then plays the update loop that is relevant
public class ItemBehavior : MonoBehaviour
{

    private Animator animator;
    private Sprite sprite;
    private Transform target;
    private Transform user;
    private Item item;
    private bool initated;
    void Initiate(Item item, Transform user, Transform target = null)
    {
        this.user = user;
        this.target = target;
        this.item = item;
        initated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (initated) { 
            switch (item.itemName)
            {
                case "shell":
                    ShellUpdate();
                    break;
                case "mushroom":
                    MushroomUpdate();
                    break;
                default:
                    Debug.LogError("Unknown item" + item.itemName);
                    break;
            }
        }
    }


    void ShellUpdate()
    {

    }

    void MushroomUpdate()
    {

    }
}
