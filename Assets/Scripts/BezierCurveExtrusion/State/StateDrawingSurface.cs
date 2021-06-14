using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;
using VRSketchingGeometry.SketchObjectManagement;

namespace BezierCurveExtrusion.State
{
    internal class StateDrawingSurface : BezierSurfaceToolState
    {
        internal StateDrawingSurface(BezierCurveExtruder tool, BezierSurfaceToolSettings settings, BezierSurfaceToolStateData stateData)
            : base(tool, settings, stateData)
        {
            BezierSurfaceToolStateData.OnStateChanged.Invoke(BezierCurveExtruder.BezierSurfaceToolState.DrawingSurface);
        }
        
        internal override void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f, BezierCurveExtruder.DrawingCurveStrategy drawingCurveStrategy = BezierCurveExtruder.DrawingCurveStrategy.Simple)
        {
            Debug.Log("Can not execute method 'StartTool()': Tool has already started.");
        }

        internal override void StartDrawingSurface()
        {
            Debug.Log("Can not execute method 'StartDrawSurface()': Tool must be drawing curve");
        }

        internal override ExtrudedBezierCurveSketchObject StopDrawingSurface()
        {
            if (!AllCounterpartVerticesAreEqual())
            {
                BezierSurfaceToolStateData.CurrentExtrudedBezierCurve.AddPatch(BezierSurfaceToolStateData.temporaryBezierPatch);
                BezierSurfaceToolStateData.CurrentExtrudedBezierCurve.CombinePatchesToSingleMesh();
            }
            Object.Destroy(BezierSurfaceToolStateData.temporaryBezierPatch.gameObject);

            BezierSurfaceToolStateData.BezierCurveSketchObject.gameObject.SetActive(true);
            BezierCurveExtruder.CurrentBezierSurfaceToolState = new StateDrawingCurve(BezierCurveExtruder, BezierSurfaceToolSettings, BezierSurfaceToolStateData);

            return BezierSurfaceToolStateData.CurrentExtrudedBezierCurve;
        }

        internal override void Update()
        {
            RedrawTemporaryBezierPatch();
                
            if (IsTmpBezierPatchMinDistMet())
            {
                // Add temporary patch to surface by combining meshes.
                // There is no check for 'AllCounterPartVerticesAreEqual()' because 'BezierPatchMinDistance' ensures
                // that this can no be the case.
                BezierSurfaceToolStateData.CurrentExtrudedBezierCurve.AddPatch(BezierSurfaceToolStateData.temporaryBezierPatch);
                    
                // save current hold bezier curve so it can be used later to continuously draw the temporary bezier patch
                for (int i = 0; i < BezierSurfaceToolStateData.cpHandles.Length; i++)
                {
                    BezierSurfaceToolStateData.prevCpHandles[i] = BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(i, BezierSurfaceToolStateData);
                }
            }
        }

        internal override BezierCurveExtruder.BezierSurfaceToolState GetCurrentState()
        {
            return BezierCurveExtruder.BezierSurfaceToolState.DrawingSurface;
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
                if ((BezierSurfaceToolStateData.prevCpHandles[i] - BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(i, BezierSurfaceToolStateData)).magnitude > BezierSurfaceToolSettings.BezierPatchMinDistance)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool AllCounterpartVerticesAreEqual()
        {
            return BezierSurfaceToolStateData.prevCpHandles[0] == BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(0, BezierSurfaceToolStateData) &&
                   BezierSurfaceToolStateData.prevCpHandles[1] == BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(1, BezierSurfaceToolStateData) &&
                   BezierSurfaceToolStateData.prevCpHandles[2] == BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(2, BezierSurfaceToolStateData) &&
                   BezierSurfaceToolStateData.prevCpHandles[3] == BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(3, BezierSurfaceToolStateData);
        }
        
        private List<Vector3> GetTmpBezierPatchCps()
        {
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(BezierSurfaceToolStateData.prevCpHandles[0]);
            controlPoints.Add(BezierSurfaceToolStateData.prevCpHandles[1]);
            controlPoints.Add(BezierSurfaceToolStateData.prevCpHandles[3]);
            controlPoints.Add(BezierSurfaceToolStateData.prevCpHandles[2]);
            controlPoints.Add(BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(0, BezierSurfaceToolStateData));
            controlPoints.Add(BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(1, BezierSurfaceToolStateData));
            controlPoints.Add(BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(3, BezierSurfaceToolStateData));
            controlPoints.Add(BezierSurfaceToolStateData.drawingCurveStrategy.CalculateControlPoint(2, BezierSurfaceToolStateData));
            return controlPoints;
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
            
            // the surface the user is currently drawing will also be deleted
            Object.Destroy(BezierSurfaceToolStateData.CurrentExtrudedBezierCurve);
            
            BezierCurveExtruder.CurrentBezierSurfaceToolState = new StateIdle(BezierCurveExtruder, BezierSurfaceToolSettings, BezierSurfaceToolStateData);
        }
        
        internal override void ShowIndicators(bool show)
        {
            ShowIndicatorsHelper(show);
        }

        internal override void ChangeCurveIntensity(BezierCurveExtruder.BezierSurfaceToolController controller, float amount)
        {
            Debug.Log("Can not execute method 'ChangeCurveIntensity()': Changing curve intensity while drawing a surface is not supported.");
        }
    }
}