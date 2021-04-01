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
    private LineSketchObject LineSketchObject;
    private PatchSketchObject PatchSketchObject;
    private CommandInvoker Invoker;
    
    // Start is called before the first frame update
    void Start()
    {
        //DrawLine();
        DrawSurface();
    }

    void DrawLine()
    {
        LineSketchObject = Instantiate(Defaults.LineSketchObjectPrefab).GetComponent<LineSketchObject>();
        /*LineSketchObject.SetControlPoints(new List<Vector3>()
        {
            new Vector3(1, 2, 3), new Vector3(1, 5, 3),
            new Vector3(1, 5, 7), new Vector3(1, 2, 7)
        });*/
        Invoker = new CommandInvoker();
        Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 2, 3)));
        Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 5, 3)));
        Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 5, 7)));
        Invoker.ExecuteCommand(new AddControlPointCommand(this.LineSketchObject, new Vector3(1, 2, 7)));
        //Invoker.Undo();
    }

    void DrawSurface()
    {
        PatchSketchObject = Instantiate(Defaults.PatchSketchObjectPrefab).GetComponent<PatchSketchObject>();
        PatchSketchObject.SetControlPoints(new List<Vector3>()
        {
            new Vector3(1, 2, 3), new Vector3(1, 2, 6), new Vector3(1, 2, 9), new Vector3(1, 2, 12),
            new Vector3(1, 5, 3), new Vector3(4, 5, 6), new Vector3(4, 5, 9), new Vector3(1, 5, 12),
            new Vector3(1, 8, 3), new Vector3(4, 8, 6), new Vector3(4, 8, 9), new Vector3(1, 8, 12),
            new Vector3(1, 11, 3), new Vector3(1, 11, 6), new Vector3(1, 11, 9), new Vector3(1, 11, 12)
        },4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
