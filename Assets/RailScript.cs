using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Linq;
[ExecuteInEditMode]
public class RailScript : MonoBehaviour
{
    // Start is called before the first frame update
    public bool updateColliders;
    
    [SerializeField]
    private SplineComputer spline;
    [SerializeField]
    private GameObject colliderObject;
    [SerializeField]
    private List<GameObject> colliders;
    [SerializeField]
    private BikeController player;
    [Range(1, 100)]
    public int fidelity = 1;
    void Initiate()
    {
        //player = Game_Manager._Instance.GetPlayer().GetController();
        GenerateColliders();
    }

    private void Awake()
    {
        GenerateColliders();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        updateColliders = true;
    }

    private void LateUpdate()
    {
        if(updateColliders)
        {
            updateColliders = false;
            GenerateColliders();
        }
    }

    public void Trigger_Intercept()
    {
        Debug.Log("Rail Crossed");
    }

    public void OnRailEnter(Racer rider,Vector3 position) {
        if (rider.GetController().GetMoveState() == MoveState.Grinding)
            return;
        //if (rider is Racer_AI)
        //    return;
        
        float percentage = (float)spline.Project(position).percent;
        //get the closest point on the rail
        //get the closest neighbor to the collision point
        //get their %s
        //get the % distance between A and B that our position is
        //A = 70%, B = 80%, C = 50%, therefore we're at position 75%
        //this limits how we can place points but it should be fine for the most part
        rider.GetController().OnRailEnter(this,percentage);
        //get enter position
    }

    public void GenerateColliders()
    {
        List<SplinePoint> points = spline.GetPoints().ToList();
        for(int i = 0; i < colliders.Count; i++)
        {
            StartCoroutine(Destroy(colliders[i].gameObject));
        }
        colliders.Clear();
        float percent = 0f;
        float increment = (float)(1.0 / fidelity);
        Vector3 point1;
        Vector3 point2;
        while (percent + increment <= 1f)
        {
            point1 = spline.EvaluatePosition(percent);
            point2 = spline.EvaluatePosition(percent + increment);
            Vector3 midpoint = new Vector3((point1.x + point2.x) / 2,
                                (point1.y + point2.y) / 2,
                                (point1.z + point2.z) / 2
                                );
            float sizeZ = Vector3.Distance(point1, point2);
            var cur = Instantiate(colliderObject, midpoint, transform.rotation, transform);
            Vector3 boxSize = cur.GetComponent<BoxCollider>().size;
            boxSize.z = sizeZ;
            cur.GetComponent<BoxCollider>().size = boxSize;
            cur.GetComponent<RailCollider>().SetParent(this);
            //last collider look at the current one
            cur.transform.LookAt(point1);
            colliders.Add(cur);
            percent += increment;
        }

        point1 = spline.EvaluatePosition(1f);
        point2 = spline.EvaluatePosition(1f - increment);
        Vector3 midpoint2 = new Vector3((point1.x + point2.x) / 2,
                    (point1.y + point2.y) / 2,
                    (point1.z + point2.z) / 2
                    );
        float sizeZ2 = Vector3.Distance(point1, point2);
        var cur2 = Instantiate(colliderObject, midpoint2, transform.rotation, transform);
        Vector3 boxSize2 = cur2.GetComponent<BoxCollider>().size;
        boxSize2.z = sizeZ2;
        cur2.GetComponent<BoxCollider>().size = boxSize2;
        cur2.GetComponent<RailCollider>().SetParent(this);
        //last collider look at the current one
        cur2.transform.LookAt(point1);
        colliders.Add(cur2);
    }

    public SplineComputer GetSpline()
    {
        return spline;
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(go);
    }

}
