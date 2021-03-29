﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VRSketchingGeometry.Serialization
{
    /// <summary>
    /// Contains the serialization data of a <see cref="VRSketchingGeometry.SketchObjectManagement.PatchSketchObject"/>.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    public class PatchSketchObjectData : SketchObjectData
    {
        public int Width;
        public int Height;
        public int ResolutionWidth;
        public int ResolutionHeight;
        public List<Vector3> ControlPoints;
        public SketchMaterialData SketchMaterial;
    }
}
