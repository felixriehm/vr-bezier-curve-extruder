using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Pointer3D;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class SteamVRPointerEventData : Pointer3DEventData
{
    public SteamVRRaycaster SteamVRRaycaster { get; private set; }
    public SteamVR_Action_Boolean SteamVRActionBoolean { get; private set; }
    
    public SteamVR_Input_Sources SteamVRInputSource { get; private set; }

    public SteamVRPointerEventData(SteamVRRaycaster ownerRaycaster, EventSystem eventSystem, SteamVR_Action_Boolean steamVRActionBoolean, SteamVR_Input_Sources steamVRInputSource, InputButton mouseButton) : base(ownerRaycaster, eventSystem)
    {
        this.SteamVRRaycaster = ownerRaycaster;
        this.button = mouseButton;

        SteamVRActionBoolean = steamVRActionBoolean;
        SteamVRInputSource = steamVRInputSource;
    }

    public override bool GetPress() { return SteamVRActionBoolean.GetState(SteamVRInputSource); }

    public override bool GetPressDown() { return SteamVRActionBoolean.GetStateDown(SteamVRInputSource); }

    public override bool GetPressUp()
    {
        return SteamVRActionBoolean.GetStateUp(SteamVRInputSource);
    }
}
