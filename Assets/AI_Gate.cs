using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Gate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    float score = 10;
    [SerializeField]
    private List<Racer> passed = new List<Racer>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Racer")
        {
            Racer r = other.transform.parent.GetComponent<Racer>();
            r.AddScore(score);

        }
    }
}
