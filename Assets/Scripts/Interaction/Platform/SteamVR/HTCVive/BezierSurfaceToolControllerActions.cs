using BezierCurveExtrusion;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;
using VRSketchingGeometry;
using VRSketchingGeometry.SketchObjectManagement;

public class BezierSurfaceToolControllerActions : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<BezierCurveExtruder.BezierCurveExtruderState> OnStateChanged;
    [SerializeField]
    private UnityEvent<BezierCurveExtruder.DrawingCurveStrategy> OnStrategyChanged;
    [SerializeField]
    private UnityEvent drawSurfaceClick;
    [SerializeField]
    private SteamVR_LaserPointer laserPointer;
    [SerializeField]
    private SteamVRRaycaster steamVRRaycaster;
    public SteamVR_Input_Sources leftHandType; 
    public SteamVR_Input_Sources rightHandType;
    public SteamVR_Action_Boolean bezierSurfaceToolAction;
    public SteamVR_Action_Boolean drawBezierSurface;
    public SteamVR_Action_Boolean nextCurveStrategy;
    public SteamVR_Action_Boolean lastCurveStrategy;
    public SteamVR_Action_Boolean saveSketchWorld;
    public SteamVR_Action_Boolean loadSketchWorld;
    public SteamVR_Action_Vector2 bezierCurveIntensity;
    public Transform leftControllerOrigin;
    public Transform rightControllerOrigin;
    public SteamVR_ActionSet bezierSurfaceToolActionSet;
    public DefaultReferences Defaults;
    private BezierCurveExtruder bezierCurveExtruder;
    private BezierCurveExtruder.DrawingCurveStrategy[] _curveStrategies = new BezierCurveExtruder.DrawingCurveStrategy[4];
    private int _strategyCounter;
    private SketchWorld sketchWorld;
    private BezierCurveExtruder.DrawingCurveStrategy toolStartCurveStrategy = BezierCurveExtruder.DrawingCurveStrategy.Simple;

    private void Start()
    {
        sketchWorld = Instantiate(Defaults.SketchWorldPrefab).GetComponent<SketchWorld>();
        
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerClick += PointerClick;
        laserPointer.PointerOut += PointerOutside;
        
        bezierCurveExtruder = Instantiate(Defaults.BezierSurfaceToolPrefab).GetComponent<BezierCurveExtruder>();
        bezierCurveExtruder.SetSketchWorld(sketchWorld);
        bezierCurveExtruder.GetOnStateChangedEvent().AddListener((state) => {OnStateChanged.Invoke(state);});
        bezierCurveExtruder.GetOnStrategyChangedEvent().AddListener((strategy) => {OnStrategyChanged.Invoke(strategy);});
        OnStateChanged.Invoke(bezierCurveExtruder.GetCurrentState());

        bezierSurfaceToolAction.AddOnStateDownListener(OnBezierSurfaceToolActionStateDown, leftHandType);
        bezierSurfaceToolAction.AddOnStateDownListener(OnBezierSurfaceToolActionStateDown, rightHandType);
        
        //saveSketchWorld.AddOnStateDownListener(OnSaveSketchWorldActionStateDown, leftHandType);
        //loadSketchWorld.AddOnStateDownListener(OnLoadSketchWorldActionStateDown, rightHandType);
        
        //lastCurveStrategy.AddOnStateDownListener(OnLastCurveStrategyActionStateDown, leftHandType);
        //nextCurveStrategy.AddOnStateDownListener(OnNextCurveStrategyActionStateDown, rightHandType);
        
        drawBezierSurface.AddOnStateDownListener(OnDrawBezierSurfaceStateDownAction, leftHandType);
        drawBezierSurface.AddOnStateDownListener(OnDrawBezierSurfaceStateDownAction, rightHandType);
        
        drawBezierSurface.AddOnStateUpListener(OnDrawBezierSurfaceStateUpAction, leftHandType);
        drawBezierSurface.AddOnStateUpListener(OnDrawBezierSurfaceStateUpAction, rightHandType);
        
        bezierCurveIntensity.AddOnChangeListener(OnBezierCurveIntensityChangeAction, leftHandType);
        bezierCurveIntensity.AddOnChangeListener(OnBezierCurveIntensityChangeAction, rightHandType);

        _strategyCounter = 4;
        _curveStrategies[0] = BezierCurveExtruder.DrawingCurveStrategy.Simple;
        _curveStrategies[1] = BezierCurveExtruder.DrawingCurveStrategy.VectorAngle;
        _curveStrategies[2] = BezierCurveExtruder.DrawingCurveStrategy.RotationAngle;
        _curveStrategies[3] = BezierCurveExtruder.DrawingCurveStrategy.Distance;
        
        Teleport.instance.CancelTeleportHint();
    }

    private void OnBezierCurveIntensityChangeAction(SteamVR_Action_Vector2 fromaction, SteamVR_Input_Sources fromsource, Vector2 axis, Vector2 delta)
    {
        BezierCurveExtruder.BezierCurveExtruderController controller =  fromsource == leftHandType ? 
            BezierCurveExtruder.BezierCurveExtruderController.Left : BezierCurveExtruder.BezierCurveExtruderController.Right;
        
        if (axis.y > 0.9)
        {
            bezierCurveExtruder.ChangeCurveIntensity(controller, 0.05f);
        }
        if (axis.y < -0.9)
        {
            bezierCurveExtruder.ChangeCurveIntensity(controller, -0.05f);
        }
    }

    private void OnDrawBezierSurfaceStateDownAction(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        if (drawBezierSurface.GetState(leftHandType) && fromsource == rightHandType || 
            fromsource == leftHandType && drawBezierSurface.GetState(rightHandType))
        {
            //Debug.Log("BezierSurfaceTool: drawing");
            bezierCurveExtruder.StartDrawSurface();
            drawSurfaceClick.Invoke();
        }
    }
    
    private void OnDrawBezierSurfaceStateUpAction(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        //Debug.Log("BezierSurfaceTool: not drawing");
        bezierCurveExtruder.StopDrawSurface();
    }

    private void OnBezierSurfaceToolActionStateDown(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        if (bezierCurveExtruder.GetCurrentState() == BezierCurveExtruder.BezierCurveExtruderState.Idle)
        {
            //Debug.Log("BezierSurfaceTool activated");
            bezierSurfaceToolActionSet.Activate();
            bezierCurveExtruder.StartTool(leftControllerOrigin, rightControllerOrigin, 24, 0.02f, toolStartCurveStrategy);
            laserPointer.enabled = false;
            steamVRRaycaster.enabled = false;
            laserPointer.holder.SetActive(false);
        }
        else
        {
            //Debug.Log("BezierSurfaceTool deactivated");
            bezierSurfaceToolActionSet.Deactivate();
            bezierCurveExtruder.ExitTool();
            laserPointer.enabled = true;
            steamVRRaycaster.enabled = true;
            laserPointer.holder.SetActive(true);
        }
    }
    
    private void OnNextCurveStrategyActionStateDown(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        //Debug.Log("BezierSurfaceTool curve strategy changed");
        _strategyCounter++;
        bezierCurveExtruder.SetDrawingCurveStrategy(_curveStrategies[_strategyCounter%(_curveStrategies.Length)]);
    }
    
    private void OnLastCurveStrategyActionStateDown(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        //Debug.Log("BezierSurfaceTool curve strategy changed");
        _strategyCounter--;
        bezierCurveExtruder.SetDrawingCurveStrategy(_curveStrategies[_strategyCounter%(_curveStrategies.Length)]);
        if (_strategyCounter <= 0)
        {
            _strategyCounter = 4;
        }
    }
    
    private void OnSaveSketchWorldActionStateDown(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        if (bezierCurveExtruder.GetCurrentState() == BezierCurveExtruder.BezierCurveExtruderState.Idle)
        {
            string savePath = System.IO.Path.Combine(Application.dataPath, "serialization\\BezierSurfaceTool.xml");
            sketchWorld.SaveSketchWorld(savePath);
        }
    }
    
    private void OnLoadSketchWorldActionStateDown(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
    {
        if (bezierCurveExtruder.GetCurrentState() == BezierCurveExtruder.BezierCurveExtruderState.Idle)
        {
            string load = System.IO.Path.Combine(Application.dataPath, "serialization\\BezierSurfaceTool.xml");
            sketchWorld.LoadSketchWorld(load);
        }
    }
    
    private void PointerClick(object sender, PointerEventArgs e)
    {
        if (e.target.name == "BezierSurface")
        {
            Destroy(e.target.gameObject);
        }
    }

    private void PointerInside(object sender, PointerEventArgs e)
    {
        if (e.target.name == "BezierSurface")
        {
            ExtrudedBezierCurveSketchObject extrudedBezierCurve = e.target.gameObject.GetComponent<ExtrudedBezierCurveSketchObject>();
            extrudedBezierCurve.highlight();
        }
    }
    
    public void PointerOutside(object sender, PointerEventArgs e)
    {
        if (e.target.name == "BezierSurface")
        {
            ExtrudedBezierCurveSketchObject extrudedBezierCurve = e.target.gameObject.GetComponent<ExtrudedBezierCurveSketchObject>();
            extrudedBezierCurve.revertHighlight();
        }
    }

    public SketchWorld GetSketchWorld()
    {
        return sketchWorld;
    }

    public SketchWorld CreateNewSketchWorld()
    {
        Destroy(sketchWorld.gameObject);
        sketchWorld = Instantiate(Defaults.SketchWorldPrefab).GetComponent<SketchWorld>();
        bezierCurveExtruder.SetSketchWorld(sketchWorld);
        return sketchWorld;
    }

    public void SetToolStartCurveStrategy(BezierCurveExtruder.DrawingCurveStrategy drawingCurveStrategy)
    {
        toolStartCurveStrategy = drawingCurveStrategy;
    }
}
