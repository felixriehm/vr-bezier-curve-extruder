using System;
using System.Collections.Generic;
using UnityEditorInternal;
using VRSketchingGeometry.SketchObjectManagement;
using UnityEngine;

namespace VRSketchingGeometry.BezierSurfaceTool
{
    public class BezierSurfaceTool : MonoBehaviour
    {
        [Header("Left Controller")]
        [Range(0, 10)]
        public float bezierCurveLeftStartOffsetDistance = 0.5f;
        public Vector3 bezierCurveLeftStartOffsetRotation = new Vector3(0f,0f,0f);
        public Vector3 bezierCurveLeftStartRotation = new Vector3(0f,0f,0f);
        [Range(0, 10)]
        public float bezierCurveLeftIntensity = 1f;
        [Header("Right Controller")]
        [Range(0, 10)]
        public float bezierCurveRightStartOffsetDistance = 0.5f;
        public Vector3 bezierCurveRightStartOffsetRotation = new Vector3(0f,0f,0f);
        public Vector3 bezierCurveRightStartRotation = new Vector3(0f,0f,0f);
        [Range(0, 10)]
        public float bezierCurveRightIntensity = 1f;
        [Header("Others")]
        public DefaultReferences Defaults;
        public bool showIndicators = false;
        public BezierSurfaceToolState CurrentBezierSurfaceToolState { get; private set; }

        public enum BezierSurfaceToolController
        {
            Left,
            Right
        }
    
        public enum BezierSurfaceToolState
        {
            ToolNotStarted,
            DrawingSurface,
            DrawingCurve
        }
        
        private BezierCurveSketchObject BezierCurveSketchObject;
        private GameObject BezierCurveSketchObjectPrefab;
        private GameObject[] controllerHandles = new GameObject[2];
        private GameObject[] cpHandles = new GameObject[4];
        private BezierPatchSketchObject temporaryBezierPatch;
        private Vector3[] prevCpHandles;
        private List<BezierSurfaceSketchObject> bezierSurfaces;
        private BezierSurfaceSketchObject currentBezierSurface;
        
        public BezierSurfaceTool()
        {
            CurrentBezierSurfaceToolState = BezierSurfaceToolState.ToolNotStarted;
            bezierSurfaces = new List<BezierSurfaceSketchObject>();
        }

