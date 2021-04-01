using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Splines;

namespace VRSketchingGeometry.BezierSurfaceTool
{
    public class BezierPatchMesh
    {
        public static Vector3[] GenerateVerticesOfPatch(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight)
        {
            return GenerateVerticesOfPatch_Parallel(controlPoints, width, height, resolutionWidth, resolutionHeight);
        }
        
        internal static Vector3[] GenerateVerticesOfPatch_Parallel(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight)
        {
            //create horizontal splines through the control points
            List<Vector3>[] horizontalPoints = new List<Vector3>[height];
            Parallel.For(0, height, (i) =>
            {
                BezierCurve horizontalSpline = new BezierCurve(resolutionWidth);
                horizontalSpline.SetControlPoints(controlPoints.GetRange(i * width, width).ToArray());
                horizontalPoints[i] = new List<Vector3>(horizontalSpline.InterpolatedPoints);
            });

            //create vertical splines through the generated interpolated points of the horizontal splines
            List<Vector3> vertices = new List<Vector3>();

            List<Vector3>[] verticesLists = new List<Vector3>[resolutionWidth];
            Parallel.For(0, resolutionWidth, (i) => {
                BezierCurve verticalSpline = new BezierCurve(resolutionHeight);
                List<Vector3> verticalControlPoints = new List<Vector3>();
                foreach (List<Vector3> horizontalPointList in horizontalPoints)
                {
                    verticalControlPoints.Add(horizontalPointList[i]);
                }
                verticalSpline.SetControlPoints(verticalControlPoints.ToArray());

                verticesLists[i] = new List<Vector3>(verticalSpline.InterpolatedPoints);
            });

            foreach (var verticesList in verticesLists)
            {
                vertices.AddRange(verticesList);
            }

            return vertices.ToArray();
        }
        
        public static Mesh GeneratePatchMesh(List<Vector3> controlPoints, int width, int height, int resolutionWidth, int resolutionHeight)
        {
            Vector3[] vertices = GenerateVerticesOfPatch(controlPoints, width, height, resolutionWidth, resolutionHeight);

            int[] trianglesArray = Triangles.GenerateTrianglesClockwise(resolutionWidth, resolutionHeight);

            Mesh patchMesh = new Mesh();
            patchMesh.SetVertices(vertices);
            patchMesh.SetTriangles(trianglesArray, 0);
            patchMesh.SetUVs(0, TextureCoordinates.GenerateQuadrilateralUVs(vertices.Length, resolutionHeight));

            patchMesh.RecalculateNormals();
            patchMesh.RecalculateTangents();

            return patchMesh;
        }
    }
}