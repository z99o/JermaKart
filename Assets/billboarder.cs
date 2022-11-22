using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class billboarder : MonoBehaviour
{
    // Start is called before the first frame update

    public bool billboardX = true;
    public bool billboardY = false;
    public bool billboardZ = true;
    public bool isAbove = false;
    public Vector3 lookPos;


    public GameObject mainCamera;

    void Start(){
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newLook = mainCamera.transform.position;
        Vector3 camLook = mainCamera.transform.position;
        Vector3 localPos = transform.position;
        if(!isAbove){

            if(!billboardX){
                newLook.x = localPos.x;
            }
            if(!billboardY){
                newLook.y = localPos.y;
            }
            if(!billboardZ){
                newLook.z = localPos.z;
            }
            transform.LookAt(newLook, Vector3.up);
        }
        else{
            transform.LookAt(camLook);
            Vector3 curRot = transform.localEulerAngles;
            curRot.x = -90;
            if(!billboardX){
                curRot.x = 0;
            }
            if(!billboardY){
                curRot.y = 0;
            }
            if(!billboardZ){
                curRot.z = 0;
            }
            Debug.Log(curRot);
            transform.localEulerAngles = curRot;
        }
    }
}
