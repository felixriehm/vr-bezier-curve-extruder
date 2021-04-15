using UnityEngine;
using Valve.VR;
using VRSketchingGeometry;
using VRSketchingGeometry.BezierSurfaceTool;

public class BezierSurfaceToolControllerActions : MonoBehaviour
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

    private void Start()
    {
        bezierSurfaceTool = Instantiate(Defaults.BezierSurfaceToolPrefab).GetComponent<BezierSurfaceTool>();
        
        bezierSurfaceToolAction.AddOnStateDownListener(OnBezierSurfaceToolActionStateDown, leftHandType);
        bezierSurfaceToolAction.AddOnStateDownListener(OnBezierSurfaceToolActionStateDown, rightHandType);
        
        drawBezierSurface.AddOnStateDownListener(OnDrawBezierSurfaceStateDownAction, leftHandType);
        drawBezierSurface.AddOnStateDownListener(OnDrawBezierSurfaceStateDownAction, rightHandType);
        
        drawBezierSurface.AddOnStateUpListener(OnDrawBezierSurfaceStateUpAction, leftHandType);
        drawBezierSurface.AddOnStateUpListener(OnDrawBezierSurfaceStateUpAction, rightHandType);
        
        bezierCurveIntensity.AddOnChangeListener(OnBezierCurveIntensityChangeAction, leftHandType);
        bezierCurveIntensity.AddOnChangeListener(OnBezierCurveIntensityChangeAction, rightHandType);
    }

    private void OnBezierCurveIntensityChangeAction(SteamVR_Action_Vector2 fromaction, SteamVR_Input_Sources fromsource, Vector2 axis, Vector2 delta)
    {
        BezierSurfaceTool.BezierSurfaceToolController controller =  fromsource == leftHandType ? 
            BezierSurfaceTool.BezierSurfaceToolController.Left : BezierSurfaceTool.BezierSurfaceToolController.Right;
        
        if (axis.y > 0.9)
        {
            bezierSurfaceTool.ChangeCurveIntensity(controller, 0.05f);
        }
        if (axis.y < -0.9)
        {
            bezierSurfaceTool.ChangeCurveIntensity(controller, -0.05f);
        }
    }

    private void OnDrawBezierSurfaceStateDownAction(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        if (drawBezierSurface.GetState(leftHandType) && fromsource == rightHandType || 
            fromsource == leftHandType && drawBezierSurface.GetState(rightHandType))
        {
            //Debug.Log("BezierSurfaceTool: drawing");
            bezierSurfaceTool.StartDrawSurface();
        }
    }
    
    private void OnDrawBezierSurfaceStateUpAction(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        //Debug.Log("BezierSurfaceTool: not drawing");
        bezierSurfaceTool.StopDrawSurface();
    }

    private void OnBezierSurfaceToolActionStateDown(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        if (bezierSurfaceTool.GetCurrentState() == BezierSurfaceTool.BezierSurfaceToolState.ToolNotStarted)
        {
            //Debug.Log("BezierSurfaceTool activated");
            bezierSurfaceToolActionSet.Activate();
            bezierSurfaceTool.StartTool(leftControllerOrigin, rightControllerOrigin);
        }
        else
        {
            //Debug.Log("BezierSurfaceTool deactivated");
            bezierSurfaceToolActionSet.Deactivate();
            bezierSurfaceTool.ExitTool();
        }
    }
}
