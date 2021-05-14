using UnityEngine;
using VRSketchingGeometry.BezierSurfaceTool.State;

namespace VRSketchingGeometry.BezierSurfaceTool.Strategy
{
    public interface DrawingCurveStrategy
    {
        internal BezierSurfaceTool.DrawingCurveStrategy GetCurrentStrategy();
        internal Vector3 CalculateControlPoint(int i, BezierSurfaceToolStateData bezierSurfaceToolStateData);
    }
}