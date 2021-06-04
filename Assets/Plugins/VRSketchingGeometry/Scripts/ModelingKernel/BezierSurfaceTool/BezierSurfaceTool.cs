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
    public class BezierSurfaceTool : MonoBehaviour
    {
        public BezierSurfaceToolSettings BezierSurfaceToolSettings;
        private SketchWorld sketchWorld;
        
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

        public void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f, BezierSurfaceTool.DrawingCurveStrategy drawingCurveStrategy = BezierSurfaceTool.DrawingCurveStrategy.Simple)
        {
            CurrentBezierSurfaceToolState.StartTool(leftControllerOrigin, rightControllerOrigin, steps ,diameter, drawingCurveStrategy);
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
        
        public BezierSurfaceSketchObject StopDrawSurface()
        {
            BezierSurfaceSketchObject surface = CurrentBezierSurfaceToolState.StopDrawingSurface();
            if (sketchWorld != null && surface != null)
            {
                sketchWorld.AddObject(surface);
            }
            return surface;
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

        public void SetSketchWorld(SketchWorld sketchWorld)
        {
            this.sketchWorld = sketchWorld;
        }
    }
}