using BezierCurveExtrusion.State;
using UnityEngine;

namespace BezierCurveExtrusion.Strategy
{
    public class StrategyDistance : IDrawingCurveStrategy
    {
        BezierCurveExtruder.DrawingCurveStrategy IDrawingCurveStrategy.GetCurrentStrategy()
        {
            return BezierCurveExtruder.DrawingCurveStrategy.Distance;
        }
        
        Vector3 IDrawingCurveStrategy.CalculateControlPoint(int i, BezierCurveExtruderStateData bezierCurveExtruderStateData)
        {
            switch (i)
            {
                case 1:
                case 3:
                    float refDistance = 1f;
                    float distance = Vector3.Distance(bezierCurveExtruderStateData.cpHandles[0].transform.position,
                        bezierCurveExtruderStateData.cpHandles[2].transform.position);
                    
                    //Debug.Log("refDistance: " + refDistance);
                    //Debug.Log("Distance: " + distance);
                    if (refDistance < 0.1f)
                    {
                        refDistance = 0.1f;
                    }
                    float t = (distance - 0.2f) / refDistance;
                    //Debug.Log("t: " + t);
                    Vector3 newCp = Vector3.Lerp(bezierCurveExtruderStateData.cpHandles[i-1].transform.position,
                        bezierCurveExtruderStateData.cpHandles[i].transform.position, t);
                    bezierCurveExtruderStateData.supplementaryCpHandles[(int)((i - 1) * 0.5)].transform.position = newCp;
                    return newCp;
                default:
                    return bezierCurveExtruderStateData.cpHandles[i].transform.position;
            }
        }
    }
}