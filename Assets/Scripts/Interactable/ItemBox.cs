using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ItemBox : MonoBehaviour
{
    [SerializeField]
    private Item default_item;
    [SerializeField]
    private SpriteRenderer default_sprite;
    [SerializeField]
    private Transform itemTransform;
    [SerializeField]
    private Transform questionTransform;
    [SerializeField]
    private Transform cube1;
    [SerializeField]
    private Transform cube2;
    [SerializeField]
    private Animator animator;

    private Vector3 risePOS;
    private bool rising = true;
    private Transform player;
    [SerializeField]
    private float riseAmount = 0.2f;
    [SerializeField]
    private float riseTime = 1f;
    [SerializeField]
    private float rotateSpeed = 1f;
    [SerializeField]
    private bool exploded;
    [SerializeField]
    private float reArmTime = 5f;
    [SerializeField]
    private float timer;
    [SerializeField]
    private float offset;
    private void Start()
    {
        risePOS = transform.position;
        risePOS.y += riseAmount;
        player = Camera.main.transform;
        rising = true;
    }

    private void ReArm()
    {
        animator.Play("Default");
        exploded = false;
        transform.GetChild(0).gameObject.SetActive(true);
    }
    private void Update()
    {
        Vector3 pos = transform.position;
        timer += 0.0001f; //timer causes issues
        offset = riseAmount * Mathf.Sin(Time.timeSinceLevelLoad / riseTime);
        pos.y = risePOS.y + offset;
        transform.position = pos;
        itemTransform.LookAt(player);
        questionTransform.LookAt(player);
        cube1.transform.Rotate(rotateSpeed*Time.deltaTime, 0, 0);
        cube2.transform.Rotate(0, -rotateSpeed * Time.deltaTime,0);
        if (exploded)
            transform.LookAt(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Racer" && !exploded)
        {
            Racer racer = other.transform.parent.GetComponent<Racer>();
            if (racer.CanPickupItem()) { 
                racer.GiveItem(default_item);
                animator.Play("boxExplode");
                exploded = true;
            }
        }
    }
    public void Event_Die()
    {
        //GameObject.Destroy(this.gameObject);
        Invoke("ReArm", reArmTime);
    }
}
