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
        
        Vector3 IDrawingCurveStrategy.CalculateControlPoint(int i, BezierSurfaceToolStateData bezierSurfaceToolStateData)
        {
            switch (i)
            {
                case 1:
                case 3:
                    float refDistance = 1f;
                    float distance = Vector3.Distance(bezierSurfaceToolStateData.cpHandles[0].transform.position,
                        bezierSurfaceToolStateData.cpHandles[2].transform.position);
                    
                    //Debug.Log("refDistance: " + refDistance);
                    //Debug.Log("Distance: " + distance);
                    if (refDistance < 0.1f)
                    {
                        refDistance = 0.1f;
                    }
                    float t = (distance - 0.2f) / refDistance;
                    //Debug.Log("t: " + t);
                    Vector3 newCp = Vector3.Lerp(bezierSurfaceToolStateData.cpHandles[i-1].transform.position,
                        bezierSurfaceToolStateData.cpHandles[i].transform.position, t);
                    bezierSurfaceToolStateData.supplementaryCpHandles[(int)((i - 1) * 0.5)].transform.position = newCp;
                    return newCp;
                default:
                    return bezierSurfaceToolStateData.cpHandles[i].transform.position;
            }
        }
    }
}