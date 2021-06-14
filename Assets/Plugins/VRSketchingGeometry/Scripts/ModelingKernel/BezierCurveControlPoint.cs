using UnityEngine;
using UnityEngine.UIElements;

namespace VRSketchingGeometry.BezierSurfaceTool
{
    /// <summary>
    /// A control point for the BezierCurve class.
    /// Contains the position of the control point that control the shape of the spline.
    /// </summary>
    /// <remarks>Original author: Felix Riehm</remarks>
    public class BezierCurveControlPoint
    {
        public Vector3 Position { get; set; }
        
        public BezierCurveControlPoint() : this(Vector3.zero) { }

        /// <summary>
        /// Implicitly convert a Vector3 object to a <see cref="BezierCurveControlPoint"/> object.
        /// </summary>
        /// <param name="position"></param>
        public static implicit operator BezierCurveControlPoint(Vector3 position) => new BezierCurveControlPoint(position);

        /// <summary>
        /// Constructor for the BezierCurveControlPoint.
        /// </summary>
        /// <param name="position">Position of the control point.</param>
        public BezierCurveControlPoint(Vector3 position) {
            Position = position;
        }
    }
}