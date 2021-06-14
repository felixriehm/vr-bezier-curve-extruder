using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Splines;

namespace VRSketchingGeometry.BezierSurfaceTool
{
    public class BezierCurveMesh : SplineMesh
    {
        public BezierCurveMesh(Spline spline) : base(spline)
        {
        }

        public BezierCurveMesh(Spline spline, Vector3 crossSectionScale, int crossSectionResolution = 6) : base(spline, crossSectionScale, crossSectionResolution)
        {
            List<Vector3> vertices = CircularCrossSection.GenerateVertices(crossSectionResolution);
            List<Vector3> crossSectionShape = vertices;
            List<Vector3> crossSectionShapeNormals = new List<Vector3>();
            foreach (Vector3 point in crossSectionShape)
            {
                crossSectionShapeNormals.Add(point.normalized);
            }

            Spline = spline;
            
            TubeMesh = new ParallelTransportTubeMesh(new CrossSection(crossSectionShape, crossSectionShapeNormals, crossSectionScale));
        }

        public BezierCurveMesh(Spline spline, ITubeMesh tubeMesh) : base(spline, tubeMesh)
        {
        }
    }
}