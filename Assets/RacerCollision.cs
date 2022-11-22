using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerCollision : MonoBehaviour
{
    [SerializeField]
    private Racer parent;
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Wall")
        {
            parent.OnWallHit();
        }
        if(other.tag == "KillBox")
        {
            parent.Respawn();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "KillBox")
        {
            parent.Respawn();
        }
        if(other.tag == "BoostPad")
        {
            BoostPad pad = other.transform.GetComponent<BoostPad>();
            parent.OnBoostPad(pad.transform.rotation, pad.getForceDirectionDuration(), pad.getBoostStrength(), pad.getBoostDuration(),pad.getIsLaunchPad()); //rotation of the pad is what we want
        }
        if (other.tag == "BoostRing")
        {
            BoostPad pad = other.transform.parent.GetComponent<BoostPad>();
            parent.OnBoostPad(pad.transform.rotation, pad.getForceDirectionDuration(),pad.getBoostStrength(), pad.getBoostDuration(), pad.getIsLaunchPad()); //get the rotation of the ring itself
        }
        if (other.tag == "Racer")
            parent.OnWallHit();
        if(other.tag == "Hazard_Damage")
        {
            parent.ApplyStun(2f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Wall")
        {
            parent.OnWallHit();
        }
        if (collision.transform.tag == "Racer")
            parent.OnRacerHit();

        if (collision.transform.tag == "KillBox")
        {
            parent.Respawn();
        }
    }
}
