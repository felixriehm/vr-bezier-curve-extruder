using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSketchingGeometry.BezierSurfaceTool;

public class UI : MonoBehaviour
{
    [SerializeField]
    private Text stateValue;
    [SerializeField]
    private Text strategyValue;
    [SerializeField]
    private Text runtimeValue;
    
    private int timeSinceStartup;

    private void Update()
    {
        timeSinceStartup = (int) Time.realtimeSinceStartup;
        runtimeValue.text = timeSinceStartup/60 + ":" + (timeSinceStartup % 60).ToString("00") + " minutes";
    }

    public void ChangeState(BezierSurfaceTool.BezierSurfaceToolState state)
    {
        switch (state)
        {
            case BezierSurfaceTool.BezierSurfaceToolState.ToolNotStarted:
                stateValue.text = "Tool not started";
                break;
            case BezierSurfaceTool.BezierSurfaceToolState.DrawingCurve:
                stateValue.text = "Drawing curve";
                break;
            case BezierSurfaceTool.BezierSurfaceToolState.DrawingSurface:
                stateValue.text = "Drawing surface";
                break;
            default:
                stateValue.text = "not found";
                break;
        }
    }
    
    public void ChangeStrategy(BezierSurfaceTool.DrawingCurveStrategy strategy)
    {
        switch (strategy)
        {
            case BezierSurfaceTool.DrawingCurveStrategy.Simple:
                strategyValue.text = "Variant A";
                break;
            case BezierSurfaceTool.DrawingCurveStrategy.VectorAngle:
                strategyValue.text = "Variant B";
                break;
            case BezierSurfaceTool.DrawingCurveStrategy.RotationAngle:
                strategyValue.text = "Variant C";
                break;
            case BezierSurfaceTool.DrawingCurveStrategy.Distance:
                strategyValue.text = "Variant D";
                break;
            default:
                strategyValue.text = "not found";
                break;
        }
    }
}
