using UnityEngine;
using UnityEngine.Events;
using BezierCurveExtrusion.Strategy;
using VRSketchingGeometry.SketchObjectManagement;

namespace BezierCurveExtrusion.State
{
    internal class BezierCurveExtruderStateData
    {
        public BezierCurveSketchObject BezierCurveSketchObject;
        public GameObject[] cpHandles = new GameObject[4];
        public ExtrudedBezierCurveSketchObject CurrentExtrudedBezierCurve;
        public BezierPatchSketchObject temporaryBezierPatch;
        public Vector3[] prevCpHandles;
        public GameObject[] supplementaryCpHandles = new GameObject[2];
        public GameObject[] controllerHandles = new GameObject[2];
        public IDrawingCurveStrategy drawingCurveStrategy;
        public UnityEvent<BezierCurveExtruder.BezierCurveExtruderState> OnStateChanged = new UnityEvent<BezierCurveExtruder.BezierCurveExtruderState>();
        public UnityEvent<BezierCurveExtruder.DrawingCurveStrategy> OnStrategyChanged = new UnityEvent<BezierCurveExtruder.DrawingCurveStrategy>();
    }
}