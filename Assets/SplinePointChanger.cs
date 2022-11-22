using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
public class SplinePointChanger : MonoBehaviour
{
    [SerializeField]
    public SplineComputer spline;

    public void SplineForward()
    {
        if (!spline)
            return;
        for (int i = 0; i < spline.pointCount; i++)
        {
            int next = (i + 1) % (spline.pointCount);
            spline.SetPoint(i, spline.GetPoints()[next]);
        }
    }

    public void SplineBackward()
    {
        if (!spline)
            return;
        SplinePoint point = new SplinePoint();
        for (int i = spline.pointCount-1; i > 1; i--)
        {
            point = spline.GetPoints()[i - 1];
            spline.SetPoint(i, point);
        }
        point = spline.GetPoints()[spline.pointCount - 1];
        spline.SetPoint(0, point);
    }
}
