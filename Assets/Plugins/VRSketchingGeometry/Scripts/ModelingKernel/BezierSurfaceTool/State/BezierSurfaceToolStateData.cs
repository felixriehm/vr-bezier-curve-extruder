using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.BezierSurfaceTool.State
{
    internal class BezierSurfaceToolStateData
    {
        public BezierCurveSketchObject BezierCurveSketchObject;
        public GameObject[] cpHandles = new GameObject[4];
        public BezierSurfaceSketchObject currentBezierSurface;
        public BezierPatchSketchObject temporaryBezierPatch;
        public Vector3[] prevCpHandles;
        public GameObject[] controllerHandles = new GameObject[2];
    }
}