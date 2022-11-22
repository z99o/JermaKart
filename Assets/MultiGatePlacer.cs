using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Linq;


[ExecuteInEditMode]
public class MultiGatePlacer : GatePlacer
{
    // Start is called before the first frame update
    [Range(0, 200)]
    public int end_point;
    private Vector3 origin;
    // Update is called once per frame
    void Update()
    {

    }




    override public void GenerateColliders()
    {
        //we have several splines of differing sizes, we need to give them the appropriate gates
        //Some splines are alt paths
        //we only deal with one spline at a time
        //we specify the range of values
        if (start_point > end_point)
        {
            end_point = start_point + 1;
            return;
        }

        List<SplinePoint> points = spline.GetPoints().ToList();
        for (int i = 0; i < colliders.Count; i++)
        {
            StartCoroutine(Destroy(colliders[i].gameObject));
        }
        colliders.Clear();
        float percent = 0f;
        fidelity = end_point - start_point;
        float increment = (float)(1.0 / fidelity); //the number of 
        int curGate = offset;
        int num = start_point;
        if (reverse)
        {
            percent = 1.0f;
        }
        float step = 0.01f;
        if (reverse)
            step *= -1;
        Vector3 point1;
        Vector3 point2;
        while (num <= end_point && percent >= 0 && percent <= 1.0f)
        {
            point1 = spline.EvaluatePosition(percent);
            point2 = spline.EvaluatePosition(Mathf.Clamp01(percent + step));
            Vector3 midpoint = new Vector3((point1.x + point2.x) / 2,
                                (point1.y + point2.y) / 2,
                                (point1.z + point2.z) / 2
                                );
            float sizeZ = Vector3.Distance(point1, point2);
            SplineSample p = spline.Project(midpoint);
            if (num == start_point)
                origin = midpoint;
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
            
            if (reverse)
            {
                //num++;
                percent -= increment;
                if (percent == 0)
                    percent = 1;
            }
            else
            {
                //num--;
                percent += increment;
                if (percent == 1)
                    percent = 0;
            }
            num++;
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
    void OnDrawGizmos()
    {
        // Draws the Light bulb icon at position of the object.
        // Because we draw it inside OnDrawGizmos the icon is also pickable
        // in the scene view.
        Gizmos.DrawIcon(origin, "marker.png", true);
    }

}
