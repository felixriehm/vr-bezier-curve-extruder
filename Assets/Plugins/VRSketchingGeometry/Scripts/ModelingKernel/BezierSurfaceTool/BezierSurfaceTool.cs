using System;
using System.Collections.Generic;
using UnityEditorInternal;
using VRSketchingGeometry.SketchObjectManagement;
using UnityEngine;
using UnityEngine.Events;
using VRSketchingGeometry.BezierSurfaceTool.State;
using VRSketchingGeometry.BezierSurfaceTool.Strategy;

namespace VRSketchingGeometry.BezierSurfaceTool
{
    // TODO: wie BezierSurfaces speichern und löschen
    public class BezierSurfaceTool : MonoBehaviour
    {
        public BezierSurfaceToolSettings BezierSurfaceToolSettings;
        
        internal State.BezierSurfaceToolState CurrentBezierSurfaceToolState { get; set; }
        
        public enum BezierSurfaceToolState
        {
            ToolNotStarted,
            DrawingCurve,
            DrawingSurface,
        }
        
        public enum BezierSurfaceToolController
        {
            Left,
            Right
        }
        
        public enum DrawingCurveStrategy
        {
            Simple,
            VectorAngle,
            RotationAngle,
            Distance
        }

        private void Awake()
        {
            CurrentBezierSurfaceToolState = new StateToolNotStarted(this, BezierSurfaceToolSettings, new BezierSurfaceToolStateData());
        }

        public void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f)
        {
            CurrentBezierSurfaceToolState.StartTool(leftControllerOrigin, rightControllerOrigin, steps ,diameter);
        }

        private void Update()
        {
            CurrentBezierSurfaceToolState.Update();
        }

        public void ExitTool()
        {
            CurrentBezierSurfaceToolState.ExitTool();
        }

        public void ShowIndicators(bool show)
        {
            CurrentBezierSurfaceToolState.ShowIndicators(show);
        }
        
        public void StopDrawSurface()
        {
            CurrentBezierSurfaceToolState.StopDrawingSurface();
        }
        
        public void StartDrawSurface()
        {
            CurrentBezierSurfaceToolState.StartDrawingSurface();
        }

        public void ChangeCurveIntensity(BezierSurfaceToolController controller, float amount)
        { 
            CurrentBezierSurfaceToolState.ChangeCurveIntensity(controller, amount);
        }

        public void SetDrawingCurveStrategy(DrawingCurveStrategy strategy)
        {
            CurrentBezierSurfaceToolState.SetDrawingCurveStrategy(strategy);
        }
        
        public BezierSurfaceToolState GetCurrentState()
        {
            return CurrentBezierSurfaceToolState.GetCurrentState();
        }
        
        public UnityEvent<BezierSurfaceToolState> GetOnStateChangedEvent()
        {
            return CurrentBezierSurfaceToolState.GetOnStateChangedEvent();
        }
        
        public UnityEvent<DrawingCurveStrategy> GetOnStrategyChangedEvent()
        {
            return CurrentBezierSurfaceToolState.GetOnStrategyChangedEvent();
        }
        
        public DrawingCurveStrategy GetCurrentDrawingCurveStrategy()
        {
            return CurrentBezierSurfaceToolState.GetCurrentDrawingCurveStrategy();
        }
    }
}