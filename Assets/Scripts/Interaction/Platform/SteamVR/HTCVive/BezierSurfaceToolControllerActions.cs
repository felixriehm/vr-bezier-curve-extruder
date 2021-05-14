using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using VRSketchingGeometry;
using VRSketchingGeometry.BezierSurfaceTool;

public class BezierSurfaceToolControllerActions : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<BezierSurfaceTool.BezierSurfaceToolState> OnStateChanged;
    [SerializeField]
    private UnityEvent<BezierSurfaceTool.DrawingCurveStrategy> OnStrategyChanged;
    [SerializeField]
    private GameObject ui;
    public SteamVR_Input_Sources leftHandType; 
    public SteamVR_Input_Sources rightHandType;
    public SteamVR_Action_Boolean bezierSurfaceToolAction;
    public SteamVR_Action_Boolean drawBezierSurface;
    public SteamVR_Action_Boolean nextCurveStrategy;
    public SteamVR_Action_Boolean lastCurveStrategy;
    public SteamVR_Action_Vector2 bezierCurveIntensity;
    public Transform leftControllerOrigin;
    public Transform rightControllerOrigin;
    public SteamVR_ActionSet bezierSurfaceToolActionSet;
    public DefaultReferences Defaults;
    private BezierSurfaceTool bezierSurfaceTool;
    private BezierSurfaceTool.DrawingCurveStrategy[] _curveStrategies = new BezierSurfaceTool.DrawingCurveStrategy[4];
    private int _strategyCounter;

    private void Start()
    {
        bezierSurfaceTool = Instantiate(Defaults.BezierSurfaceToolPrefab).GetComponent<BezierSurfaceTool>();
        bezierSurfaceTool.GetOnStateChangedEvent().AddListener((state) => {OnStateChanged.Invoke(state);});
        bezierSurfaceTool.GetOnStrategyChangedEvent().AddListener((strategy) => {OnStrategyChanged.Invoke(strategy);});
        OnStateChanged.Invoke(bezierSurfaceTool.GetCurrentState());

        bezierSurfaceToolAction.AddOnStateDownListener(OnBezierSurfaceToolActionStateDown, leftHandType);
        bezierSurfaceToolAction.AddOnStateDownListener(OnBezierSurfaceToolActionStateDown, rightHandType);
        
        lastCurveStrategy.AddOnStateDownListener(OnLastCurveStrategyActionStateDown, leftHandType);
        nextCurveStrategy.AddOnStateDownListener(OnNextCurveStrategyActionStateDown, rightHandType);
        
        drawBezierSurface.AddOnStateDownListener(OnDrawBezierSurfaceStateDownAction, leftHandType);
        drawBezierSurface.AddOnStateDownListener(OnDrawBezierSurfaceStateDownAction, rightHandType);
        
        drawBezierSurface.AddOnStateUpListener(OnDrawBezierSurfaceStateUpAction, leftHandType);
        drawBezierSurface.AddOnStateUpListener(OnDrawBezierSurfaceStateUpAction, rightHandType);
        
        bezierCurveIntensity.AddOnChangeListener(OnBezierCurveIntensityChangeAction, leftHandType);
        bezierCurveIntensity.AddOnChangeListener(OnBezierCurveIntensityChangeAction, rightHandType);

        _strategyCounter = 4;
        _curveStrategies[0] = BezierSurfaceTool.DrawingCurveStrategy.Simple;
        _curveStrategies[1] = BezierSurfaceTool.DrawingCurveStrategy.VectorAngle;
        _curveStrategies[2] = BezierSurfaceTool.DrawingCurveStrategy.RotationAngle;
        _curveStrategies[3] = BezierSurfaceTool.DrawingCurveStrategy.Distance;
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
            ui.SetActive(true);
        }
        else
        {
            //Debug.Log("BezierSurfaceTool deactivated");
            bezierSurfaceToolActionSet.Deactivate();
            bezierSurfaceTool.ExitTool();
            ui.SetActive(false);
        }
    }
    
    private void OnNextCurveStrategyActionStateDown(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        //Debug.Log("BezierSurfaceTool curve strategy changed");
        _strategyCounter++;
        bezierSurfaceTool.SetDrawingCurveStrategy(_curveStrategies[_strategyCounter%(_curveStrategies.Length)]);
    }
    
    private void OnLastCurveStrategyActionStateDown(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        //Debug.Log("BezierSurfaceTool curve strategy changed");
        _strategyCounter--;
        bezierSurfaceTool.SetDrawingCurveStrategy(_curveStrategies[_strategyCounter%(_curveStrategies.Length)]);
        if (_strategyCounter <= 0)
        {
            _strategyCounter = 4;
        }
    }
}
