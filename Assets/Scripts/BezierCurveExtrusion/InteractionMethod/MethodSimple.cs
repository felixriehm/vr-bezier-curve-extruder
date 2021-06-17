using System.Collections.Generic;
using BezierCurveExtrusion.State;
using UnityEngine;

namespace BezierCurveExtrusion.InteractionMethod
{
    public class MethodSimple : IInteractionMethod
    {
        BezierCurveExtruder.InteractionMethod IInteractionMethod.GetCurrentInteractionMethod()
        {
            return BezierCurveExtruder.InteractionMethod.Simple;
        }
        
        Vector3 IInteractionMethod.CalculateControlPoint(int i, BezierCurveExtruderStateData bezierCurveExtruderStateData)
        {
            return bezierCurveExtruderStateData.cpHandles[i].transform.position;
        }
    }
}