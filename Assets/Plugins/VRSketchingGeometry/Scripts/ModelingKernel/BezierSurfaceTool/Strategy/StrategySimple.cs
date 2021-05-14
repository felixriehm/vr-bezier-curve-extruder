using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.BezierSurfaceTool.State;

namespace VRSketchingGeometry.BezierSurfaceTool.Strategy
{
    public class StrategySimple : DrawingCurveStrategy
    {
        BezierSurfaceTool.DrawingCurveStrategy DrawingCurveStrategy.GetCurrentStrategy()
        {
            return BezierSurfaceTool.DrawingCurveStrategy.Simple;
        }
        
        Vector3 DrawingCurveStrategy.CalculateControlPoint(int i, BezierSurfaceToolStateData bezierSurfaceToolStateData)
        {
            return bezierSurfaceToolStateData.cpHandles[i].transform.position;
        }
    }
}