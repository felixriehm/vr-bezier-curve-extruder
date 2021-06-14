using BezierCurveExtrusion.State;
using UnityEngine;

namespace BezierCurveExtrusion.Strategy
{
    public interface IDrawingCurveStrategy
    {
        internal BezierCurveExtruder.DrawingCurveStrategy GetCurrentStrategy();
        internal Vector3 CalculateControlPoint(int i, BezierCurveExtruderStateData bezierCurveExtruderStateData);
    }
}