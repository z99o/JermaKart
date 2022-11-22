using System.Collections;
using UnityEngine;
using Dreamteck.Splines;
using System.Linq;
using System.Collections.Generic;
using System;
public static class ExtensionMethods
{

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


    public static Vector3 MidPoint(this Vector3 p1, Vector3 p2)
    {
        return new Vector3((p1.x + p2.x) / 2,
                            (p1.y + p2.y) / 2,
                            (p1.z + p2.z) / 2
                            );
    }

    public static SplinePoint FindClosest(this List<SplinePoint> points, Vector3 target)
    {
        SplinePoint closestPoint = points[0];
        float closestDistance = Mathf.Infinity;
        for(int i = 0; i < points.Count(); i++)
        {
            float distance = Vector3.Distance(points[i].position, target);
            if (distance < closestDistance)
            {
                closestPoint = points[i];
                closestDistance = distance;
            }
        }
        return closestPoint;
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}