        public void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f)
        {
            if (CurrentBezierSurfaceToolState == BezierSurfaceToolState.ToolNotStarted)
            {
                // init arrays for the later for loop
                Transform[] controllerOrigins = new Transform[2];
                controllerOrigins[0] = leftControllerOrigin;
                controllerOrigins[1] = rightControllerOrigin;

                float[] bezierCurveStartOffsetDistance = new float[2];
                bezierCurveStartOffsetDistance[0] = bezierCurveLeftStartOffsetDistance;
                bezierCurveStartOffsetDistance[1] = bezierCurveRightStartOffsetDistance;

                Vector3[] bezierCurveStartOffsetRotation = new Vector3[2];
                bezierCurveStartOffsetRotation[0] = bezierCurveLeftStartOffsetRotation;
                bezierCurveStartOffsetRotation[1] = bezierCurveRightStartOffsetRotation;
                
                Vector3[] bezierCurveStartRotation = new Vector3[2];
                bezierCurveStartRotation[0] = bezierCurveLeftStartRotation;
                bezierCurveStartRotation[1] = bezierCurveRightStartRotation;
                
                float[] bezierCurveIntensity = new float[2];
                bezierCurveIntensity[0] = bezierCurveLeftIntensity;
                bezierCurveIntensity[1] = bezierCurveRightIntensity;

                // create cp handles and controller handle for left and right controller
                for (int i = 0; i < controllerOrigins.Length; i++)
                {
                    // init controller handle
                    Destroy(controllerHandles[i]);
                    controllerHandles[i] = Instantiate(Defaults.BezierSurfaceToolIndicatorPrefab);
                    controllerHandles[i].name = "ControllerHandle" + i;
                    controllerHandles[i].transform.SetParent(controllerOrigins[i]);
                    // init indices for the arrays previously created
                    int firstCpHandle = i * 2;
                    int secondCpHandle = i * 2 + 1;
                    
                    // init first and second cp handle
                    Destroy(cpHandles[firstCpHandle]);
                    Destroy(cpHandles[secondCpHandle]);
                    cpHandles[firstCpHandle] = Instantiate(Defaults.BezierSurfaceToolIndicatorPrefab);
                    cpHandles[firstCpHandle].name = "CpHandle" + firstCpHandle;
                    cpHandles[secondCpHandle] = Instantiate(Defaults.BezierSurfaceToolIndicatorPrefab);
                    cpHandles[secondCpHandle].name = "CpHandle" + secondCpHandle;
                    cpHandles[firstCpHandle].transform.SetParent(controllerHandles[i].transform);
                    cpHandles[secondCpHandle].transform.SetParent(cpHandles[firstCpHandle].transform);
                        
                    // set position and rotation of controller handle
                    controllerHandles[i].transform.SetPositionAndRotation(
                        controllerOrigins[i].transform.position, 
                        controllerOrigins[i].rotation);
                    controllerHandles[i].transform.Rotate(bezierCurveStartOffsetRotation[i]);
                     
                    // set position and rotation of first cp handle
                    cpHandles[firstCpHandle].transform.SetPositionAndRotation((controllerHandles[i].transform.forward * bezierCurveStartOffsetDistance[i]) + controllerHandles[i].transform.position, controllerHandles[i].transform.rotation );
                    cpHandles[firstCpHandle].transform.Rotate(bezierCurveStartRotation[i]);
                    
                    // set position and rotation of second cp handle
                    cpHandles[secondCpHandle].transform.SetPositionAndRotation((cpHandles[firstCpHandle].transform.up * bezierCurveIntensity[i]) + cpHandles[firstCpHandle].transform.position, cpHandles[firstCpHandle].transform.rotation );
                }

                // init Bezier curve
                Destroy(BezierCurveSketchObjectPrefab);
                BezierCurveSketchObjectPrefab = Instantiate(Defaults.BezierCurveSketchObjectPrefab);
                BezierCurveSketchObject = BezierCurveSketchObjectPrefab.GetComponent<BezierCurveSketchObject>();
                BezierCurveSketchObject.SetLineDiameter(diameter);
                BezierCurveSketchObject.SetInterpolationSteps(steps);

                // get control points for the bezier curve
                List<Vector3> controlPoints = new List<Vector3>();
                controlPoints.Add(cpHandles[0].transform.position);
                controlPoints.Add(cpHandles[1].transform.position);
                controlPoints.Add(cpHandles[3].transform.position);
                controlPoints.Add(cpHandles[2].transform.position);

                // set control points of bezier curve and draw it
                BezierCurveSketchObject.SetControlPoints(controlPoints);

                // set tool in use state to true
                CurrentBezierSurfaceToolState = BezierSurfaceToolState.DrawingCurve;
                
                // show or hide indicator spheres
                ShowIndicators(showIndicators);
            }
            else
            {
                Debug.Log("Can not execute method 'StartTool()': Tool has already started.");
                return;
            }
        }

        private void Update()
        {
            if (CurrentBezierSurfaceToolState == BezierSurfaceToolState.DrawingCurve)
            {
                RedrawBezierCurve();
            }

            if (CurrentBezierSurfaceToolState == BezierSurfaceToolState.DrawingSurface)
            {
                RedrawTemporaryBezierPatch();
                
                if (IsTmpBezierPatchMinDistMet())
                {
                    currentBezierSurface.AddPatch(temporaryBezierPatch);
                    for (int i = 0; i < cpHandles.Length; i++)
                    {
                        prevCpHandles[i] = cpHandles[i].transform.position;
                    }
                }
            }
        }

        public void ExitTool()
        {
            if (CurrentBezierSurfaceToolState != BezierSurfaceToolState.ToolNotStarted)
            {
                for (int i = 0; i < controllerHandles.Length; i++)
                {
                    Destroy(controllerHandles[i]);
                }
            
                for (int i = 0; i < cpHandles.Length; i++)
                {
                    Destroy(cpHandles[i]);
                }
            
                Destroy(BezierCurveSketchObjectPrefab);
            
                CurrentBezierSurfaceToolState = BezierSurfaceToolState.ToolNotStarted;
            }
            else
            {
                Debug.Log("Can not execute method 'ExitTool()': Tool must have been started.");
            }
            
        }

        public void ShowIndicators(bool show)
        {
            if (CurrentBezierSurfaceToolState != BezierSurfaceToolState.ToolNotStarted)
            {
                foreach (var controllerHandle in controllerHandles)
                {
                    foreach (var childRenderer in controllerHandle.GetComponentsInChildren<Renderer>())
                    {
                        childRenderer.enabled = show;
                    }
                }
            }
            else
            {
                Debug.Log("Can not execute method 'SetIndicatorsEnabled()': Tool must have been started.");
            }
            
        }
        
