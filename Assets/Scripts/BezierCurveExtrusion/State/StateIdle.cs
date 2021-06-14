using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace BezierCurveExtrusion.State
{
    internal class StateIdle : BezierCurveExtruderState
    {
        internal StateIdle(BezierCurveExtruder tool, BezierCurveExtruderSettings settings, BezierCurveExtruderStateData stateData)
            : base(tool, settings, stateData)
        {
            BezierCurveExtruderStateData.OnStateChanged.Invoke(BezierCurveExtruder.BezierCurveExtruderState.Idle);
        }

        internal override void Init(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f, BezierCurveExtruder.DrawingCurveStrategy drawingCurveStrategy = BezierCurveExtruder.DrawingCurveStrategy.Simple)
        {
            // init arrays for the later for loop
            Transform[] controllerOrigins = new Transform[2];
            controllerOrigins[0] = leftControllerOrigin;
            controllerOrigins[1] = rightControllerOrigin;

            float[] bezierCurveStartOffsetDistance = new float[2];
            bezierCurveStartOffsetDistance[0] = BezierCurveExtruderSettings.bezierCurveLeftStartOffsetDistance;
            bezierCurveStartOffsetDistance[1] = BezierCurveExtruderSettings.bezierCurveRightStartOffsetDistance;

            Vector3[] bezierCurveStartOffsetRotation = new Vector3[2];
            bezierCurveStartOffsetRotation[0] = BezierCurveExtruderSettings.bezierCurveLeftStartOffsetRotation;
            bezierCurveStartOffsetRotation[1] = BezierCurveExtruderSettings.bezierCurveRightStartOffsetRotation;
            
            Vector3[] bezierCurveStartRotation = new Vector3[2];
            bezierCurveStartRotation[0] = BezierCurveExtruderSettings.bezierCurveLeftStartRotation;
            bezierCurveStartRotation[1] = BezierCurveExtruderSettings.bezierCurveRightStartRotation;
            
            float[] bezierCurveIntensity = new float[2];
            bezierCurveIntensity[0] = BezierCurveExtruderSettings.bezierCurveLeftIntensity;
            bezierCurveIntensity[1] = BezierCurveExtruderSettings.bezierCurveRightIntensity;

            // create cp handles and controller handle for left and right controller
            for (int i = 0; i < controllerOrigins.Length; i++)
            {
                // init controller handle
                BezierCurveExtruderStateData.controllerHandles[i] = Object.Instantiate<GameObject>(BezierCurveExtruderSettings.BezierSurfaceToolIndicatorPrefab);
                BezierCurveExtruderStateData.controllerHandles[i].name = "ControllerHandle" + i;
                BezierCurveExtruderStateData.controllerHandles[i].transform.SetParent(controllerOrigins[i]);
                BezierCurveExtruderStateData.controllerHandles[i].transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color =
                    new Color(0.5f, 0.5f, 0.5f);
                
                // init indices for the arrays previously created
                int firstCpHandle = i * 2;
                int secondCpHandle = i * 2 + 1;
                
                // init first and second cp handle
                BezierCurveExtruderStateData.cpHandles[firstCpHandle] = Object.Instantiate<GameObject>(BezierCurveExtruderSettings.BezierSurfaceToolIndicatorPrefab);
                BezierCurveExtruderStateData.cpHandles[firstCpHandle].name = "CpHandle" + firstCpHandle;
                BezierCurveExtruderStateData.cpHandles[secondCpHandle] = Object.Instantiate<GameObject>(BezierCurveExtruderSettings.BezierSurfaceToolIndicatorPrefab);
                BezierCurveExtruderStateData.cpHandles[secondCpHandle].name = "CpHandle" + secondCpHandle;
                BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform.SetParent(BezierCurveExtruderStateData.controllerHandles[i].transform);
                BezierCurveExtruderStateData.cpHandles[secondCpHandle].transform.SetParent(BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform);
                
                // init supplementary cp handles
                BezierCurveExtruderStateData.supplementaryCpHandles[i] = Object.Instantiate<GameObject>(BezierCurveExtruderSettings.BezierSurfaceToolIndicatorPrefab);
                BezierCurveExtruderStateData.supplementaryCpHandles[i].name = "SupplementaryCpHandle" + i;
                BezierCurveExtruderStateData.supplementaryCpHandles[i].transform.SetParent(BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform);
                    
                // set position and rotation of controller handle
                BezierCurveExtruderStateData.controllerHandles[i].transform.SetPositionAndRotation(
                    controllerOrigins[i].transform.position, 
                    controllerOrigins[i].rotation);
                BezierCurveExtruderStateData.controllerHandles[i].transform.Rotate(bezierCurveStartOffsetRotation[i]);
                 
                // set position and rotation of first cp handle
                BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform.SetPositionAndRotation((BezierCurveExtruderStateData.controllerHandles[i].transform.forward * bezierCurveStartOffsetDistance[i]) + BezierCurveExtruderStateData.controllerHandles[i].transform.position, BezierCurveExtruderStateData.controllerHandles[i].transform.rotation );
                BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform.Rotate(bezierCurveStartRotation[i]);
                
                // set position and rotation of second cp handle
                BezierCurveExtruderStateData.cpHandles[secondCpHandle].transform.SetPositionAndRotation((BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform.up * bezierCurveIntensity[i]) + BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform.position, BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform.rotation );
                
                // set position and rotation of supplementary cp handles
                BezierCurveExtruderStateData.supplementaryCpHandles[i].transform.SetPositionAndRotation((BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform.up * bezierCurveIntensity[i]) + BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform.position, BezierCurveExtruderStateData.cpHandles[firstCpHandle].transform.rotation );
            }

            // init Bezier curve
            BezierCurveExtruderStateData.BezierCurveSketchObject = Object.Instantiate<GameObject>(BezierCurveExtruderSettings.Defaults.BezierCurveSketchObjectPrefab).GetComponent<BezierCurveSketchObject>();
            BezierCurveExtruderStateData.BezierCurveSketchObject.SetLineDiameter(diameter);
            BezierCurveExtruderStateData.BezierCurveSketchObject.SetInterpolationSteps(steps);

            // set drawing curve strategy
            SetDrawingCurveStrategy(drawingCurveStrategy);
            
            // get control points for the bezier curve
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(BezierCurveExtruderStateData.drawingCurveStrategy.CalculateControlPoint(0, BezierCurveExtruderStateData));
            controlPoints.Add(BezierCurveExtruderStateData.drawingCurveStrategy.CalculateControlPoint(1, BezierCurveExtruderStateData));
            controlPoints.Add(BezierCurveExtruderStateData.drawingCurveStrategy.CalculateControlPoint(3, BezierCurveExtruderStateData));
            controlPoints.Add(BezierCurveExtruderStateData.drawingCurveStrategy.CalculateControlPoint(2, BezierCurveExtruderStateData));

            // set control points of bezier curve and draw it
            BezierCurveExtruderStateData.BezierCurveSketchObject.SetControlPoints(controlPoints);

            // set tool in use state to true
            BezierCurveExtruder.CurrentBezierCurveExtruderState = new StateCurveView(BezierCurveExtruder, BezierCurveExtruderSettings, BezierCurveExtruderStateData);

            // show or hide indicators for the bezier curve
            ShowIndicatorsHelper(BezierCurveExtruderSettings.showIndicators);
        }

        internal override void Reset()
        {
            Debug.Log("Can not execute method 'ExitTool()': Tool must have been started.");
        }

        internal override void StartExtrusion()
        {
            Debug.Log("Can not execute method 'StartDrawSurface()': Tool must be drawing curve");
        }

        internal override ExtrudedBezierCurveSketchObject StopExtrusion()
        {
            Debug.Log("Can not execute method 'StopDrawSurface()': Tool must be drawing surface.");
            return null;
        }

        internal override void ShowIndicators(bool show)
        {
            Debug.Log("Can not execute method 'ShowIndicators()': Tool must have been started.");
        }

        internal override void ChangeCurveIntensity(BezierCurveExtruder.BezierCurveExtruderController controller, float amount)
        {
            Debug.Log("Can not execute method 'ChangeCurveIntensity()': Tool must have been started.");
        }

        internal override void Update()
        {
        }

        internal override BezierCurveExtruder.BezierCurveExtruderState GetCurrentState()
        {
            return BezierCurveExtruder.BezierCurveExtruderState.Idle;
        }
    }
}