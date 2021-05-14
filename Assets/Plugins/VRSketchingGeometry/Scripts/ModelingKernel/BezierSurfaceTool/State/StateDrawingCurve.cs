using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.BezierSurfaceTool.State
{
    internal class StateDrawingCurve : BezierSurfaceToolState
    {
        internal StateDrawingCurve(BezierSurfaceTool tool, BezierSurfaceToolSettings settings, BezierSurfaceToolStateData stateData)
        : base(tool, settings, stateData)
        {
            BezierSurfaceToolStateData.OnStateChanged.Invoke(BezierSurfaceTool.BezierSurfaceToolState.DrawingCurve);
        }
        
        internal override void Update()
        {
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(0, BezierSurfaceToolStateData));
            controlPoints.Add(BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(1, BezierSurfaceToolStateData));
            controlPoints.Add(BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(3, BezierSurfaceToolStateData));
            controlPoints.Add(BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(2, BezierSurfaceToolStateData));
            BezierSurfaceToolStateData.BezierCurveSketchObject.SetControlPoints(controlPoints);
        }

        internal override BezierSurfaceTool.BezierSurfaceToolState GetCurrentState()
        {
            return BezierSurfaceTool.BezierSurfaceToolState.DrawingCurve;
        }

        internal override void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f)
        {
            Debug.Log("Can not execute method 'StartTool()': Tool has already started.");
        }
        
        internal override void StartDrawingSurface()
        {
            // init bezier surface so it can be used later to continuously add temporary bezier patches
            BezierSurfaceToolStateData.currentBezierSurface = Object.Instantiate(BezierSurfaceToolSettings.BezierSurfaceSketchObjectPrefab).GetComponent<BezierSurfaceSketchObject>();
            
            // save current hold bezier curve so it can be used later to continuously draw the temporary bezier patch
            BezierSurfaceToolStateData.prevCpHandles = new Vector3[4];
            for (int i = 0; i < BezierSurfaceToolStateData.cpHandles.Length; i++)
            {
                BezierSurfaceToolStateData.prevCpHandles[i] = BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(i, BezierSurfaceToolStateData);
            }
            
            // init temporary bezier patch so it can be continuously be drawn later and added to the bezier surface
            BezierSurfaceToolStateData.temporaryBezierPatch = Object.Instantiate(BezierSurfaceToolSettings.BezierPatchSketchObjectPrefab).GetComponent<BezierPatchSketchObject>();

            // deactivate bezier curve
            BezierSurfaceToolStateData.BezierCurveSketchObject.gameObject.SetActive(false);
            
            // set state to 'drawing surface'
            BezierSurfaceTool.CurrentBezierSurfaceToolState = new StateDrawingSurface(BezierSurfaceTool, BezierSurfaceToolSettings, BezierSurfaceToolStateData);
        }

        internal override void StopDrawingSurface()
        {
            Debug.Log("Can not execute method 'StopDrawSurface()': Tool must be drawing surface.");
        }
        
        internal override void ExitTool()
        {
            for (int i = 0; i < BezierSurfaceToolStateData.controllerHandles.Length; i++)
            {
                Object.Destroy(BezierSurfaceToolStateData.controllerHandles[i]);
            }
            
            for (int i = 0; i < BezierSurfaceToolStateData.cpHandles.Length; i++)
            {
                Object.Destroy(BezierSurfaceToolStateData.cpHandles[i]);
            }
            
            for (int i = 0; i < BezierSurfaceToolStateData.supplementaryCpHandles.Length; i++)
            {
                Object.Destroy(BezierSurfaceToolStateData.supplementaryCpHandles[i]);
            }
            
            Object.Destroy(BezierSurfaceToolStateData.BezierCurveSketchObject.gameObject);
            // if you exit the tool while drawing, 'StopDrawSurface()' does not destroy temporaryBezierPatch
            if (BezierSurfaceToolStateData.temporaryBezierPatch != null)
            {
                Object.Destroy(BezierSurfaceToolStateData.temporaryBezierPatch.gameObject);
            }
                
            BezierSurfaceTool.CurrentBezierSurfaceToolState = new StateToolNotStarted(BezierSurfaceTool, BezierSurfaceToolSettings, BezierSurfaceToolStateData);
        }
        
        internal override void ShowIndicators(bool show)
        {
            ShowIndicatorsHelper(show);
        }

        internal override void ChangeCurveIntensity(BezierSurfaceTool.BezierSurfaceToolController controller, float amount)
        {
            switch (controller)
            {
                case BezierSurfaceTool.BezierSurfaceToolController.Left:
                    BezierSurfaceToolStateData.cpHandles[1].transform.Translate(0, amount, 0);;
                    break;
                case BezierSurfaceTool.BezierSurfaceToolController.Right:
                    BezierSurfaceToolStateData.cpHandles[3].transform.Translate(0, amount, 0);;
                    break;
            }
        }
    }
}