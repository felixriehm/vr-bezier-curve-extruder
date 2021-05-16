using System.Collections.Generic;
using UnityEngine;

namespace VRSketchingGeometry.Serialization
{
    public class BezierSurfaceObjectData : SketchObjectData
    {
        public List<PatchSketchObjectData> BezierPatchData;
        
        internal override ISerializableComponent InstantiateComponent(DefaultReferences defaults)
        {
            return GameObject.Instantiate(defaults.BezierSurfaceSketchObjectPrefab).GetComponent<ISerializableComponent>();
        }
    }
}