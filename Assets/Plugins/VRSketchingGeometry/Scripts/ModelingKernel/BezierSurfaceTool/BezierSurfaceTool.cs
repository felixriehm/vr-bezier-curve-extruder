using System;
using System.Collections.Generic;
using UnityEditorInternal;
using VRSketchingGeometry.SketchObjectManagement;
using UnityEngine;

namespace VRSketchingGeometry.BezierSurfaceTool
{
    // TODO: wie BezierSurfaces speichern und löschen
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
        [Header("Bezier Surface")]
        public GameObject BezierPatchSketchObjectPrefab;
        public GameObject BezierSurfaceSketchObjectPrefab;
        [Range(0.002f, 4)]
        public float BezierPatchMinDistance = 0.05f;
        [Header("Bezier Curve")]
        public GameObject BezierCurveSketchObjectPrefab;
        public GameObject BezierSurfaceToolIndicatorPrefab;
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
        private GameObject[] controllerHandles = new GameObject[2];
        private GameObject[] cpHandles = new GameObject[4];
        private BezierPatchSketchObject temporaryBezierPatch;
        private Vector3[] prevCpHandles;
        private BezierSurfaceSketchObject currentBezierSurface;
        
        public BezierSurfaceTool()
        {
            CurrentBezierSurfaceToolState = BezierSurfaceToolState.ToolNotStarted;
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
                    controllerHandles[i] = Instantiate(BezierSurfaceToolIndicatorPrefab);
                    controllerHandles[i].name = "ControllerHandle" + i;
                    controllerHandles[i].transform.SetParent(controllerOrigins[i]);
                    // init indices for the arrays previously created
                    int firstCpHandle = i * 2;
                    int secondCpHandle = i * 2 + 1;
                    
                    // init first and second cp handle
                    cpHandles[firstCpHandle] = Instantiate(BezierSurfaceToolIndicatorPrefab);
                    cpHandles[firstCpHandle].name = "CpHandle" + firstCpHandle;
                    cpHandles[secondCpHandle] = Instantiate(BezierSurfaceToolIndicatorPrefab);
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
                BezierCurveSketchObject = Instantiate(BezierCurveSketchObjectPrefab).GetComponent<BezierCurveSketchObject>();
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
                    // Add temporary patch to surface by combining meshes.
                    // There is no check for 'AllCounterPartVerticesAreEqual()' because 'BezierPatchMinDistance' ensures
                    // that this can no be the case.
                    currentBezierSurface.AddPatch(temporaryBezierPatch);
                    
                    // save current hold bezier curve so it can be used later to continuously draw the temporary bezier patch
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
            
                Destroy(BezierCurveSketchObject.gameObject);
                // if you exit the tool while drawing, 'StopDrawSurface()' does not destroy temporaryBezierPatch
                if (temporaryBezierPatch != null)
                {
                    Destroy(temporaryBezierPatch.gameObject);
                }
                
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
            if (CurrentBezierSurfaceToolState == BezierSurfaceToolState.DrawingSurface)
            {
                if (!AllCounterpartVerticesAreEqual())
                {
                    currentBezierSurface.AddPatch(temporaryBezierPatch);
                }
                Destroy(temporaryBezierPatch.gameObject);

                BezierCurveSketchObject.gameObject.SetActive(true);
                CurrentBezierSurfaceToolState = BezierSurfaceToolState.DrawingCurve;
            }
            else
            {
                Debug.Log("Can not execute method 'StopDrawSurface()': Tool must be drawing surface.");
            }
        }
        
        public void StartDrawSurface()
        {
            if (CurrentBezierSurfaceToolState == BezierSurfaceToolState.DrawingCurve)
            {
                // init bezier surface so it can be used later to continuously add temporary bezier patches
                currentBezierSurface = Instantiate(BezierSurfaceSketchObjectPrefab).GetComponent<BezierSurfaceSketchObject>();
                
                // save current hold bezier curve so it can be used later to continuously draw the temporary bezier patch
                prevCpHandles = new Vector3[4];
                for (int i = 0; i < cpHandles.Length; i++)
                {
                    prevCpHandles[i] = cpHandles[i].transform.position;
                }
                
                // init temporary bezier patch so it can be continuously be drawn later and added to the bezier surface
                temporaryBezierPatch = Instantiate(BezierPatchSketchObjectPrefab).GetComponent<BezierPatchSketchObject>();

                // deactivate bezier curve
                BezierCurveSketchObject.gameObject.SetActive(false);
                
                // set state to 'drawing surface'
                CurrentBezierSurfaceToolState = BezierSurfaceToolState.DrawingSurface;
            }
            else
            {
                Debug.Log("Can not execute method 'StartDrawSurface()': Tool must be drawing curve");
            }
        }

        private bool IsTmpBezierPatchMinDistMet()
        {
            for (int i = 0; i < prevCpHandles.Length; i++)
            {
                if ((prevCpHandles[i] - cpHandles[i].transform.position).magnitude > BezierPatchMinDistance)
                {
                    return true;
                }
            }

            return false;
        }

        private void RedrawTemporaryBezierPatch()
        {
            if(AllCounterpartVerticesAreEqual())
            {
                // this is needed to avoid the error: "[Physics.PhysX] cleaning the mesh failed"
                // this is happening because all Vertices overlap and that is messing up the cleaning procedures
                return;
            }
            
            temporaryBezierPatch.SetControlPoints(GetTmpBezierPatchCps(),4);
        }

        private bool AllCounterpartVerticesAreEqual()
        {
            return prevCpHandles[0] == cpHandles[0].transform.position &&
                   prevCpHandles[1] == cpHandles[1].transform.position &&
                   prevCpHandles[2] == cpHandles[2].transform.position &&
                   prevCpHandles[3] == cpHandles[3].transform.position;
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