        public void StopDrawSurface()
        {
            if (BezierCurveSketchObjectPrefab && CurrentBezierSurfaceToolState == BezierSurfaceToolState.DrawingSurface)
            {
                currentBezierSurface.AddPatch(temporaryBezierPatch);
                Destroy(temporaryBezierPatch.gameObject);
                
                bezierSurfaces.Add(currentBezierSurface);

                BezierCurveSketchObjectPrefab.SetActive(true);
                CurrentBezierSurfaceToolState = BezierSurfaceToolState.DrawingCurve;
            }
            else
            {
                Debug.Log("Can not execute method 'StopDrawSurface()': Tool must be drawing surface or BezierCurveSketchObjectPrefab was not initiated");
            }
        }
        
        public void StartDrawSurface()
        {
            if (BezierCurveSketchObjectPrefab && CurrentBezierSurfaceToolState == BezierSurfaceToolState.DrawingCurve)
            {
                currentBezierSurface = Instantiate(Defaults.BezierSurfaceSketchObjectPrefab).GetComponent<BezierSurfaceSketchObject>();
                prevCpHandles = new Vector3[4];
                for (int i = 0; i < cpHandles.Length; i++)
                {
                    prevCpHandles[i] = cpHandles[i].transform.position;
                }
                
                // TODO: temporaryBezierPatch und bezierPatches werden immer neu erstellt. Muss mit Desotry aufgeräumt werden
                // und in BezierSurfaceSKetchObject gepseichert werden.
                if (temporaryBezierPatch != null)
                {
                    Destroy(temporaryBezierPatch.gameObject);
                }
                
                temporaryBezierPatch = Instantiate(Defaults.BezierPatchSketchObjectPrefab).GetComponent<BezierPatchSketchObject>();
                if (bezierSurfaces != null)
                {
                    foreach (var surface in bezierSurfaces)
                    {
                        if (surface != null)
                        {
                            Destroy(surface.gameObject);
                        }
                    }
                }

                BezierCurveSketchObjectPrefab.SetActive(false);
                CurrentBezierSurfaceToolState = BezierSurfaceToolState.DrawingSurface;
            }
            else
            {
                Debug.Log("Can not execute method 'StartDrawSurface()': Tool must be drawing curve or BezierCurveSketchObjectPrefab was not initiated");
            }
        }

        private bool IsTmpBezierPatchMinDistMet()
        {
            for (int i = 0; i < prevCpHandles.Length; i++)
            {
                if ((prevCpHandles[i] - cpHandles[i].transform.position).magnitude > 0.05f)
                {
                    return true;
                }
            }

            return false;
        }

        private void RedrawTemporaryBezierPatch()
        {
            if(prevCpHandles[0] == cpHandles[0].transform.position &&
               prevCpHandles[1] == cpHandles[1].transform.position &&
               prevCpHandles[2] == cpHandles[2].transform.position &&
               prevCpHandles[3] == cpHandles[3].transform.position)
            {
                // this is needed to avoid the error: "[Physics.PhysX] cleaning the mesh failed"
                // this is happening because all Vertices overlap and that is messing up the cleaning procedures
                return;
            }
            
            temporaryBezierPatch.SetControlPoints(GetTmpBezierPatchCps(),4);
        }
        
        private void RedrawBezierCurve()
        {
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(cpHandles[0].transform.position);
            controlPoints.Add(cpHandles[1].transform.position);
            controlPoints.Add(cpHandles[3].transform.position);
            controlPoints.Add(cpHandles[2].transform.position);
            BezierCurveSketchObject.SetControlPoints(controlPoints);
        }

        private List<Vector3> GetTmpBezierPatchCps()
        {
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(prevCpHandles[0]);
            controlPoints.Add(prevCpHandles[1]);
            controlPoints.Add(prevCpHandles[3]);
            controlPoints.Add(prevCpHandles[2]);
            controlPoints.Add(cpHandles[0].transform.position);
            controlPoints.Add(cpHandles[1].transform.position);
            controlPoints.Add(cpHandles[3].transform.position);
            controlPoints.Add(cpHandles[2].transform.position);
            return controlPoints;
        }

        public void ChangeCurveIntensity(BezierSurfaceToolController controller, float amount)
        {
            if (CurrentBezierSurfaceToolState != BezierSurfaceToolState.ToolNotStarted)
            {
                switch (controller)
                {
                    case BezierSurfaceToolController.Left:
                        cpHandles[1].transform.Translate(0, amount, 0);;
                        break;
                    case BezierSurfaceToolController.Right:
                        cpHandles[3].transform.Translate(0, amount, 0);;
                        break;
                }
            }
            else
            {
                Debug.Log("Can not execute method 'ChangeCurveIntensity()': Tool must have been started.");
            }
        }
    }
}