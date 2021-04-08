using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using VRSketchingGeometry;
using VRSketchingGeometry.BezierSurfaceTool;
using VRSketchingGeometry.SketchObjectManagement;

public class ControllerActions : MonoBehaviour
{
    public SteamVR_Input_Sources leftHandType; 
    public SteamVR_Input_Sources rightHandType;
    public SteamVR_Action_Boolean bezierSurfaceToolAction;
    public SteamVR_Action_Boolean drawBezierSurface;
    public SteamVR_Action_Vector2 bezierCurveIntensity;
    public Transform leftControllerOrigin;
    public Transform rightControllerOrigin;
    public SteamVR_ActionSet bezierSurfaceToolActionSet;
    public DefaultReferences Defaults;
    private BezierSurfaceTool bezierSurfaceTool;
    private bool bezierSurfaceToolInUse;
    private bool bezierSurfaceToolIsDrawing;

    private void Start()
    {
        bezierSurfaceTool = Instantiate(Defaults.BezierSurfaceToolPrefab).GetComponent<BezierSurfaceTool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bezierSurfaceToolAction.GetStateDown(leftHandType) ||bezierSurfaceToolAction.GetStateDown(rightHandType))
        {
            bezierSurfaceToolInUse = !bezierSurfaceToolInUse;
            if (bezierSurfaceToolInUse)
            {
                //Debug.Log("BezierSurfaceTool activated");
                bezierSurfaceToolInUse = true;
                
                bezierSurfaceToolActionSet.Activate();

                bezierSurfaceTool.InitInitialBezierCurve(leftControllerOrigin, rightControllerOrigin);
            }
            else
            {
                //Debug.Log("BezierSurfaceTool deactivated");
                bezierSurfaceToolInUse = false;
                bezierSurfaceToolActionSet.Deactivate();
                bezierSurfaceTool.DestroyBezierCurve();
            }
            
        }

        if (bezierSurfaceToolInUse)
        {
            if (drawBezierSurface.GetState(leftHandType) && drawBezierSurface.GetStateDown(rightHandType) || 
                drawBezierSurface.GetStateDown(leftHandType) && drawBezierSurface.GetState(rightHandType))
            {
                //Debug.Log("BezierSurfaceTool: drawing");
                bezierSurfaceTool.DisableBezierCurve();
                bezierSurfaceToolIsDrawing = true;
            }
            
            if (drawBezierSurface.GetStateUp(leftHandType) || drawBezierSurface.GetStateUp(rightHandType))
            {
                //Debug.Log("BezierSurfaceTool: not drawing");
                bezierSurfaceTool.EnableBezierCurve();
                bezierSurfaceToolIsDrawing = false;
            }
            
            if (!bezierSurfaceToolIsDrawing)
            {
                // draw bezier curve on each update
                bezierSurfaceTool.redrawInitialBezierCurve();

                if (bezierCurveIntensity.GetAxis(leftHandType).y > 0.9)
                {
                    // increase vector length by 0.1
                    //Debug.Log("BezierSurfaceTool: left curve intensity increased");
                    //Debug.Log(bezierCurveIntensity.GetAxis(leftHandType));
                }
                if (bezierCurveIntensity.GetAxis(leftHandType).y < -0.9)
                {
                    // decrease vector length by 0.1
                    //Debug.Log("BezierSurfaceTool: left curve intensity decreased");
                    //Debug.Log(bezierCurveIntensity.GetAxis(leftHandType));
                }

                if (bezierCurveIntensity.GetAxis(rightHandType).y > 0.9)
                {
                    // increase vector length by 0.1
                    //Debug.Log("BezierSurfaceTool: right curve intensity");
                    //Debug.Log(bezierCurveIntensity.GetAxis(rightHandType));
                }

                if (bezierCurveIntensity.GetAxis(rightHandType).y < -0.9)
                {
                    // decrease vector length by 0.1
                    //Debug.Log("BezierSurfaceTool: right curve detensity");
                    //Debug.Log(bezierCurveIntensity.GetAxis(rightHandType));
                }
            }
            else
            {
                // draw bezierfurface on each update
            }
        }
    }
}
