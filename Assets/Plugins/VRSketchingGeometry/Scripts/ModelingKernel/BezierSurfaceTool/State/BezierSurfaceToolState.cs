using UnityEngine;
using UnityEngine.Events;
using VRSketchingGeometry.BezierSurfaceTool.Strategy;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.BezierSurfaceTool.State
{
    internal abstract class BezierSurfaceToolState
    {
        protected BezierSurfaceTool BezierSurfaceTool { get; set; }
        protected BezierSurfaceToolStateData BezierSurfaceToolStateData;
        protected BezierSurfaceToolSettings BezierSurfaceToolSettings;

        internal BezierSurfaceToolState(BezierSurfaceTool tool, BezierSurfaceToolSettings settings, BezierSurfaceToolStateData stateData)
        {
            BezierSurfaceTool = tool;
            BezierSurfaceToolSettings = settings;
            BezierSurfaceToolStateData = stateData;
        }
        
        internal abstract void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f);
        internal abstract void ExitTool();
        internal abstract void StartDrawingSurface();
        internal abstract BezierSurfaceSketchObject StopDrawingSurface();
        internal abstract void ShowIndicators(bool show);
        internal abstract void ChangeCurveIntensity(BezierSurfaceTool.BezierSurfaceToolController controller, float amount);
        internal abstract void Update();
        internal abstract BezierSurfaceTool.BezierSurfaceToolState GetCurrentState();
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

            SetDrawingCurveStrategy(GetCurrentDrawingCurveStrategy());
        }

        protected internal UnityEvent<BezierSurfaceTool.BezierSurfaceToolState> GetOnStateChangedEvent()
        {
            return BezierSurfaceToolStateData.OnStateChanged;
        }
        protected internal UnityEvent<BezierSurfaceTool.DrawingCurveStrategy> GetOnStrategyChangedEvent()
        {
            return BezierSurfaceToolStateData.OnStrategyChanged;
        }
        protected internal void SetDrawingCurveStrategy(BezierSurfaceTool.DrawingCurveStrategy strategy)
        {
            switch (strategy)
            {
                case BezierSurfaceTool.DrawingCurveStrategy.Simple:
                    // change strategy
                    BezierSurfaceToolStateData.drawingCurveStrategy = new StrategySimple();
                    BezierSurfaceToolStateData.OnStrategyChanged.Invoke(BezierSurfaceTool.DrawingCurveStrategy.Simple);
                    
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
                case BezierSurfaceTool.DrawingCurveStrategy.VectorAngle:
                    // change strategy
                    BezierSurfaceToolStateData.drawingCurveStrategy = new StrategyVectorAngle();
                    BezierSurfaceToolStateData.OnStrategyChanged.Invoke(BezierSurfaceTool.DrawingCurveStrategy.VectorAngle);

                    ChangeVisibilityAndColorOfHandles();
                    break;
                case BezierSurfaceTool.DrawingCurveStrategy.RotationAngle:
                    // change strategy
                    BezierSurfaceToolStateData.drawingCurveStrategy = new StrategyRotationAngle();
                    BezierSurfaceToolStateData.OnStrategyChanged.Invoke(BezierSurfaceTool.DrawingCurveStrategy.RotationAngle);

                    ChangeVisibilityAndColorOfHandles();
                    break;
                case BezierSurfaceTool.DrawingCurveStrategy.Distance:
                    // change strategy
                    BezierSurfaceToolStateData.drawingCurveStrategy = new StrategyDistance();
                    BezierSurfaceToolStateData.OnStrategyChanged.Invoke(BezierSurfaceTool.DrawingCurveStrategy.Distance);

                    ChangeVisibilityAndColorOfHandles();
                    break;
            }
        }

        protected internal BezierSurfaceTool.DrawingCurveStrategy GetCurrentDrawingCurveStrategy()
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