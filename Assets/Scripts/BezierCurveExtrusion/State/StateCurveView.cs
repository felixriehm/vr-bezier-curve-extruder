using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;
using VRSketchingGeometry.SketchObjectManagement;

namespace BezierCurveExtrusion.State
{
    internal class StateCurveView : BezierCurveExtruderState
    {
        internal StateCurveView(BezierCurveExtruder tool, BezierCurveExtruderSettings settings, BezierCurveExtruderStateData stateData)
        : base(tool, settings, stateData)
        {
            BezierCurveExtruderStateData.OnStateChanged.Invoke(BezierCurveExtruder.BezierCurveExtruderState.CurveView);
        }
        
        internal override void Update()
        {
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(0, BezierCurveExtruderStateData));
            controlPoints.Add(BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(1, BezierCurveExtruderStateData));
            controlPoints.Add(BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(3, BezierCurveExtruderStateData));
            controlPoints.Add(BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(2, BezierCurveExtruderStateData));
            BezierCurveExtruderStateData.BezierCurveSketchObject.SetControlPoints(controlPoints);
        }

        internal override BezierCurveExtruder.BezierCurveExtruderState GetCurrentState()
        {
            return BezierCurveExtruder.BezierCurveExtruderState.CurveView;
        }

        internal override void Init(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f, BezierCurveExtruder.InteractionMethod interactionMethod = BezierCurveExtruder.InteractionMethod.Simple)
        {
            Debug.Log("Can not execute method 'StartTool()': Tool has already started.");
        }
        
        internal override void StartExtrusion()
        {
            // init bezier surface so it can be used later to continuously add temporary bezier patches
            BezierCurveExtruderStateData.CurrentExtrudedBezierCurve = Object.Instantiate(BezierCurveExtruderSettings.Defaults.BezierSurfaceSketchObjectPrefab).GetComponent<ExtrudedBezierCurveSketchObject>();

            // save current hold bezier curve so it can be used later to continuously draw the temporary bezier patch
            BezierCurveExtruderStateData.prevCpHandles = new Vector3[4];
            for (int i = 0; i < BezierCurveExtruderStateData.cpHandles.Length; i++)
            {
                BezierCurveExtruderStateData.prevCpHandles[i] = BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(i, BezierCurveExtruderStateData);
            }
            
            // init temporary bezier patch so it can be continuously be drawn later and added to the bezier surface
            BezierCurveExtruderStateData.temporaryBezierPatch = Object.Instantiate(BezierCurveExtruderSettings.Defaults.BezierPatchSketchObjectPrefab).GetComponent<BezierPatchSketchObject>();

            // deactivate bezier curve
            BezierCurveExtruderStateData.BezierCurveSketchObject.gameObject.SetActive(false);
            
            // set state to 'drawing surface'
            BezierCurveExtruder.CurrentBezierCurveExtruderState = new StateCurveExtrusion(BezierCurveExtruder, BezierCurveExtruderSettings, BezierCurveExtruderStateData);
        }

        internal override ExtrudedBezierCurveSketchObject StopExtrusion()
        {
            Debug.Log("Can not execute method 'StopDrawSurface()': Tool must be drawing surface.");
            return null;
        }
        
        internal override void Reset()
        {
            for (int i = 0; i < BezierCurveExtruderStateData.controllerHandles.Length; i++)
            {
                Object.Destroy(BezierCurveExtruderStateData.controllerHandles[i]);
            }
            
            for (int i = 0; i < BezierCurveExtruderStateData.cpHandles.Length; i++)
            {
                Object.Destroy(BezierCurveExtruderStateData.cpHandles[i]);
            }
            
            for (int i = 0; i < BezierCurveExtruderStateData.supplementaryCpHandles.Length; i++)
            {
                Object.Destroy(BezierCurveExtruderStateData.supplementaryCpHandles[i]);
            }
            
            Object.Destroy(BezierCurveExtruderStateData.BezierCurveSketchObject.gameObject);
            // if you exit the tool while drawing, 'StopDrawSurface()' does not destroy temporaryBezierPatch
            if (BezierCurveExtruderStateData.temporaryBezierPatch != null)
            {
                Object.Destroy(BezierCurveExtruderStateData.temporaryBezierPatch.gameObject);
            }
                
            BezierCurveExtruder.CurrentBezierCurveExtruderState = new StateIdle(BezierCurveExtruder, BezierCurveExtruderSettings, BezierCurveExtruderStateData);
        }
        
        internal override void ShowIndicators(bool show)
        {
            ShowIndicatorsHelper(show);
        }

        internal override void ChangeCurveIntensity(BezierCurveExtruder.BezierCurveExtruderController controller, float amount)
        {
            switch (controller)
            {
                case BezierCurveExtruder.BezierCurveExtruderController.Left:
                    BezierCurveExtruderStateData.cpHandles[1].transform.Translate(0, amount, 0);;
                    break;
                case BezierCurveExtruder.BezierCurveExtruderController.Right:
                    BezierCurveExtruderStateData.cpHandles[3].transform.Translate(0, amount, 0);;
                    break;
            }
        }
    }
}