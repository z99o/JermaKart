using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class RailCollider : MonoBehaviour
{
    [SerializeField]
    private RailScript parent;
    private Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }
    private void Start()
    {
    }

    public void SetParent(RailScript parent)
    {
        if (parent == null)
            return;
        this.parent = parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Racer")
        {
            Racer racer = other.transform.parent.GetComponent<Racer>();
            if (racer)
            {
                racer.AddScore(0.25f);
                parent.OnRailEnter(racer, collider.ClosestPoint(other.transform.position));
            }
        }
    }
}
