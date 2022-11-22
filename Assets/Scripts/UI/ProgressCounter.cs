using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//sets the progress  of the racer that passed it, MAKE SURE IT DOESNT ACCIDENTALLY MAKE A RACER GO BACKWARDS IN PROGRESS
[RequireComponent(typeof(Collider))]
public class ProgressCounter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public Lap_Progress progress;
    [SerializeField]
    public int progress_num;
    [SerializeField]
    private List<Racer> passed = new List<Racer>();
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Racer")
        {
            Racer r = other.transform.parent.GetComponent<Racer>();
            r.SetLapPoint(progress_num);
            
        }
    }
}
