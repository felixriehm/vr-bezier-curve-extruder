using BezierCurveExtrusion.InteractionMethod;
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
        
        internal abstract void Init(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f, BezierCurveExtruder.InteractionMethod interactionMethod = BezierCurveExtruder.InteractionMethod.Simple);
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
        protected internal UnityEvent<BezierCurveExtruder.InteractionMethod> GetOnStrategyChangedEvent()
        {
            return BezierCurveExtruderStateData.OnStrategyChanged;
        }
        protected internal void SetInteractionMethod(BezierCurveExtruder.InteractionMethod strategy)
        {
            switch (strategy)
            {
                case BezierCurveExtruder.InteractionMethod.Simple:
                    // change strategy
                    BezierCurveExtruderStateData.InteractionMethod = new MethodSimple();
                    BezierCurveExtruderStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.InteractionMethod.Simple);
                    
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
                case BezierCurveExtruder.InteractionMethod.VectorAngle:
                    // change strategy
                    BezierCurveExtruderStateData.InteractionMethod = new MethodVectorAngle();
                    BezierCurveExtruderStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.InteractionMethod.VectorAngle);

                    ChangeVisibilityAndColorOfHandles();
                    break;
                case BezierCurveExtruder.InteractionMethod.RotationAngle:
                    // change strategy
                    BezierCurveExtruderStateData.InteractionMethod = new MethodRotationAngle();
                    BezierCurveExtruderStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.InteractionMethod.RotationAngle);

                    ChangeVisibilityAndColorOfHandles();
                    break;
                case BezierCurveExtruder.InteractionMethod.Distance:
                    // change strategy
                    BezierCurveExtruderStateData.InteractionMethod = new MethodDistance();
                    BezierCurveExtruderStateData.OnStrategyChanged.Invoke(BezierCurveExtruder.InteractionMethod.Distance);

                    ChangeVisibilityAndColorOfHandles();
                    break;
            }
        }

        protected internal BezierCurveExtruder.InteractionMethod GetCurrentDrawingCurveStrategy()
        {
            return BezierCurveExtruderStateData.InteractionMethod.GetCurrentInteractionMethod();
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