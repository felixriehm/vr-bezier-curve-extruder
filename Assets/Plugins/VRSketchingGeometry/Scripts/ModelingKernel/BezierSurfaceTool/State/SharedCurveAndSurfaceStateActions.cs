using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.BezierSurfaceTool.State
{
    internal abstract class SharedCurveAndSurfaceStateActions : BezierSurfaceToolState
    {
        internal SharedCurveAndSurfaceStateActions(BezierSurfaceTool tool, BezierSurfaceToolSettings settings,
            BezierSurfaceToolStateData stateData): base(tool, settings, stateData)
        {
        }
        internal override void ExitTool()
        {
            for (int i = 0; i < BezierSurfaceToolStateData.controllerHandles.Length; i++)
            {
                Object.Destroy(BezierSurfaceToolStateData.controllerHandles[i]);
            }
            
            for (int i = 0; i < BezierSurfaceToolStateData.cpHandles.Length; i++)
            {
                Object.Destroy(BezierSurfaceToolStateData.cpHandles[i]);
            }
            
            Object.Destroy(BezierSurfaceToolStateData.BezierCurveSketchObject.gameObject);
            // if you exit the tool while drawing, 'StopDrawSurface()' does not destroy temporaryBezierPatch
            if (BezierSurfaceToolStateData.temporaryBezierPatch != null)
            {
                Object.Destroy(BezierSurfaceToolStateData.temporaryBezierPatch.gameObject);
            }
                
            BezierSurfaceTool.CurrentBezierSurfaceToolState = new StateToolNotStarted(BezierSurfaceTool, BezierSurfaceToolSettings, BezierSurfaceToolStateData);
        }
        
        internal override void ShowIndicators(bool show)
        {
            ShowIndicatorsHelper(show);
        }

        internal override void ChangeCurveIntensity(BezierSurfaceTool.BezierSurfaceToolController controller, float amount)
        {
            switch (controller)
            {
                case BezierSurfaceTool.BezierSurfaceToolController.Left:
                    BezierSurfaceToolStateData.cpHandles[1].transform.Translate(0, amount, 0);;
                    break;
                case BezierSurfaceTool.BezierSurfaceToolController.Right:
                    BezierSurfaceToolStateData.cpHandles[3].transform.Translate(0, amount, 0);;
                    break;
            }
        }
    }
}