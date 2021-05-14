using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.BezierSurfaceTool.State;

namespace VRSketchingGeometry.BezierSurfaceTool.Strategy
{
    public class StrategyVectorAngle : DrawingCurveStrategy
    {
        BezierSurfaceTool.DrawingCurveStrategy DrawingCurveStrategy.GetCurrentStrategy()
        {
            return BezierSurfaceTool.DrawingCurveStrategy.VectorAngle;
        }

        Vector3 DrawingCurveStrategy.CalculateControlPoint(int i, BezierSurfaceToolStateData bezierSurfaceToolStateData)
        {
            switch (i)
            {
                case 1:
                case 3:
                    Vector3 leftUpVector = bezierSurfaceToolStateData.cpHandles[1].transform.position -
                                           bezierSurfaceToolStateData.cpHandles[0].transform.position;
                    Vector3 rightUpVector = bezierSurfaceToolStateData.cpHandles[3].transform.position -
                                            bezierSurfaceToolStateData.cpHandles[2].transform.position;
                    float angle = Vector3.Angle(leftUpVector, rightUpVector);
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