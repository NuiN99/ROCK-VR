using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeVisual : MonoBehaviour
{
    [SerializeField] LineRenderer lr;
    [SerializeField] int resolution = 10;

    [NonSerialized] public bool draw;
    
    Vector3 _startPoint;
    Vector3 _endPoint;
    float _distance;
    
    void LateUpdate()
    {
        lr.enabled = draw;

        if (!draw)
        {
            return;
        }
        
        lr.positionCount = resolution;
        DrawCurve();
    }

    public void SetDistance(float dist)
    {
        _distance = dist;
    }

    public void SetPositions(Vector3 start, Vector3 end)
    {
        _startPoint = start;
        _endPoint = end;
    }

    void DrawCurve()
    {
        Vector3 controlPoint = (_startPoint + _endPoint) / 2f;

        // Calculate the distance between start and end points
        float currentDistance = Vector3.Distance(_startPoint, _endPoint);

        // Calculate the sag factor based on the difference between current distance and desired distance
        float sagFactor = Mathf.Clamp01((_distance - currentDistance) / _distance);

        // Apply sag by moving the control point vertically
        controlPoint += Vector3.up * -_distance * sagFactor;

        var points = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            points[i] = CalculateBezierPoint(t, _startPoint, controlPoint, _endPoint);
        }
        lr.SetPositions(points);
    }

    static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}
