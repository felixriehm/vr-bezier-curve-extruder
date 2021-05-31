using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Pointer3D;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class SteamVRRaycaster : Pointer3DRaycaster
{
    [SerializeField]
    private SteamVR_Action_Boolean steamVRActionBooleanButton;

    [SerializeField]
    private SteamVR_Input_Sources steamVRInputSource;
    
    [SerializeField]
    private SteamVR_Action_Vector2 steamVRActionBooleanScroll;

    [SerializeField]
    private SteamVR_Input_Sources steamVRInputSourceScroll;
    
    protected override void Start()
    {
        base.Start();

        buttonEventDataList.Add(new SteamVRPointerEventData(this, EventSystem.current, steamVRActionBooleanButton, steamVRInputSource, PointerEventData.InputButton.Left));
    }

    public override Vector2 GetScrollDelta()
    {
        return steamVRActionBooleanScroll.GetAxisDelta(steamVRInputSourceScroll);
    }
}
