using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;
using VRSketchingGeometry;
using VRSketchingGeometry.Commands;
using VRSketchingGeometry.Commands.Line;

public class test : MonoBehaviour
{
    public DefaultReferences Defaults;
    private BezierCurveSketchObject BezierCurveSketchObject;
    private BezierPatchSketchObject BezierPatchSketchObject;
    private CommandInvoker Invoker;
    private float scale = 0.4f;
    
    // Start is called before the first frame update
    void Start()
    {
        //DrawLine();
        //DrawSurface();
    }

    void DrawLine()
    {
        BezierCurveSketchObject = Instantiate(Defaults.BezierCurveSketchObjectPrefab).GetComponent<BezierCurveSketchObject>();
        /*LineSketchObject.SetControlPoints(new List<Vector3>()
        {
            new Vector3(1, 2, 3), new Vector3(1, 5, 3),
            new Vector3(1, 5, 7), new Vector3(1, 2, 7)
        });*/
        Invoker = new CommandInvoker();
        Invoker.ExecuteCommand(new AddControlPointCommand(this.BezierCurveSketchObject, new Vector3(10 * scale, 2 * scale, 3 * scale)));
        Invoker.ExecuteCommand(new AddControlPointCommand(this.BezierCurveSketchObject, new Vector3(10 * scale, 5 * scale, 3 * scale)));
        Invoker.ExecuteCommand(new AddControlPointCommand(this.BezierCurveSketchObject, new Vector3(10 * scale, 5 * scale, 7 * scale)));
        Invoker.ExecuteCommand(new AddControlPointCommand(this.BezierCurveSketchObject, new Vector3(10 * scale, 2 * scale, 7 * scale)));
        //Invoker.Undo();
    }

    void DrawSurface()
    {
        BezierPatchSketchObject = Instantiate(Defaults.BezierPatchSketchObjectPrefab).GetComponent<BezierPatchSketchObject>();
        BezierPatchSketchObject.SetControlPoints(new List<Vector3>()
        {
            new Vector3(1 * scale, 2 * scale, 3 * scale), new Vector3(1 * scale, 2 * scale, 6 * scale), new Vector3(1 * scale, 2 * scale, 9 * scale), new Vector3(1 * scale, 2 * scale, 12 * scale),
            new Vector3(1 * scale, 5 * scale, 3 * scale), new Vector3(4 * scale, 5 * scale, 6 * scale), new Vector3(4 * scale, 5 * scale, 9 * scale), new Vector3(1 * scale, 5 * scale, 12 * scale),
            //new Vector3(1, 8, 3), new Vector3(4, 8, 6), new Vector3(4, 8, 9), new Vector3(1, 8, 12),
            //new Vector3(1, 11, 3), new Vector3(1, 11, 6), new Vector3(1, 11, 9), new Vector3(1, 11, 12)
        },4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
