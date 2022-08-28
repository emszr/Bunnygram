using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    #region List

    public static void Shuffle<T>(this List<T> list, int seed)
    {
        var rng = new System.Random(seed);
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


    #endregion

    #region Vector3[]

    public static void GenerateCurve(this LineRenderer renderer, Vector3 startPosition, Vector3 midPoint,Vector3 endPosition,int numberOfPoints)
    {
        Vector3[] arr = new Vector3[numberOfPoints];
        Vector3 p0 = startPosition;
        Vector3 p1 = midPoint;
        Vector3 p2 = endPosition;
        float t;
        Vector3 position;
        for (int i = 0; i < numberOfPoints; i++)
        {
            t = i / (numberOfPoints - 1.0f);
            position = (1.0f - t) * (1.0f - t) * p0
            + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
            arr[i] = position;
        }
        renderer.SetPositions(arr);
    }
    #endregion
}

