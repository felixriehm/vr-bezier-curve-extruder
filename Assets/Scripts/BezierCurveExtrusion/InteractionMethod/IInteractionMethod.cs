using BezierCurveExtrusion.State;
using UnityEngine;

namespace BezierCurveExtrusion.InteractionMethod
{
    public interface IInteractionMethod
    {
        internal BezierCurveExtruder.InteractionMethod GetCurrentInteractionMethod();
        internal Vector3 CalculateControlPoint(int i, BezierCurveExtruderStateData bezierCurveExtruderStateData);
    }
}