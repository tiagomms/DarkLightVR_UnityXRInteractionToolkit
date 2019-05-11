using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;


public class ToggleControllersManager : MonoBehaviour
{
    private Teleport teleportScript;
    private VoiceCommandManager voiceCommandManagerScript;
    private ToggleLightBeam toggleLightBeamScript;
    private ControllerHintsManager hintManagerScript;

    private void Awake() {
        teleportScript = Teleport.instance;
        hintManagerScript = ControllerHintsManager.instance;
        toggleLightBeamScript = ToggleLightBeam.instance;
        voiceCommandManagerScript = gameObject.GetComponent<VoiceCommandManager>();
    }

    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.SET_TELEPORT, SetTeleportScript);
        EventManager.StartListening(Global.Shared_Events.SET_VOICECOMMAND, SetVoiceCommandScript);
        EventManager.StartListening(Global.Shared_Events.SET_SELECTION_RAY, SetToggleLightBeamScript);
        EventManager.StartListening(Global.Shared_Events.SET_HINTMENU, SetHintManagerScript);
    }


    private void OnDisable()
    {
        EventManager.StopListening(Global.Shared_Events.SET_TELEPORT, SetTeleportScript);
        EventManager.StopListening(Global.Shared_Events.SET_VOICECOMMAND, SetVoiceCommandScript);
        EventManager.StopListening(Global.Shared_Events.SET_SELECTION_RAY, SetToggleLightBeamScript);
        EventManager.StopListening(Global.Shared_Events.SET_HINTMENU, SetHintManagerScript);
    }

    private void SetTeleportScript()
    {
        if (teleportScript != null)
        {
            teleportScript.enabled = Global.Shared_Controllers.TELEPORT;
        }
    }

    private void SetVoiceCommandScript()
    {
        if (voiceCommandManagerScript != null)
        {
            voiceCommandManagerScript.enabled = Global.Shared_Controllers.VOICECOMMAND;
        }
    }

    private void SetToggleLightBeamScript()
    {
        if (toggleLightBeamScript != null)
        {
            // gameObject of toggleLightBeamScript is the parent of all light beam variations
            // so by setting active / inactive this object, it blocks the light beams
            toggleLightBeamScript.gameObject.SetActive(Global.Shared_Controllers.SELECTION_RAY);
        }
    }
    private void SetHintManagerScript()
    {
        if (hintManagerScript != null)
        {
            hintManagerScript.enabled = Global.Shared_Controllers.HINTMENU;
        }
    }
}
