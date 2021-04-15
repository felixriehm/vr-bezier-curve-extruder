using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.BezierSurfaceTool.State
{
    internal class StateDrawingSurface : SharedCurveAndSurfaceStateActions
    {
        internal StateDrawingSurface(BezierSurfaceTool tool, BezierSurfaceToolSettings settings, BezierSurfaceToolStateData stateData)
            : base(tool, settings, stateData)
        {
        }
        
        internal override void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f)
        {
            Debug.Log("Can not execute method 'StartTool()': Tool has already started.");
        }

        internal override void StartDrawingSurface()
        {
            Debug.Log("Can not execute method 'StartDrawSurface()': Tool must be drawing curve");
        }

        internal override void StopDrawingSurface()
        {
            if (!AllCounterpartVerticesAreEqual())
            {
                BezierSurfaceToolStateData.currentBezierSurface.AddPatch(BezierSurfaceToolStateData.temporaryBezierPatch);
            }
            Object.Destroy(BezierSurfaceToolStateData.temporaryBezierPatch.gameObject);

            BezierSurfaceToolStateData.BezierCurveSketchObject.gameObject.SetActive(true);
            BezierSurfaceTool.CurrentBezierSurfaceToolState = new StateDrawingCurve(BezierSurfaceTool, BezierSurfaceToolSettings, BezierSurfaceToolStateData);
        }

        internal override void Update()
        {
            RedrawTemporaryBezierPatch();
                
            if (IsTmpBezierPatchMinDistMet())
            {
                // Add temporary patch to surface by combining meshes.
                // There is no check for 'AllCounterPartVerticesAreEqual()' because 'BezierPatchMinDistance' ensures
                // that this can no be the case.
                BezierSurfaceToolStateData.currentBezierSurface.AddPatch(BezierSurfaceToolStateData.temporaryBezierPatch);
                    
                // save current hold bezier curve so it can be used later to continuously draw the temporary bezier patch
                for (int i = 0; i < BezierSurfaceToolStateData.cpHandles.Length; i++)
                {
                    BezierSurfaceToolStateData.prevCpHandles[i] = BezierSurfaceToolStateData.cpHandles[i].transform.position;
                }
            }
        }

        internal override BezierSurfaceTool.BezierSurfaceToolState GetCurrentState()
        {
            return BezierSurfaceTool.BezierSurfaceToolState.DrawingSurface;
        }

        private void RedrawTemporaryBezierPatch()
        {
            if(AllCounterpartVerticesAreEqual())
            {
                // this is needed to avoid the error: "[Physics.PhysX] cleaning the mesh failed"
                // this is happening because all Vertices overlap and that is messing up the cleaning procedures
                return;
            }
            
            BezierSurfaceToolStateData.temporaryBezierPatch.SetControlPoints(GetTmpBezierPatchCps(),4);
        }
        
        private bool IsTmpBezierPatchMinDistMet()
        {
            for (int i = 0; i < BezierSurfaceToolStateData.prevCpHandles.Length; i++)
            {
                if ((BezierSurfaceToolStateData.prevCpHandles[i] - BezierSurfaceToolStateData.cpHandles[i].transform.position).magnitude > BezierSurfaceToolSettings.BezierPatchMinDistance)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool AllCounterpartVerticesAreEqual()
        {
            return BezierSurfaceToolStateData.prevCpHandles[0] == BezierSurfaceToolStateData.cpHandles[0].transform.position &&
                   BezierSurfaceToolStateData.prevCpHandles[1] == BezierSurfaceToolStateData.cpHandles[1].transform.position &&
                   BezierSurfaceToolStateData.prevCpHandles[2] == BezierSurfaceToolStateData.cpHandles[2].transform.position &&
                   BezierSurfaceToolStateData.prevCpHandles[3] == BezierSurfaceToolStateData.cpHandles[3].transform.position;
        }
        
        private List<Vector3> GetTmpBezierPatchCps()
        {
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(BezierSurfaceToolStateData.prevCpHandles[0]);
            controlPoints.Add(BezierSurfaceToolStateData.prevCpHandles[1]);
            controlPoints.Add(BezierSurfaceToolStateData.prevCpHandles[3]);
            controlPoints.Add(BezierSurfaceToolStateData.prevCpHandles[2]);
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[0].transform.position);
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[1].transform.position);
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[3].transform.position);
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[2].transform.position);
            return controlPoints;
        }
    }
}