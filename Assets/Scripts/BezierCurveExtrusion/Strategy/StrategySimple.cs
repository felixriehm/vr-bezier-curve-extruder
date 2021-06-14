using System.Collections.Generic;
using BezierCurveExtrusion.State;
using UnityEngine;

namespace BezierCurveExtrusion.Strategy
{
    public class StrategySimple : IDrawingCurveStrategy
    {
        BezierCurveExtruder.DrawingCurveStrategy IDrawingCurveStrategy.GetCurrentStrategy()
        {
            return BezierCurveExtruder.DrawingCurveStrategy.Simple;
        }
        
        Vector3 IDrawingCurveStrategy.CalculateControlPoint(int i, BezierSurfaceToolStateData bezierSurfaceToolStateData)
        {
            return bezierSurfaceToolStateData.cpHandles[i].transform.position;
        }
    }
}