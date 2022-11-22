using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BillboardAngle
{
    Above,
    Below,
    Front,
    Back,
    Left,
    Right
}
public static class BillboardHelper{

    private static float aboveThreshold = 45;
    private static float frontThreshold = 45;
    private static float belowThreshold = 45;
    private static float leftThreshold = 45;
    private static float rightThreshold = 45;
    private static float backThreshold = 45;

    public static bool AngleIsAbove(Transform obj, Transform cam){
        //is +90 degrees above +- abovethreshold
        //is 0 degrees to the side +- abovethreshold
        Vector3 dir_vec = cam.position - obj.position;
        Vector3 n_obj = obj.position;
        Vector3 n_cam = cam.position;
        Vector2 YZ_obj = new Vector2(obj.transform.position.y,obj.transform.position.z).normalized;
        Vector2 YZ_cam = new Vector2(cam.transform.position.y,cam.transform.position.z).normalized;
        Vector2 XZ_obj = new Vector2(obj.transform.position.x,obj.transform.position.z).normalized;
        Vector2 XZ_cam = new Vector2(cam.transform.position.x,cam.transform.position.z).normalized;
        float yzangle = Vector2.SignedAngle(YZ_obj,YZ_cam);
        float xzangle = Vector2.SignedAngle(XZ_obj,XZ_cam);
        float ocangle = Vector3.Angle(dir_vec,Vector3.up);

        if(ocangle > (0 - aboveThreshold) && 
           ocangle < (0 + aboveThreshold)){
                    return true;
        }
        return false;
    }

    public static bool AngleIsBehind(Transform obj, Transform cam){
        //is +90 degrees above +- abovethreshold
        //is 0 degrees to the side +- abovethreshold
        Vector3 dir_vec = cam.position - obj.position;
        float ocangle = Vector3.Angle(dir_vec,obj.forward);

        if(ocangle > (180 - backThreshold) && 
           ocangle < (180 + backThreshold)){
                    return true;
        }
        return false;
    }
    public static bool AngleIsFront(Transform obj, Transform cam){
        //is +90 degrees above +- abovethreshold
        //is 0 degrees to the side +- abovethreshold
        Vector3 dir_vec = cam.position - obj.position;
        float ocangle = Vector3.Angle(dir_vec,obj.forward);

        if(ocangle > (0 - frontThreshold) && 
           ocangle < (0 + frontThreshold)){
                    return true;
        }
        return false;
    }
    public static bool AngleIsLeft(Transform obj, Transform cam){
        //is +90 degrees above +- abovethreshold
        //is 0 degrees to the side +- abovethreshold
        Vector3 dir_vec = cam.position - obj.position;
        float ocangle = Vector3.Angle(dir_vec,obj.right);

        if(ocangle > (180 - leftThreshold) && 
           ocangle < (180 + leftThreshold)){
                    return true;
        }
        return false;
    }
    public static bool AngleIsRight(Transform obj, Transform cam){
        //is +90 degrees above +- abovethreshold
        //is 0 degrees to the side +- abovethreshold
        Vector3 dir_vec = cam.position - obj.position;
        float ocangle = Vector3.Angle(dir_vec,obj.right);
        //Debug.Log(ocangle);

        if(ocangle > (0 - rightThreshold) && 
           ocangle < (0 + rightThreshold)){
                    return true;
        }
        return false;
    }
    public static bool AngleIsBelow(Transform obj, Transform cam){
        //is +90 degrees above +- abovethreshold
        //is 0 degrees to the side +- abovethreshold
        Vector3 dir_vec = cam.position - obj.position;
        float ocangle = Vector3.Angle(dir_vec,obj.forward);

        if(ocangle > (180 - belowThreshold) && 
           ocangle < (180 + belowThreshold)){
                    return true;
        }
        return false;
    }
}