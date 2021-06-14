using BezierCurveExtrusion.Strategy;
using UnityEngine;
using UnityEngine.Events;
using VRSketchingGeometry.SketchObjectManagement;

namespace BezierCurveExtrusion.State
{
    internal abstract class BezierCurveExtruderState
    {
        protected BezierCurveExtruder BezierCurveExtruder { get; set; }
        protected BezierCurveExtruderStateData BezierCurveExtruderStateData;
        protected BezierCurveExtruderSettings BezierCurveExtruderSettings;

        internal BezierCurveExtruderState(BezierCurveExtruder tool, BezierCurveExtruderSettings settings, BezierCurveExtruderStateData stateData)
        {
            BezierCurveExtruder = tool;
            BezierCurveExtruderSettings = settings;
            BezierCurveExtruderStateData = stateData;
        }
        
        internal abstract void Init(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f, BezierCurveExtruder.DrawingCurveStrategy drawingCurveStrategy = BezierCurveExtruder.DrawingCurveStrategy.Simple);
        internal abstract void Reset();
        internal abstract void StartExtrusion();
        internal abstract ExtrudedBezierCurveSketchObject StopExtrusion();
        internal abstract void ShowIndicators(bool show);
        internal abstract void ChangeCurveIntensity(BezierCurveExtruder.BezierCurveExtruderController controller, float amount);
        internal abstract void Update();
        internal abstract BezierCurveExtruder.BezierCurveExtruderState GetCurrentState();
        protected internal void ShowIndicatorsHelper(bool show)
        {
            foreach (var controllerHandle in BezierCurveExtruderStateData.controllerHandles)
            {
                foreach (var childRenderer in controllerHandle.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = show;
                }
            }
            
            foreach (var cpHandle in BezierCurveExtruderStateData.cpHandles)
            {
                foreach (var childRenderer in cpHandle.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = show;
                }
            }
            
            foreach (var supplementaryCpHandle in BezierCurveExtruderStateData.supplementaryCpHandles)
            {
                foreach (var childRenderer in supplementaryCpHandle.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = show;
                }
            }
        }

        protected internal UnityEvent<BezierCurveExtruder.BezierCurveExtruderState> GetOnStateChangedEvent()
        {
            return BezierCurveExtruderStateData.OnStateChanged;
        }
        protected internal UnityEvent<BezierCurveExtruder.DrawingCurveStrategy> GetOnStrategyChangedEvent()
        {
            return BezierCurveExtruderStateData.OnStrategyChanged;
        }
        protected internal void SetDrawingCurveStrategy(BezierCurveExtruder.DrawingCurveStrategy strategy)
        {
            switch (strategy)
            {
                case BezierCurveExtruder.DrawingCurveStrategy.Simple:
                    // change strategy
                    BezierCurveExtruderStateData.drawingCurveStrategy = new StrategySimple();
                    BezierCurveExtruderStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.DrawingCurveStrategy.Simple);
                    
                    // change visibility of supplementary handles
                    foreach (var supplementaryCpHandle in BezierCurveExtruderStateData.supplementaryCpHandles)
                    {
                        foreach (var childRenderer in supplementaryCpHandle.GetComponentsInChildren<Renderer>())
                        {
                            childRenderer.enabled = false;
                        }
                    }
                    
                    // change color of cp handles
                    BezierCurveExtruderStateData.cpHandles[1].transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1,0,0);
                    BezierCurveExtruderStateData.cpHandles[3].transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1,0,0);
                    break;
                case BezierCurveExtruder.DrawingCurveStrategy.VectorAngle:
                    // change strategy
                    BezierCurveExtruderStateData.drawingCurveStrategy = new StrategyVectorAngle();
                    BezierCurveExtruderStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.DrawingCurveStrategy.VectorAngle);

                    ChangeVisibilityAndColorOfHandles();
                    break;
                case BezierCurveExtruder.DrawingCurveStrategy.RotationAngle:
                    // change strategy
                    BezierCurveExtruderStateData.drawingCurveStrategy = new StrategyRotationAngle();
                    BezierCurveExtruderStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.DrawingCurveStrategy.RotationAngle);

                    ChangeVisibilityAndColorOfHandles();
                    break;
                case BezierCurveExtruder.DrawingCurveStrategy.Distance:
                    // change strategy
                    BezierCurveExtruderStateData.drawingCurveStrategy = new StrategyDistance();
                    BezierCurveExtruderStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.DrawingCurveStrategy.Distance);

                    ChangeVisibilityAndColorOfHandles();
                    break;
            }
        }

        protected internal BezierCurveExtruder.DrawingCurveStrategy GetCurrentDrawingCurveStrategy()
        {
            return BezierCurveExtruderStateData.drawingCurveStrategy.GetCurrentStrategy();
        }

        private void ChangeVisibilityAndColorOfHandles()
        {
            // change visibility of supplementary handles
            foreach (var supplementaryCpHandle in BezierCurveExtruderStateData.supplementaryCpHandles)
            {
                foreach (var childRenderer in supplementaryCpHandle.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = true;
                }
            }
            // change color of cp handles
            BezierCurveExtruderStateData.cpHandles[1].transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0,0,1);
            BezierCurveExtruderStateData.cpHandles[3].transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0,0,1);
        }
    }
}