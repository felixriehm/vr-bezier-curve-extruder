using System.Collections.Generic;
using BezierCurveExtrusion.State;
using UnityEngine;

namespace BezierCurveExtrusion.InteractionMethod
{
    public class MethodVectorAngle : IInteractionMethod
    {
        BezierCurveExtruder.InteractionMethod IInteractionMethod.GetCurrentInteractionMethod()
        {
            return BezierCurveExtruder.InteractionMethod.VectorAngle;
        }

        Vector3 IInteractionMethod.CalculateControlPoint(int i, BezierCurveExtruderStateData bezierCurveExtruderStateData)
        {
            switch (i)
            {
                case 1:
                case 3:
                    Vector3 leftUpVector = bezierCurveExtruderStateData.cpHandles[1].transform.position -
                                           bezierCurveExtruderStateData.cpHandles[0].transform.position;
                    Vector3 rightUpVector = bezierCurveExtruderStateData.cpHandles[3].transform.position -
                                            bezierCurveExtruderStateData.cpHandles[2].transform.position;
                    float angle = Vector3.Angle(leftUpVector, rightUpVector);
                    //Debug.Log("Angle: " + angle);
                    float t = angle / 180;
                    //Debug.Log("t: " + t);
                    Vector3 newCp = Vector3.Lerp(bezierCurveExtruderStateData.cpHandles[i].transform.position,
                        bezierCurveExtruderStateData.cpHandles[i-1].transform.position, t);
                    bezierCurveExtruderStateData.supplementaryCpHandles[(int)((i - 1) * 0.5)].transform.position = newCp;
                    return newCp;
                default:
                    return bezierCurveExtruderStateData.cpHandles[i].transform.position;
            }
        }
    }
}