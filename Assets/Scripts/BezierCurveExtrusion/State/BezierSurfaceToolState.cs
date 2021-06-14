using BezierCurveExtrusion.Strategy;
using UnityEngine;
using UnityEngine.Events;
using VRSketchingGeometry.SketchObjectManagement;

namespace BezierCurveExtrusion.State
{
    internal abstract class BezierSurfaceToolState
    {
        protected BezierCurveExtruder BezierCurveExtruder { get; set; }
        protected BezierSurfaceToolStateData BezierSurfaceToolStateData;
        protected BezierSurfaceToolSettings BezierSurfaceToolSettings;

        internal BezierSurfaceToolState(BezierCurveExtruder tool, BezierSurfaceToolSettings settings, BezierSurfaceToolStateData stateData)
        {
            BezierCurveExtruder = tool;
            BezierSurfaceToolSettings = settings;
            BezierSurfaceToolStateData = stateData;
        }
        
        internal abstract void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f, BezierCurveExtruder.DrawingCurveStrategy drawingCurveStrategy = BezierCurveExtruder.DrawingCurveStrategy.Simple);
        internal abstract void ExitTool();
        internal abstract void StartDrawingSurface();
        internal abstract ExtrudedBezierCurveSketchObject StopDrawingSurface();
        internal abstract void ShowIndicators(bool show);
        internal abstract void ChangeCurveIntensity(BezierCurveExtruder.BezierSurfaceToolController controller, float amount);
        internal abstract void Update();
        internal abstract BezierCurveExtruder.BezierSurfaceToolState GetCurrentState();
        protected internal void ShowIndicatorsHelper(bool show)
        {
            foreach (var controllerHandle in BezierSurfaceToolStateData.controllerHandles)
            {
                foreach (var childRenderer in controllerHandle.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = show;
                }
            }
            
            foreach (var cpHandle in BezierSurfaceToolStateData.cpHandles)
            {
                foreach (var childRenderer in cpHandle.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = show;
                }
            }
            
            foreach (var supplementaryCpHandle in BezierSurfaceToolStateData.supplementaryCpHandles)
            {
                foreach (var childRenderer in supplementaryCpHandle.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = show;
                }
            }
        }

        protected internal UnityEvent<BezierCurveExtruder.BezierSurfaceToolState> GetOnStateChangedEvent()
        {
            return BezierSurfaceToolStateData.OnStateChanged;
        }
        protected internal UnityEvent<BezierCurveExtruder.DrawingCurveStrategy> GetOnStrategyChangedEvent()
        {
            return BezierSurfaceToolStateData.OnStrategyChanged;
        }
        protected internal void SetDrawingCurveStrategy(BezierCurveExtruder.DrawingCurveStrategy strategy)
        {
            switch (strategy)
            {
                case BezierCurveExtruder.DrawingCurveStrategy.Simple:
                    // change strategy
                    BezierSurfaceToolStateData.drawingCurveStrategy = new StrategySimple();
                    BezierSurfaceToolStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.DrawingCurveStrategy.Simple);
                    
                    // change visibility of supplementary handles
                    foreach (var supplementaryCpHandle in BezierSurfaceToolStateData.supplementaryCpHandles)
                    {
                        foreach (var childRenderer in supplementaryCpHandle.GetComponentsInChildren<Renderer>())
                        {
                            childRenderer.enabled = false;
                        }
                    }
                    
                    // change color of cp handles
                    BezierSurfaceToolStateData.cpHandles[1].transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1,0,0);
                    BezierSurfaceToolStateData.cpHandles[3].transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1,0,0);
                    break;
                case BezierCurveExtruder.DrawingCurveStrategy.VectorAngle:
                    // change strategy
                    BezierSurfaceToolStateData.drawingCurveStrategy = new StrategyVectorAngle();
                    BezierSurfaceToolStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.DrawingCurveStrategy.VectorAngle);

                    ChangeVisibilityAndColorOfHandles();
                    break;
                case BezierCurveExtruder.DrawingCurveStrategy.RotationAngle:
                    // change strategy
                    BezierSurfaceToolStateData.drawingCurveStrategy = new StrategyRotationAngle();
                    BezierSurfaceToolStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.DrawingCurveStrategy.RotationAngle);

                    ChangeVisibilityAndColorOfHandles();
                    break;
                case BezierCurveExtruder.DrawingCurveStrategy.Distance:
                    // change strategy
                    BezierSurfaceToolStateData.drawingCurveStrategy = new StrategyDistance();
                    BezierSurfaceToolStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.DrawingCurveStrategy.Distance);

                    ChangeVisibilityAndColorOfHandles();
                    break;
            }
        }

        protected internal BezierCurveExtruder.DrawingCurveStrategy GetCurrentDrawingCurveStrategy()
        {
            return BezierSurfaceToolStateData.drawingCurveStrategy.GetCurrentStrategy();
        }

        private void ChangeVisibilityAndColorOfHandles()
        {
            // change visibility of supplementary handles
            foreach (var supplementaryCpHandle in BezierSurfaceToolStateData.supplementaryCpHandles)
            {
                foreach (var childRenderer in supplementaryCpHandle.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = true;
                }
            }
            // change color of cp handles
            BezierSurfaceToolStateData.cpHandles[1].transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0,0,1);
            BezierSurfaceToolStateData.cpHandles[3].transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0,0,1);
        }
    }
}