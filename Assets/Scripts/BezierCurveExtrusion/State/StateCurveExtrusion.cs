using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;
using VRSketchingGeometry.SketchObjectManagement;

namespace BezierCurveExtrusion.State
{
    internal class StateCurveExtrusion : BezierCurveExtruderState
    {
        internal StateCurveExtrusion(BezierCurveExtruder tool, BezierCurveExtruderSettings settings, BezierCurveExtruderStateData stateData)
            : base(tool, settings, stateData)
        {
            BezierCurveExtruderStateData.OnStateChanged.Invoke(BezierCurveExtruder.BezierCurveExtruderState.CurveExtrusion);
        }
        
        internal override void Init(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f, BezierCurveExtruder.InteractionMethod interactionMethod = BezierCurveExtruder.InteractionMethod.Simple)
        {
            Debug.Log("Can not execute method 'StartTool()': Tool has already started.");
        }

        internal override void StartExtrusion()
        {
            Debug.Log("Can not execute method 'StartDrawSurface()': Tool must be drawing curve");
        }

        internal override ExtrudedBezierCurveSketchObject StopExtrusion()
        {
            if (!AllCounterpartVerticesAreEqual())
            {
                BezierCurveExtruderStateData.CurrentExtrudedBezierCurve.AddPatch(BezierCurveExtruderStateData.temporaryBezierPatch);
                BezierCurveExtruderStateData.CurrentExtrudedBezierCurve.CombinePatchesToSingleMesh();
            }
            Object.Destroy(BezierCurveExtruderStateData.temporaryBezierPatch.gameObject);

            BezierCurveExtruderStateData.BezierCurveSketchObject.gameObject.SetActive(true);
            BezierCurveExtruder.CurrentBezierCurveExtruderState = new StateCurveView(BezierCurveExtruder, BezierCurveExtruderSettings, BezierCurveExtruderStateData);

            return BezierCurveExtruderStateData.CurrentExtrudedBezierCurve;
        }

        internal override void Update()
        {
            RedrawTemporaryBezierPatch();
                
            if (IsTmpBezierPatchMinDistMet())
            {
                // Add temporary patch to surface by combining meshes.
                // There is no check for 'AllCounterPartVerticesAreEqual()' because 'BezierPatchMinDistance' ensures
                // that this can no be the case.
                BezierCurveExtruderStateData.CurrentExtrudedBezierCurve.AddPatch(BezierCurveExtruderStateData.temporaryBezierPatch);
                    
                // save current hold bezier curve so it can be used later to continuously draw the temporary bezier patch
                for (int i = 0; i < BezierCurveExtruderStateData.cpHandles.Length; i++)
                {
                    BezierCurveExtruderStateData.prevCpHandles[i] = BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(i, BezierCurveExtruderStateData);
                }
            }
        }

        internal override BezierCurveExtruder.BezierCurveExtruderState GetCurrentState()
        {
            return BezierCurveExtruder.BezierCurveExtruderState.CurveExtrusion;
        }

        private void RedrawTemporaryBezierPatch()
        {
            if(AllCounterpartVerticesAreEqual())
            {
                // this is needed to avoid the error: "[Physics.PhysX] cleaning the mesh failed"
                // this is happening because all Vertices overlap and that is messing up the cleaning procedures
                return;
            }
            
            BezierCurveExtruderStateData.temporaryBezierPatch.SetControlPoints(GetTmpBezierPatchCps(),4);
        }
        
        private bool IsTmpBezierPatchMinDistMet()
        {
            for (int i = 0; i < BezierCurveExtruderStateData.prevCpHandles.Length; i++)
            {
                if ((BezierCurveExtruderStateData.prevCpHandles[i] - BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(i, BezierCurveExtruderStateData)).magnitude > BezierCurveExtruderSettings.BezierPatchMinDistance)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool AllCounterpartVerticesAreEqual()
        {
            return BezierCurveExtruderStateData.prevCpHandles[0] == BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(0, BezierCurveExtruderStateData) &&
                   BezierCurveExtruderStateData.prevCpHandles[1] == BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(1, BezierCurveExtruderStateData) &&
                   BezierCurveExtruderStateData.prevCpHandles[2] == BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(2, BezierCurveExtruderStateData) &&
                   BezierCurveExtruderStateData.prevCpHandles[3] == BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(3, BezierCurveExtruderStateData);
        }
        
        private List<Vector3> GetTmpBezierPatchCps()
        {
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(BezierCurveExtruderStateData.prevCpHandles[0]);
            controlPoints.Add(BezierCurveExtruderStateData.prevCpHandles[1]);
            controlPoints.Add(BezierCurveExtruderStateData.prevCpHandles[3]);
            controlPoints.Add(BezierCurveExtruderStateData.prevCpHandles[2]);
            controlPoints.Add(BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(0, BezierCurveExtruderStateData));
            controlPoints.Add(BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(1, BezierCurveExtruderStateData));
            controlPoints.Add(BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(3, BezierCurveExtruderStateData));
            controlPoints.Add(BezierCurveExtruderStateData.InteractionMethod.CalculateControlPoint(2, BezierCurveExtruderStateData));
            return controlPoints;
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
            
            // the surface the user is currently drawing will also be deleted
            Object.Destroy(BezierCurveExtruderStateData.CurrentExtrudedBezierCurve);
            
            BezierCurveExtruder.CurrentBezierCurveExtruderState = new StateIdle(BezierCurveExtruder, BezierCurveExtruderSettings, BezierCurveExtruderStateData);
        }
        
        internal override void ShowIndicators(bool show)
        {
            ShowIndicatorsHelper(show);
        }

        internal override void ChangeCurveIntensity(BezierCurveExtruder.BezierCurveExtruderController controller, float amount)
        {
            Debug.Log("Can not execute method 'ChangeCurveIntensity()': Changing curve intensity while drawing a surface is not supported.");
        }
    }
}