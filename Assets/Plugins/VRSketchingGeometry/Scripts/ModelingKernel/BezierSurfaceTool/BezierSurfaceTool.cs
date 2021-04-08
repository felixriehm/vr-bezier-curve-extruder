using System.Collections.Generic;
using VRSketchingGeometry.SketchObjectManagement;
using UnityEngine;

namespace VRSketchingGeometry.BezierSurfaceTool
{
    public class BezierSurfaceTool : MonoBehaviour
    {
        private BezierCurveSketchObject BezierCurveSketchObject;
        private GameObject BezierCurveSketchObjectPrefab;
        private GameObject[] controllerHandles = new GameObject[2];
        private GameObject[] cpHandles = new GameObject[4];
        public DefaultReferences Defaults;

        public void InitInitialBezierCurve(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f)
        {
            Transform[] controllerOrigins = new Transform[2];
            controllerOrigins[0] = leftControllerOrigin;
            controllerOrigins[1] = rightControllerOrigin;

            for (int i = 0; i < controllerOrigins.Length; i++)
            {
                Destroy(controllerHandles[i]);
                controllerHandles[i] = Instantiate(Defaults.BezierSurfaceToolIndicatorPrefab);
                controllerHandles[i].name = "ControllerHandle" + i;
                controllerHandles[i].transform.SetParent(controllerOrigins[i]);
                int firstCpHandle = i * 2;
                int secondCpHandle = i * 2 + 1;
                
                Destroy(cpHandles[firstCpHandle]);
                Destroy(cpHandles[secondCpHandle]);
                cpHandles[firstCpHandle] = Instantiate(Defaults.BezierSurfaceToolIndicatorPrefab);
                cpHandles[firstCpHandle].name = "CpHandle" + firstCpHandle;
                cpHandles[secondCpHandle] = Instantiate(Defaults.BezierSurfaceToolIndicatorPrefab);
                cpHandles[secondCpHandle].name = "CpHandle" + secondCpHandle;
                cpHandles[firstCpHandle].transform.SetParent(controllerHandles[i].transform);
                cpHandles[secondCpHandle].transform.SetParent(cpHandles[firstCpHandle].transform);
                    
                controllerHandles[i].transform.SetPositionAndRotation(
                    controllerOrigins[i].transform.position, 
                    controllerOrigins[i].rotation);
                controllerHandles[i].transform.Rotate(0,0,0);
                 
                cpHandles[firstCpHandle].transform.SetPositionAndRotation((controllerHandles[i].transform.forward * 2) + controllerHandles[i].transform.position, controllerHandles[i].transform.rotation );
                cpHandles[firstCpHandle].transform.Rotate(0,0,0);
                
                cpHandles[secondCpHandle].transform.SetPositionAndRotation((cpHandles[firstCpHandle].transform.up * 4) + cpHandles[firstCpHandle].transform.position, cpHandles[firstCpHandle].transform.rotation );
            }

            Destroy(BezierCurveSketchObjectPrefab);
            BezierCurveSketchObjectPrefab = Instantiate(Defaults.BezierCurveSketchObjectPrefab);
            BezierCurveSketchObject = BezierCurveSketchObjectPrefab.GetComponent<BezierCurveSketchObject>();
            BezierCurveSketchObject.SetLineDiameter(diameter);
            BezierCurveSketchObject.SetInterpolationSteps(steps);

            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(cpHandles[0].transform.position);
            controlPoints.Add(cpHandles[1].transform.position);
            controlPoints.Add(cpHandles[3].transform.position);
            controlPoints.Add(cpHandles[2].transform.position);

            BezierCurveSketchObject.SetControlPoints(controlPoints);
        }

        public void DestroyBezierCurve()
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
        }

        public void EnableBezierCurve()
        {
            if (BezierCurveSketchObjectPrefab)
            {
                BezierCurveSketchObjectPrefab.SetActive(true);
            }
        }
        
        public void DisableBezierCurve()
        {
            if (BezierCurveSketchObjectPrefab)
            { 
                BezierCurveSketchObjectPrefab.SetActive(false);
            }
        }

        public void redrawInitialBezierCurve()
        {
            List<Vector3> controlPoints = new List<Vector3>();
            controlPoints.Add(cpHandles[0].transform.position);
            controlPoints.Add(cpHandles[1].transform.position);
            controlPoints.Add(cpHandles[3].transform.position);
            controlPoints.Add(cpHandles[2].transform.position);
            BezierCurveSketchObject.SetControlPoints(controlPoints);
        }

        public void drawBezierSurface(int surfaceResolution)
        {
            
        }
    }
}