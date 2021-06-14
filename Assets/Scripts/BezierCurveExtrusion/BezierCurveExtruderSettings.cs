using UnityEngine;
using VRSketchingGeometry;

namespace BezierCurveExtrusion
{
    [CreateAssetMenu(fileName = "BezierCurveExtruderSettings", menuName = "ScriptableObjects/BezierCurveExtruderSettings", order = 2)]
    public class BezierCurveExtruderSettings : ScriptableObject
    {
        public DefaultReferences Defaults;
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
        [Header("Bezier Patch")]
        [Range(0.002f, 4)]
        public float BezierPatchMinDistance = 0.02f;
        [Header("Bezier Curve")]
        public GameObject BezierSurfaceToolIndicatorPrefab;
        public bool showIndicators = false;
    }
}