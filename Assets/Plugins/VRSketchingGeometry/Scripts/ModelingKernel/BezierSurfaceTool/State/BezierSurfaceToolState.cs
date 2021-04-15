using UnityEngine;
using VRSketchingGeometry.SketchObjectManagement;

namespace VRSketchingGeometry.BezierSurfaceTool.State
{
    internal abstract class BezierSurfaceToolState
    {
        protected BezierSurfaceTool BezierSurfaceTool { get; set; }
        protected BezierSurfaceToolStateData BezierSurfaceToolStateData;
        protected BezierSurfaceToolSettings BezierSurfaceToolSettings;

        internal BezierSurfaceToolState(BezierSurfaceTool tool, BezierSurfaceToolSettings settings, BezierSurfaceToolStateData stateData)
        {
            BezierSurfaceTool = tool;
            BezierSurfaceToolSettings = settings;
            BezierSurfaceToolStateData = stateData;
        }
        
        internal abstract void StartTool(Transform leftControllerOrigin, Transform rightControllerOrigin, int steps = 20, float diameter = 0.1f);
        internal abstract void ExitTool();
        internal abstract void StartDrawingSurface();
        internal abstract void StopDrawingSurface();
        internal abstract void ShowIndicators(bool show);
        internal abstract void ChangeCurveIntensity(BezierSurfaceTool.BezierSurfaceToolController controller, float amount);
        internal abstract void Update();
        internal abstract BezierSurfaceTool.BezierSurfaceToolState GetCurrentState();
        
        protected internal void ShowIndicatorsHelper(bool show)
        {
            foreach (var controllerHandle in BezierSurfaceToolStateData.controllerHandles)
            {
                foreach (var childRenderer in controllerHandle.GetComponentsInChildren<Renderer>())
                {
                    childRenderer.enabled = show;
                }
            }
        }
    }
}