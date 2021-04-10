using System;
using UnityEngine;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class BezierSurfaceSketchObject : SketchObject, ISerializableComponent
    {
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        public int PatchCount { get; private set; }
        
        private void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
            meshFilter.mesh = new Mesh();
            PatchCount = 0;
        }

        public void AddPatch(BezierPatchSketchObject patch)
        {
            MeshFilter meshFilters = patch.gameObject.GetComponent<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[2];

            combine[0].mesh = meshFilter.sharedMesh;
            combine[0].transform = meshFilter.transform.localToWorldMatrix;
            combine[1].mesh = meshFilters.sharedMesh;
            combine[1].transform = meshFilters.transform.localToWorldMatrix;

            meshFilter.mesh = new Mesh();
            meshFilter.mesh.CombineMeshes(combine, true);
            meshCollider.sharedMesh = meshFilter.sharedMesh;

            PatchCount += 1;
        }

        public SerializableComponentData GetData()
        {
            throw new System.NotImplementedException();
        }

        public void ApplyData(SerializableComponentData data)
        {
            throw new System.NotImplementedException();
        }
    }
}