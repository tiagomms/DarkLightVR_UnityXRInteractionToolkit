using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;


public class ToggleControllersManager : MonoBehaviour
{
    private static ToggleControllersManager _instance;
    public static ToggleControllersManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ToggleControllersManager>();
            }

            return _instance;
        }
    }

    private Teleport teleportScript;
    private VoiceCommandManager voiceCommandManagerScript;
    private ToggleLightBeam toggleLightBeamScript;
    private ControllerHintsManager hintManagerScript;

    private void Awake() {
        _instance = this;
        instance.teleportScript = Teleport.instance;
        instance.hintManagerScript = ControllerHintsManager.instance;
        instance.toggleLightBeamScript = ToggleLightBeam.instance;
        instance.voiceCommandManagerScript = gameObject.GetComponent<VoiceCommandManager>();
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
        if (instance.teleportScript != null)
        {
            StartCoroutine(SetTeleportEnable());
        }
    }

    private IEnumerator SetTeleportEnable()
    {
        yield return new WaitForEndOfFrame();
        instance.teleportScript.enabled = Global.Shared_Controllers.TELEPORT;
    }

    private void SetVoiceCommandScript()
    {
        if (instance.voiceCommandManagerScript != null)
        {
            instance.voiceCommandManagerScript.enabled = Global.Shared_Controllers.VOICECOMMAND;
        }
    }

    private void SetToggleLightBeamScript()
    {
        if (instance.toggleLightBeamScript != null)
        {
            // gameObject of toggleLightBeamScript is the parent of all light beam variations
            // so by setting active / inactive this object, it blocks the light beams
            instance.toggleLightBeamScript.gameObject.SetActive(Global.Shared_Controllers.SELECTION_RAY);
        }
    }
    private void SetHintManagerScript()
    {
        if (instance.hintManagerScript != null)
        {
            instance.hintManagerScript.enabled = Global.Shared_Controllers.HINTMENU;
        }
    }

    public void EnablePlayerOnEverything()
    {
        Global.Shared_Controllers.TELEPORT = true;
        Global.Shared_Controllers.SELECTION_RAY = true;
        Global.Shared_Controllers.VOICECOMMAND = true;
        Global.Shared_Controllers.HINTMENU = true;

        EventManager.TriggerEvent(Global.Shared_Events.SET_TELEPORT);
        EventManager.TriggerEvent(Global.Shared_Events.SET_SELECTION_RAY);
        EventManager.TriggerEvent(Global.Shared_Events.SET_VOICECOMMAND);
        EventManager.TriggerEvent(Global.Shared_Events.SET_HINTMENU);
    }
}
