using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VRSketchingGeometry.BezierSurfaceTool;
using UnityEngine;
using VRSketchingGeometry;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement
{
    /// <summary>
    /// Patch surface sketch object using a grid of control points.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class BezierPatchSketchObject : PatchSketchObject, ISerializableComponent, IBrushable
    {
        /// <summary>
        /// Generates the patch mesh and assigns it to the MeshFilter of this GameObject.
        /// </summary>
        /// <param name="controlPoints"></param>
        /// <param name="width">Number of control points in x direction</param>
        /// <param name="height">Number of control points in y direction</param>
        private void UpdatePatchMesh(List<Vector3> controlPoints, int width, int height)
        {
            if (height < 3 || width < 3 || controlPoints.Count / width != height) {
                Debug.LogWarning("The amount of control points is invalid! \n There must at least be 3x3 control points. Amount of control ponits must be a multiple of width.");
                SetMesh(null);
                return;
            }

            Mesh patchMesh = BezierPatchMesh.GeneratePatchMesh(controlPoints, width, height, this.ResolutionWidth, this.ResolutionHeight);
            SetMesh(patchMesh);
        }
    }
}