using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRSketchingGeometry.Splines;

namespace VRSketchingGeometry.BezierSurfaceTool
{
    /// <summary>
    /// Implementation of Bezier curve
    /// </summary>
    /// <remarks>Original author: Felix Riehm</remarks>
    public class BezierCurve : Spline
    {
        private List<BezierCurveControlPoint> ControlPoints { get; set; }
        private int Steps;
        
        /// <summary>
        /// Constructor for the BezierCurve class.
        /// </summary>
        /// <param name="steps">Number of interpolated points on the curve.</param>
        /// <param name="controlPoints">Control points of the curve curve.</param>
        public BezierCurve(int steps = 16) {
            ControlPoints = new List<BezierCurveControlPoint>();
            InterpolatedPoints = new List<Vector3>();
            Steps = steps;
        }


        /// <summary>
        /// Set all control points and recalculate.
        /// </summary>
        /// <param name="controlPoints"></param>
        public override void SetControlPoints(Vector3[] controlPoints)
        {
            ControlPoints.Clear();
            foreach (Vector3 controlPoint in controlPoints)
            {
                ControlPoints.Add(controlPoint);
            }
            InterpolateSpline();
        }

        /// <summary>
        /// Reset the existing control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controlPoint"></param>
        public override SplineModificationInfo SetControlPoint(int index, Vector3 controlPoint)
        {
            ControlPoints[index] = controlPoint;
            
            InterpolateSpline();
            return new SplineModificationInfo(0, InterpolatedPoints.Count, InterpolatedPoints.Count);
        }

        /// <summary>
        /// Delete the control point at index.
        /// If the index is not the first or last control point the curve will skip the deleted control point and connect the control point before and after the deleted one.
        /// </summary>
        /// <param name="index"></param>
        public override SplineModificationInfo DeleteControlPoint(int index)
        {
            if(index < 0)
            {
                Debug.LogError("Index has to be greater than 0.");
            }
            
            if (ControlPoints.Count == 1 && index == 0)
            {
                Debug.LogError("Last control point can not be removed.");
            }
            
            ControlPoints.RemoveAt(index);
            
            if (ControlPoints.Count == 2 || ControlPoints.Count < 2)
            {
                return new SplineModificationInfo(0, InterpolatedPoints.Count, 0);
            }
            
            InterpolateSpline();
            return new SplineModificationInfo(0, InterpolatedPoints.Count, InterpolatedPoints.Count);
        }

        /// <summary>
        /// Add control point at the end of the curve.
        /// </summary>
        /// <param name="controlPoint"></param>
        public override SplineModificationInfo AddControlPoint(Vector3 controlPoint)
        {
            return InsertControlPoint(ControlPoints.Count, controlPoint);
        }

        /// <summary>
        /// Insert a control point at index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="controlPoint"></param>
        public override SplineModificationInfo InsertControlPoint(int index, Vector3 controlPoint)
        {
            if(index < 0)
            {
                Debug.LogError("Index has to be greater than 0.");
            }
            
            ControlPoints.Insert(index, controlPoint);

            if (ControlPoints.Count < 3)
            {
                return new SplineModificationInfo(0, 0, 0);
            }
            if (ControlPoints.Count == 3)
            {
                InterpolateSpline();
                return new SplineModificationInfo(0, 0, InterpolatedPoints.Count);
            }
            
            InterpolateSpline();
            return new SplineModificationInfo(0, InterpolatedPoints.Count, InterpolatedPoints.Count);
        }

        public override int GetNumberOfControlPoints()
        {
            return ControlPoints.Count;
        }

        public override List<Vector3> GetControlPoints()
        {
            List<Vector3> vectorControlPoints = ControlPoints.Select(controlPoint => controlPoint.Position).ToList();
            return vectorControlPoints;
        }
        
        /// <summary>
        /// Recalculate the complete spline.
        /// </summary>
        private void InterpolateSpline()
        {
            InterpolatedPoints.Clear();
            if (ControlPoints.Count < 3)
            {
                Debug.LogWarning("BezierCurve: Not enough control points! Minimum is 2.");
                return;
            }
            if (Steps < 1)
            {
                Debug.LogWarning("BezierCurve: Steps must be higher than 0.");
                return;
            }
            
            float iterationStep = 1f/ (Steps-1);
            float t = 0;
            Vector3[] startPoints = ControlPoints.Select(controlPoint => controlPoint.Position).ToArray();
            for (int i = 0; i < Steps; i++)
            {
                InterpolatedPoints.Add(PointOnBezierCurve(startPoints, t));
                t += iterationStep;
            }
            
        }
        
        
        // De Casteljau algorithm
        private Vector3 PointOnBezierCurve(Vector3[] points, float t) {
            // init points
            // segmentationPoints: points which are on the segmentation lines of the De Casteljau algorithm
            Vector3[] segmentationPoints = new Vector3[points.Length - 1]; 
            for(int i = 0; i < segmentationPoints.Length ; i++) {
                Vector3 firstPoint = points[i];
                Vector3 secondPoint = points[i+1];

                segmentationPoints[i] = new Vector3(
                    lerp(firstPoint.x, secondPoint.x, t),
                    lerp(firstPoint.y, secondPoint.y, t),
                    lerp(firstPoint.z, secondPoint.z, t));
            }

            // point on bezier curve
            if(segmentationPoints.Length == 1) {
                return segmentationPoints[0];
            }

            return PointOnBezierCurve(segmentationPoints, t);
        }
        
        private float lerp(float a, float b, float t)
        {
            return (1 - t) * a + t * b;
        }
    }
}