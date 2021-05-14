﻿using UnityEngine;
using VRSketchingGeometry.BezierSurfaceTool.State;

namespace VRSketchingGeometry.BezierSurfaceTool.Strategy
{
    public class StrategyRotationAngle : DrawingCurveStrategy
    {
        BezierSurfaceTool.DrawingCurveStrategy DrawingCurveStrategy.GetCurrentStrategy()
        {
            return BezierSurfaceTool.DrawingCurveStrategy.RotationAngle;
        }
        
        Vector3 DrawingCurveStrategy.CalculateControlPoint(int i, BezierSurfaceToolStateData bezierSurfaceToolStateData)
        {
            switch (i)
            {
                case 1:
                case 3:
                    float angle = Quaternion.Angle(bezierSurfaceToolStateData.cpHandles[0].transform.rotation,
                        bezierSurfaceToolStateData.cpHandles[2].transform.rotation);
                    //Debug.Log("Angle: " + angle);
                    float t = angle / 180;
                    //Debug.Log("t: " + t);
                    Vector3 newCp = Vector3.Lerp(bezierSurfaceToolStateData.cpHandles[i].transform.position,
                        bezierSurfaceToolStateData.cpHandles[i-1].transform.position, t);
                    bezierSurfaceToolStateData.supplementaryCpHandles[(int)((i - 1) * 0.5)].transform.position = newCp;
                    return newCp;
                default:
                    return bezierSurfaceToolStateData.cpHandles[i].transform.position;
            }
        }
    }
}