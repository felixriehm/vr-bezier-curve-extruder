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
        public BezierCurveExtruderSettings BezierCurveExtruderSettings;
        private SketchWorld sketchWorld;
        private CommandInvoker Invoker;
        
        internal State.BezierCurveExtruderState CurrentBezierCurveExtruderState { get; set; }
        
        public enum BezierCurveExtruderState
        {
            Idle,
            CurveView,
            CurveExtrusion,
        }
        
        public enum BezierCurveExtruderController
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
            CurrentBezierCurveExtruderState = new StateIdle(this, BezierCurveExtruderSettings, new BezierCurveExtruderStateData());
        }

        public void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f, BezierCurveExtruder.DrawingCurveStrategy drawingCurveStrategy = BezierCurveExtruder.DrawingCurveStrategy.Simple)
        {
            CurrentBezierCurveExtruderState.Init(leftControllerOrigin, rightControllerOrigin, steps ,diameter, drawingCurveStrategy);
        }

        private void Update()
        {
            CurrentBezierCurveExtruderState.Update();
        }

        public void ExitTool()
        {
            CurrentBezierCurveExtruderState.Reset();
        }

        public void ShowIndicators(bool show)
        {
            CurrentBezierCurveExtruderState.ShowIndicators(show);
        }
        
        public ExtrudedBezierCurveSketchObject StopDrawSurface()
        {
            ExtrudedBezierCurveSketchObject extrudedBezierCurve = CurrentBezierCurveExtruderState.StopExtrusion();
            if (sketchWorld != null && extrudedBezierCurve != null)
            {
                Invoker.ExecuteCommand(new AddObjectToSketchWorldRootCommand(extrudedBezierCurve, sketchWorld));
            }
            return extrudedBezierCurve;
        }
        
        public void StartDrawSurface()
        {
            CurrentBezierCurveExtruderState.StartExtrusion();
        }

        public void ChangeCurveIntensity(BezierCurveExtruderController controller, float amount)
        { 
            CurrentBezierCurveExtruderState.ChangeCurveIntensity(controller, amount);
        }

        public void SetDrawingCurveStrategy(DrawingCurveStrategy strategy)
        {
            CurrentBezierCurveExtruderState.SetDrawingCurveStrategy(strategy);
        }
        
        public BezierCurveExtruderState GetCurrentState()
        {
            return CurrentBezierCurveExtruderState.GetCurrentState();
        }
        
        public UnityEvent<BezierCurveExtruderState> GetOnStateChangedEvent()
        {
            return CurrentBezierCurveExtruderState.GetOnStateChangedEvent();
        }
        
        public UnityEvent<DrawingCurveStrategy> GetOnStrategyChangedEvent()
        {
            return CurrentBezierCurveExtruderState.GetOnStrategyChangedEvent();
        }
        
        public DrawingCurveStrategy GetCurrentDrawingCurveStrategy()
        {
            return CurrentBezierCurveExtruderState.GetCurrentDrawingCurveStrategy();
        }

        public void SetSketchWorld(SketchWorld sketchWorld)
        {
            this.sketchWorld = sketchWorld;
            Invoker = new CommandInvoker();
        }
    }
}