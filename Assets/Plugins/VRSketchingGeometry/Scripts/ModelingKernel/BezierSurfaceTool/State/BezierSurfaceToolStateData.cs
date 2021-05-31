using UnityEngine;
using UnityEngine.Events;
using VRSketchingGeometry.BezierSurfaceTool.Strategy;
using VRSketchingGeometry.Serialization;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.BezierSurfaceTool.State
{
    internal class BezierSurfaceToolStateData
    {
        public BezierCurveSketchObject BezierCurveSketchObject;
        public GameObject[] cpHandles = new GameObject[4];
        public BezierSurfaceSketchObject currentBezierSurface;
        public BezierPatchSketchObject temporaryBezierPatch;
        public ISerializableComponent tmpBPSerializableComp;
        public Vector3[] prevCpHandles;
        public GameObject[] supplementaryCpHandles = new GameObject[2];
        public GameObject[] controllerHandles = new GameObject[2];
        public DrawingCurveStrategy drawingCurveStrategy;
        public UnityEvent<BezierSurfaceTool.BezierSurfaceToolState> OnStateChanged = new UnityEvent<BezierSurfaceTool.BezierSurfaceToolState>();
        public UnityEvent<BezierSurfaceTool.DrawingCurveStrategy> OnStrategyChanged = new UnityEvent<BezierSurfaceTool.DrawingCurveStrategy>();
    }
}