using System;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.BezierSurfaceTool;
using VRSketchingGeometry.Serialization;

namespace VRSketchingGeometry.SketchObjectManagement
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class ExtrudedBezierCurveSketchObject : SketchObject, ISerializableComponent
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
            PatchCount = 0;
            bezierPatchData = new List<PatchSketchObjectData>();
            name = "BezierSurface";
        }

        public void AddPatch(BezierPatchSketchObject bezierPatchSketchObject)
        {
            // Get patch data
            PatchSketchObjectData patchSketchObjectData = (PatchSketchObjectData) bezierPatchSketchObject
                .GetComponent<ISerializableComponent>().GetData();

            AddPatchToExtrudedBezierCurve(patchSketchObjectData);
        }
        
        private void AddPatchToExtrudedBezierCurve(PatchSketchObjectData patchSketchObjectData)
        {
            // save bezier patch data for serialization
            bezierPatchData.Add(patchSketchObjectData);
            
            // init new bezier patch and add it to surface object
            GameObject bezierPatch = Instantiate(Defaults.BezierPatchSketchObjectPrefab, gameObject.transform);
            bezierPatch.GetComponent<ISerializableComponent>().ApplyData(patchSketchObjectData);

            PatchCount += 1;
        }

        public void CombinePatchesToSingleMesh()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];

            int i = 1;
            while (i < meshFilters.Length)
            {
                combine[i-1].mesh = meshFilters[i].sharedMesh;
                combine[i-1].transform = meshFilters[i].transform.localToWorldMatrix;
                Destroy(meshFilters[i].gameObject);//.SetActive(false));

                i++;
            }
            meshFilter.mesh = new Mesh();
            try
            {
                meshFilter.mesh.CombineMeshes(combine, true);
                meshFilter.mesh.RecalculateNormals();
            }
            catch (ArgumentException e)
            {
                Debug.LogWarning("Failed to create bezier surface while combining patch meshes. Bezier surface is too long and has too many vertices:");
                Debug.LogWarning(e.Message);
                Destroy(this);
                return;
            }
            
            meshCollider.sharedMesh = meshFilter.sharedMesh;
            transform.gameObject.SetActive(true);
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
                
                foreach (var patchSketchObjectData in surfaceData.BezierPatchData)
                {
                    AddPatchToExtrudedBezierCurve(patchSketchObjectData);
                }

                CombinePatchesToSingleMesh();
            
                data.ApplyDataToTransform(this.transform);
            }
        }
    }
}