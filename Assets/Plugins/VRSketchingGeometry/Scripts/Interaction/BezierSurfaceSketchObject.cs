using System;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class BezierSurfaceSketchObject : SketchObject, ISerializableComponent
    {
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private List<PatchSketchObjectData> bezierPatchData;
        
        public int PatchCount { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
            meshFilter.mesh = new Mesh();
            PatchCount = 0;
            bezierPatchData = new List<PatchSketchObjectData>();
            name = "BezierSurface";
        }

        public void AddPatch(BezierPatchSketchObject patch)
        {
            // save bezier patch data for serialization
            PatchSketchObjectData patchSketchObjectData = (PatchSketchObjectData) patch.gameObject.GetComponent<ISerializableComponent>().GetData();
            bezierPatchData.Add(patchSketchObjectData);

            // merge bezier patch mesh with bezier surface mesh
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
            BezierSurfaceObjectData data = new BezierSurfaceObjectData
            {
                BezierPatchData = this.bezierPatchData
            };
                
            data.SetDataFromTransform(this.transform);
            
            return data;
        }

        public void ApplyData(SerializableComponentData data)
        {
            if (data is BezierSurfaceObjectData surfaceData)
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                
                GameObject bezierPatch;
                foreach (var patchSketchObjectData in surfaceData.BezierPatchData)
                {
                    bezierPatch = Instantiate(Defaults.BezierPatchSketchObjectPrefab);
                    bezierPatch.GetComponent<ISerializableComponent>().ApplyData(patchSketchObjectData);
                    AddPatch(bezierPatch.GetComponent<BezierPatchSketchObject>());
                    Destroy(bezierPatch);
                }
            
                data.ApplyDataToTransform(this.transform);
            }
        }
    }
}