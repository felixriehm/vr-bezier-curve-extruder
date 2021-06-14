using System;
using System.Collections.Generic;
using BezierCurveExtrusion.State;
using UnityEditorInternal;
using VRSketchingGeometry.SketchObjectManagement;
using UnityEngine;
using UnityEngine.Events;
using VRSketchingGeometry.Commands;

namespace BezierCurveExtrusion
{
    public class BezierCurveExtruder : MonoBehaviour
    {
        public BezierSurfaceToolSettings BezierSurfaceToolSettings;
        private SketchWorld sketchWorld;
        private CommandInvoker Invoker;
        
        internal State.BezierSurfaceToolState CurrentBezierSurfaceToolState { get; set; }
        
        public enum BezierSurfaceToolState
        {
            Idle,
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
            CurrentBezierSurfaceToolState = new StateIdle(this, BezierSurfaceToolSettings, new BezierSurfaceToolStateData());
        }

        public void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f, BezierCurveExtruder.DrawingCurveStrategy drawingCurveStrategy = BezierCurveExtruder.DrawingCurveStrategy.Simple)
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
        
        public ExtrudedBezierCurveSketchObject StopDrawSurface()
        {
            ExtrudedBezierCurveSketchObject extrudedBezierCurve = CurrentBezierSurfaceToolState.StopDrawingSurface();
            if (sketchWorld != null && extrudedBezierCurve != null)
            {
                Invoker.ExecuteCommand(new AddObjectToSketchWorldRootCommand(extrudedBezierCurve, sketchWorld));
            }
            return extrudedBezierCurve;
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
            Invoker = new CommandInvoker();
        }
    }
}