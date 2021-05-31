using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSketchingGeometry.BezierSurfaceTool;

public class UI : MonoBehaviour
{
    [SerializeField]
    private Text runtimeValue;
    
    [SerializeField]
    private Image testImage;
    
    private int timeSinceStartup;

    private void Update()
    {
        timeSinceStartup = (int) Time.realtimeSinceStartup;
        runtimeValue.text = timeSinceStartup/60 + ":" + (timeSinceStartup % 60).ToString("00") + " minutes";
    }

    public void OnUTImageChanged(Sprite sprite)
    {
        testImage.sprite = sprite;
    }
}
