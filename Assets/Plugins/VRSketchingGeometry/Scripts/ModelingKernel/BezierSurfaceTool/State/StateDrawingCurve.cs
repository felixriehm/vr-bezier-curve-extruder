using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.BezierSurfaceTool.State
{
    internal class StateDrawingCurve : SharedCurveAndSurfaceStateActions
    {
        internal StateDrawingCurve(BezierSurfaceTool tool, BezierSurfaceToolSettings settings, BezierSurfaceToolStateData stateData)
            : base(tool, settings, stateData)
        {
        }
        
        internal override void Update()
        {
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[0].transform.position);
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[1].transform.position);
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[3].transform.position);
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[2].transform.position);
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
                BezierSurfaceToolStateData.prevCpHandles[i] = BezierSurfaceToolStateData.cpHandles[i].transform.position;
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
    }
}