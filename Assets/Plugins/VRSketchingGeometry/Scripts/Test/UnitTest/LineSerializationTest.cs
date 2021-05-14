﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.Utils;

namespace Tests
{
    public class LineSerializationTest
    {
        private LineSketchObject LineSketchObject;
        private ISerializableComponent SerializableSketchObject;

        [UnitySetUp]
        public IEnumerator SetUpScene()
        {
            yield return SceneManager.LoadSceneAsync("CommandTestScene", LoadSceneMode.Single);
            this.LineSketchObject = GameObject.FindObjectOfType<LineSketchObject>();
            this.SerializableSketchObject = this.LineSketchObject;
            yield return null;
        }

        [Test]
        public void GetData_ControlPoints()
        {
            this.LineSketchObject.SetControlPointsLocalSpace(new List<Vector3> { new Vector3(1, 2, 3), new Vector3(3, 2, 1), new Vector3(1, 1, 1), new Vector3(2, 2, 2) });
            LineSketchObjectData data = this.SerializableSketchObject.GetData() as LineSketchObjectData;
            Assert.AreEqual(new Vector3(3, 2, 1), data.ControlPoints[1]);
        }

        [Test]
        public void GetData_CrossSection()
        {
            List<Vector3> crossSection = new List<Vector3> { new Vector3(0, 0, 1), new Vector3(.5f, 0, 0), new Vector3(-.5f, 0, 0) };
            this.LineSketchObject.SetLineCrossSection(crossSection, crossSection, .3f);
            LineSketchObjectData data = this.SerializableSketchObject.GetData() as LineSketchObjectData;
            Assert.AreEqual(new Vector3(.5f, 0, 0), data.CrossSectionVertices[1]);
            Assert.AreEqual(new Vector3(0, 0, 1), data.CrossSectionVertices[0]);
            Assert.AreEqual(.3f, data.CrossSectionScale);
        }

        [Test]
        public void GetData_Position() {
            this.LineSketchObject.transform.position = new Vector3(1,2,3);
            LineSketchObjectData data = this.SerializableSketchObject.GetData() as LineSketchObjectData;
            Assert.AreEqual(new Vector3(1, 2, 3), data.Position);
        }

        [Test]
        public void GetData_Rotation()
        {
            this.LineSketchObject.transform.rotation = Quaternion.Euler(0,25,0);
            LineSketchObjectData data = this.SerializableSketchObject.GetData() as LineSketchObjectData;
            Assert.That(data.Rotation, Is.EqualTo(Quaternion.Euler(0, 25, 0)).Using(QuaternionEqualityComparer.Instance));
        }

        [Test]
        public void GetData_Scale()
        {
            this.LineSketchObject.transform.localScale = new Vector3(3,3,3);
            LineSketchObjectData data = this.SerializableSketchObject.GetData() as LineSketchObjectData;
            Assert.AreEqual(new Vector3(3, 3, 3), data.Scale);
        }

        [Test]
        public void ApplyData_ControlPoints()
        {
            LineSketchObjectData data = this.SerializableSketchObject.GetData() as LineSketchObjectData;
            data.ControlPoints = new List<Vector3> { new Vector3(1, 2, 3), new Vector3(3, 2, 1), new Vector3(1, 1, 1), new Vector3(2, 2, 2) };
            this.SerializableSketchObject.ApplyData(data);
            Assert.AreEqual(new Vector3(3, 2, 1), this.LineSketchObject.GetControlPoints()[1]);
            Assert.AreEqual((3 * 20 + 2) * 7, this.LineSketchObject.gameObject.GetComponent<MeshFilter>().sharedMesh.vertexCount);
        }

        [Test]
        public void ApplyData_CrossSection()
        {
            LineSketchObjectData data = this.SerializableSketchObject.GetData() as LineSketchObjectData;
            List<Vector3> crossSection = new List<Vector3> { new Vector3(0, 0, 1), new Vector3(.5f, 0, 0), new Vector3(-.5f, 0, 0) };
            data.ControlPoints = new List<Vector3> { new Vector3(1, 2, 3), new Vector3(3, 2, 1), new Vector3(1, 1, 1), new Vector3(2, 2, 2) };
            data.CrossSectionVertices = crossSection;
            data.CrossSectionNormals = crossSection;
            data.CrossSectionScale = 3.0f;
            this.SerializableSketchObject.ApplyData(data);
            Assert.AreEqual(new Vector3(3, 2, 1), this.LineSketchObject.GetControlPoints()[1]);
            Assert.AreEqual((3 * 20 + 2) * 3, this.LineSketchObject.gameObject.GetComponent<MeshFilter>().sharedMesh.vertexCount);
        }

        [Test]
        public void ApplyData_Position() {
            LineSketchObjectData data = this.SerializableSketchObject.GetData() as LineSketchObjectData;
            data.Position = new Vector3(2, 5, 8);
            this.SerializableSketchObject.ApplyData(data);
            Assert.AreEqual(new Vector3(2, 5, 8), this.LineSketchObject.gameObject.transform.position);
        }

        [Test]
        public void ApplyData_Rotation()
        {
            LineSketchObjectData data = this.SerializableSketchObject.GetData() as LineSketchObjectData;
            data.Rotation = Quaternion.Euler(10, 20, 30);
            this.SerializableSketchObject.ApplyData(data);
            Assert.That(this.LineSketchObject.gameObject.transform.rotation, Is.EqualTo(Quaternion.Euler(10, 20, 30)).Using(QuaternionEqualityComparer.Instance));
        }

        [Test]
        public void ApplyData_Scale()
        {
            LineSketchObjectData data = this.SerializableSketchObject.GetData() as LineSketchObjectData;
            data.Scale = new Vector3(1,2,3);
            this.SerializableSketchObject.ApplyData(data);
            Assert.AreEqual(new Vector3(1, 2, 3), this.LineSketchObject.gameObject.transform.localScale);
        }
    }
}
