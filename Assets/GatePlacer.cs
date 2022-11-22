using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Linq;

[ExecuteInEditMode]
public class GatePlacer : MonoBehaviour
{
    // Start is called before the first frame update
    public bool updateColliders;
    public bool dynamicUpdate;

    [SerializeField]
    protected SplineComputer spline;
    [SerializeField]
    protected GameObject colliderObject;
    [SerializeField]
    protected List<GameObject> colliders;
    [SerializeField]
    protected BikeController player;
    [Range(1, 200)]
    public int fidelity = 1;
    [Range(0, 200)]
    public int start_point;
    [Range(25, 500)]
    public float width = 25;
    [Range(1, 25)]
    public float height = 1;
    public bool reverse = false;
    [Range(0, 200)]
    public int offset;
    // Update is called once per frame
    void Update()
    {

    }

    protected void OnValidate()
    {
        updateColliders = true;
    }

    protected void LateUpdate()
    {
        if (updateColliders && dynamicUpdate)
        {
            updateColliders = false;
            GenerateColliders();
        }
    }

    public void Trigger_Intercept()
    {
        Debug.Log("Gate Crossed");
    }

    virtual public void GenerateColliders()
    {
        List<SplinePoint> points = spline.GetPoints().ToList();
        for (int i = 0; i < colliders.Count; i++)
        {
            StartCoroutine(Destroy(colliders[i].gameObject));
        }
        colliders.Clear();
        float percent = 0f;
        float increment = (float)(1.0 / fidelity);
        int num = start_point;
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
            SplineSample p = spline.Project(midpoint);
            var cur = Instantiate(colliderObject, midpoint, p.rotation, transform);
            Vector3 boxSize = cur.GetComponent<BoxCollider>().size;
            boxSize.x = p.size * width;
            boxSize.y = p.size * height;
            cur.GetComponent<BoxCollider>().size = boxSize;
            cur.GetComponent<ProgressCounter>().transform.SetParent(this.transform);
            cur.GetComponent<ProgressCounter>().progress_num = num;
            //last collider look at the current one
            cur.transform.LookAt(point1);
            colliders.Add(cur);
            percent += increment;
            if (reverse)
            {
                num++;
                if (num == fidelity)
                    num = 0;
            }
            else
            {
                num--;
                if (num < 0)
                    num = fidelity - 1;
            }
        }

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
