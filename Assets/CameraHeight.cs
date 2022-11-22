using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraHeight : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask layerMask;
    public float height = 10;
    public float adjustmentSpeed = 2f;
    public CinemachineVirtualCamera cam;
    private CinemachineTransposer transposer;
    private CinemachineComposer composer;
    public Racer_Player player;
    public float aimOffset = 1;
    public float threshhold = 1f;
    void Start()
    {
        transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
        composer = cam.GetCinemachineComponent<CinemachineComposer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.GetController().Is_Grounded())
            return;
        //update body pos
        RaycastHit groundHit;
        Physics.Raycast(transform.position, Vector3.down, out groundHit, height, layerMask);
        RaycastHit ceilingHit;
        Physics.Raycast(transform.position, Vector3.up, out ceilingHit, height, layerMask);
        Vector3 lerpTo = transposer.m_FollowOffset;
        if (groundHit.transform != null)
        {
            lerpTo.y = height - groundHit.distance;
        }
        else if (ceilingHit.transform != null)
        {
            lerpTo.y += adjustmentSpeed;
        }

        if (groundHit.transform != null && groundHit.distance < height)
            lerpTo.y += adjustmentSpeed;
        if (groundHit.transform != null && groundHit.distance > height)
            lerpTo.y -= adjustmentSpeed;


        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, lerpTo, Time.deltaTime*adjustmentSpeed);

        //update look pos
        Vector3 newOffset = player.GetController().GetNormal().forward * aimOffset;
        //composer.m_TrackedObjectOffset = newOffset;
    }
}
