using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.BezierSurfaceTool.State
{
    internal class StateToolNotStarted : BezierSurfaceToolState
    {
        internal StateToolNotStarted(BezierSurfaceTool tool, BezierSurfaceToolSettings settings, BezierSurfaceToolStateData stateData)
            : base(tool, settings, stateData)
        {
        }

        internal override void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f)
        {
            // init arrays for the later for loop
            Transform[] controllerOrigins = new Transform[2];
            controllerOrigins[0] = leftControllerOrigin;
            controllerOrigins[1] = rightControllerOrigin;

            float[] bezierCurveStartOffsetDistance = new float[2];
            bezierCurveStartOffsetDistance[0] = BezierSurfaceToolSettings.bezierCurveLeftStartOffsetDistance;
            bezierCurveStartOffsetDistance[1] = BezierSurfaceToolSettings.bezierCurveRightStartOffsetDistance;

            Vector3[] bezierCurveStartOffsetRotation = new Vector3[2];
            bezierCurveStartOffsetRotation[0] = BezierSurfaceToolSettings.bezierCurveLeftStartOffsetRotation;
            bezierCurveStartOffsetRotation[1] = BezierSurfaceToolSettings.bezierCurveRightStartOffsetRotation;
            
            Vector3[] bezierCurveStartRotation = new Vector3[2];
            bezierCurveStartRotation[0] = BezierSurfaceToolSettings.bezierCurveLeftStartRotation;
            bezierCurveStartRotation[1] = BezierSurfaceToolSettings.bezierCurveRightStartRotation;
            
            float[] bezierCurveIntensity = new float[2];
            bezierCurveIntensity[0] = BezierSurfaceToolSettings.bezierCurveLeftIntensity;
            bezierCurveIntensity[1] = BezierSurfaceToolSettings.bezierCurveRightIntensity;

            // create cp handles and controller handle for left and right controller
            for (int i = 0; i < controllerOrigins.Length; i++)
            {
                // init controller handle
                BezierSurfaceToolStateData.controllerHandles[i] = Object.Instantiate<GameObject>(BezierSurfaceToolSettings.BezierSurfaceToolIndicatorPrefab);
                BezierSurfaceToolStateData.controllerHandles[i].name = "ControllerHandle" + i;
                BezierSurfaceToolStateData.controllerHandles[i].transform.SetParent(controllerOrigins[i]);
                // init indices for the arrays previously created
                int firstCpHandle = i * 2;
                int secondCpHandle = i * 2 + 1;
                
                // init first and second cp handle
                BezierSurfaceToolStateData.cpHandles[firstCpHandle] = Object.Instantiate<GameObject>(BezierSurfaceToolSettings.BezierSurfaceToolIndicatorPrefab);
                BezierSurfaceToolStateData.cpHandles[firstCpHandle].name = "CpHandle" + firstCpHandle;
                BezierSurfaceToolStateData.cpHandles[secondCpHandle] = Object.Instantiate<GameObject>(BezierSurfaceToolSettings.BezierSurfaceToolIndicatorPrefab);
                BezierSurfaceToolStateData.cpHandles[secondCpHandle].name = "CpHandle" + secondCpHandle;
                BezierSurfaceToolStateData.cpHandles[firstCpHandle].transform.SetParent(BezierSurfaceToolStateData.controllerHandles[i].transform);
                BezierSurfaceToolStateData.cpHandles[secondCpHandle].transform.SetParent(BezierSurfaceToolStateData.cpHandles[firstCpHandle].transform);
                    
                // set position and rotation of controller handle
                BezierSurfaceToolStateData.controllerHandles[i].transform.SetPositionAndRotation(
                    controllerOrigins[i].transform.position, 
                    controllerOrigins[i].rotation);
                BezierSurfaceToolStateData.controllerHandles[i].transform.Rotate(bezierCurveStartOffsetRotation[i]);
                 
                // set position and rotation of first cp handle
                BezierSurfaceToolStateData.cpHandles[firstCpHandle].transform.SetPositionAndRotation((BezierSurfaceToolStateData.controllerHandles[i].transform.forward * bezierCurveStartOffsetDistance[i]) + BezierSurfaceToolStateData.controllerHandles[i].transform.position, BezierSurfaceToolStateData.controllerHandles[i].transform.rotation );
                BezierSurfaceToolStateData.cpHandles[firstCpHandle].transform.Rotate(bezierCurveStartRotation[i]);
                
                // set position and rotation of second cp handle
                BezierSurfaceToolStateData.cpHandles[secondCpHandle].transform.SetPositionAndRotation((BezierSurfaceToolStateData.cpHandles[firstCpHandle].transform.up * bezierCurveIntensity[i]) + BezierSurfaceToolStateData.cpHandles[firstCpHandle].transform.position, BezierSurfaceToolStateData.cpHandles[firstCpHandle].transform.rotation );
            }

            // init Bezier curve
            BezierSurfaceToolStateData.BezierCurveSketchObject = Object.Instantiate<GameObject>(BezierSurfaceToolSettings.BezierCurveSketchObjectPrefab).GetComponent<BezierCurveSketchObject>();
            BezierSurfaceToolStateData.BezierCurveSketchObject.SetLineDiameter(diameter);
            BezierSurfaceToolStateData.BezierCurveSketchObject.SetInterpolationSteps(steps);

            // get control points for the bezier curve
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[0].transform.position);
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[1].transform.position);
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[3].transform.position);
            controlPoints.Add(BezierSurfaceToolStateData.cpHandles[2].transform.position);

            // set control points of bezier curve and draw it
            BezierSurfaceToolStateData.BezierCurveSketchObject.SetControlPoints(controlPoints);

            // set tool in use state to true
            BezierSurfaceTool.CurrentBezierSurfaceToolState = new StateDrawingCurve(BezierSurfaceTool, BezierSurfaceToolSettings, BezierSurfaceToolStateData);

            // show or hide indicators for the bezier curve
            ShowIndicatorsHelper(BezierSurfaceToolSettings.showIndicators);
        }

        internal override void ExitTool()
        {
            Debug.Log("Can not execute method 'ExitTool()': Tool must have been started.");
        }

        internal override void StartDrawingSurface()
        {
            Debug.Log("Can not execute method 'StartDrawSurface()': Tool must be drawing curve");
        }

        internal override void StopDrawingSurface()
        {
            Debug.Log("Can not execute method 'StopDrawSurface()': Tool must be drawing surface.");
        }

        internal override void ShowIndicators(bool show)
        {
            Debug.Log("Can not execute method 'ShowIndicators()': Tool must have been started.");
        }

        internal override void ChangeCurveIntensity(BezierSurfaceTool.BezierSurfaceToolController controller, float amount)
        {
            Debug.Log("Can not execute method 'ChangeCurveIntensity()': Tool must have been started.");
        }

        internal override void Update()
        {
        }

        internal override BezierSurfaceTool.BezierSurfaceToolState GetCurrentState()
        {
            return BezierSurfaceTool.BezierSurfaceToolState.ToolNotStarted;
        }
    }
}