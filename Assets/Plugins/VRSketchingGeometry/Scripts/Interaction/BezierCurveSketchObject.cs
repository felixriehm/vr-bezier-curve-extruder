//-----------------------------------------------------------------------
//
// Original repository: https://github.com/tterpi/VRSketchingGeometry
//
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Serialization;
using System.Linq;
using VRSketchingGeometry.BezierSurfaceTool;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// Provides methods to interact with a line game object in the scene.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class BezierCurveSketchObject : LineSketchObject, ISerializableComponent, IBrushable
    {
        protected override void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            SplineMesh = new SplineMesh(new BezierCurve(InterpolationSteps), Vector3.one * lineDiameter);
            LinearSplineMesh = new SplineMesh(new LinearInterpolationSpline(), Vector3.one * lineDiameter);

            meshCollider.sharedMesh = meshFilter.sharedMesh;
            setUpOriginalMaterialAndMeshRenderer();
        }

        /// <summary>
        /// Set the number of interpolation steps between two control points.
        /// A higher number makes the line smoother.
        /// </summary>
        /// <param name="steps"></param>
        public override void SetInterpolationSteps(int steps) {
            InterpolationSteps = steps;
            List<Vector3> controlPoints = this.GetControlPoints();
            this.SplineMesh.GetCrossSectionShape(out List<Vector3> CurrentCrossSectionShape, out List<Vector3> CurrentCrossSectionNormals);
            SplineMesh = new SplineMesh(new BezierCurve(steps), this.lineDiameter * Vector3.one);
            this.SetLineCrossSection(CurrentCrossSectionShape, CurrentCrossSectionNormals, this.lineDiameter);
            if (controlPoints.Count != 0) {
                this.SetControlPointsLocalSpace(controlPoints);
            }
        }
    }
}