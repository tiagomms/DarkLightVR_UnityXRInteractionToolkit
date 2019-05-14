using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ControllerHintsManager : MonoBehaviour {

    private static ControllerHintsManager _instance;
    public static ControllerHintsManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ControllerHintsManager>();
            }

            return _instance;
        }
    }
    private SteamVR_Input_ActionSet_default actionsSet;
    private Teleport teleportScript;

    private Hand leftHand;
	private Hand rightHand;
    private bool isActivated = false;

    void Awake()
    {
        _instance = this;

        instance.teleportScript = Teleport.instance;
        instance.leftHand = GameObject.FindGameObjectWithTag("LeftHand").GetComponent<Hand>();
        instance.rightHand = GameObject.FindGameObjectWithTag("RightHand").GetComponent<Hand>();
        instance.actionsSet = SteamVR_Actions._default;

	}

    #region HINT_MENU
    public void ShowTextHintMenu() {
        string voiceStrAux = "";
        ControllerButtonHints.ShowTextHint(instance.leftHand, instance.actionsSet.ExtraMenu, "Hint Menu", false);

        if (Global.Shared_Controllers.TELEPORT) {
            ControllerButtonHints.ShowTextHint(instance.leftHand, instance.actionsSet.Teleport, "Teleport", false);
        }

        if (Global.Shared_Controllers.SELECTION_RAY) {
            ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.SelectionRay, "Select Trash", false);
            ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.CancelSelection, "\t\t\t\t\t\tCancel Selection", false);
            voiceStrAux += "\nGo Away (trash)";
        }

        if (Global.Shared_Controllers.MEDITATION_CIRCLE_READY) {
            voiceStrAux += "\nI am Ready \n(sit in meditation)"; 
        }

        if (Global.Shared_Controllers.VOICECOMMAND && voiceStrAux != "") {
            ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.VoiceInput, "Press for Voice Input:" + voiceStrAux, false);
        }

        ControllerButtonHints.ShowTextHint(instance.leftHand, instance.actionsSet.GrabPinch, "Grab", false);
        ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.GrabPinch, "Grab", false);

    }

    public void HideTextHintMenu() {
        ControllerButtonHints.HideAllTextHints(instance.leftHand);
        ControllerButtonHints.HideAllTextHints(instance.rightHand);
    }

    #endregion

    #region INDIVIDUAL_HINTS
    public void ShowSpecificTextHint(Global.Shared_Hints hint) {
        switch (hint)
        {
            case Global.Shared_Hints.TUT_TELEPORT:
                instance.teleportScript.ShowTeleportHint(); // Teleport.cs already has its own hint
                break;
            case Global.Shared_Hints.TUT_HINTMENU:
                ControllerButtonHints.ShowTextHint(instance.leftHand, instance.actionsSet.ExtraMenu, "Hint Menu");
                break;
            case Global.Shared_Hints.TUT_SELECTIONRAY:
                ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.SelectionRay, "Press to Select Trash");
                break;
            case Global.Shared_Hints.TUT_CANCELSELECTION:
                ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.CancelSelection, "\t\t\t\t\tCancel Selection");
                break;
            case Global.Shared_Hints.TUT_GRAB:
                ControllerButtonHints.ShowTextHint(instance.leftHand, instance.actionsSet.GrabPinch, "Grab");
                ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.GrabPinch, "Grab");
                break;
            case Global.Shared_Hints.TUT_THROW:
                ControllerButtonHints.ShowTextHint(instance.leftHand, instance.actionsSet.GrabPinch, "Grab & Throw");
                ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.GrabPinch, "Grab & Throw");
                break;
            case Global.Shared_Hints.TUT_GOAWAY:
                ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.VoiceInput, "W/ trash selected\nKeep pressing &\nsay: Go Away");
                break;
            case Global.Shared_Hints.TUT_IAMREADY:
                ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.VoiceInput, "Sitting in the circle\nPress & say: I am Ready");
                break;
            default:
                break;
        }
    }

    public void HideSpecificTextHint(Global.Shared_Hints hint) {
        switch (hint)
        {
            case Global.Shared_Hints.TUT_TELEPORT:
                // Teleport.cs has its own hint in place
                break;
            case Global.Shared_Hints.TUT_HINTMENU:
                ControllerButtonHints.HideTextHint(instance.leftHand, instance.actionsSet.ExtraMenu);
                break;
            case Global.Shared_Hints.TUT_SELECTIONRAY:
                ControllerButtonHints.HideTextHint(instance.rightHand, instance.actionsSet.SelectionRay);
                break;
            case Global.Shared_Hints.TUT_CANCELSELECTION:
                ControllerButtonHints.HideTextHint(instance.rightHand, instance.actionsSet.CancelSelection);
                break;
            case Global.Shared_Hints.TUT_GRAB:
                ControllerButtonHints.HideTextHint(instance.leftHand, instance.actionsSet.GrabPinch);
                ControllerButtonHints.HideTextHint(instance.rightHand, instance.actionsSet.GrabPinch);
                break;
            case Global.Shared_Hints.TUT_THROW:
                ControllerButtonHints.HideTextHint(instance.leftHand, instance.actionsSet.GrabPinch);
                ControllerButtonHints.HideTextHint(instance.rightHand, instance.actionsSet.GrabPinch);
                break;
            case Global.Shared_Hints.TUT_GOAWAY:
                ControllerButtonHints.HideTextHint(instance.rightHand, instance.actionsSet.VoiceInput);
                break;
            case Global.Shared_Hints.TUT_IAMREADY:
                ControllerButtonHints.HideTextHint(instance.rightHand, instance.actionsSet.VoiceInput);
                break;
            default:
                break;
        }
    }



    public void ShowSpecificButtonHint(Global.Shared_Hints hint)
    {
        switch (hint)
        {
            case Global.Shared_Hints.TUT_TELEPORT:
                instance.teleportScript.ShowTeleportHint(); // Teleport.cs already has its own hint
                break;
            case Global.Shared_Hints.TUT_HINTMENU:
                ControllerButtonHints.ShowButtonHint(instance.leftHand, instance.actionsSet.ExtraMenu);
                break;
            case Global.Shared_Hints.TUT_SELECTIONRAY:
                // for some reason show button hint does not work here
                // ControllerButtonHints.ShowButtonHint(instance.rightHand, instance.actionsSet.SelectionRay);
                ControllerButtonHints.ShowTextHint(instance.rightHand, instance.actionsSet.SelectionRay, "Go for it :)");
                break;
            case Global.Shared_Hints.TUT_CANCELSELECTION:
                ControllerButtonHints.ShowButtonHint(instance.rightHand, instance.actionsSet.CancelSelection);
                break;
            case Global.Shared_Hints.TUT_GRAB:
                ControllerButtonHints.ShowButtonHint(instance.leftHand, instance.actionsSet.GrabPinch);
                ControllerButtonHints.ShowButtonHint(instance.rightHand, instance.actionsSet.GrabPinch);
                break;
            case Global.Shared_Hints.TUT_THROW:
                ControllerButtonHints.ShowButtonHint(instance.leftHand, instance.actionsSet.GrabPinch);
                ControllerButtonHints.ShowButtonHint(instance.rightHand, instance.actionsSet.GrabPinch);
                break;
            case Global.Shared_Hints.TUT_GOAWAY:
                ControllerButtonHints.ShowButtonHint(instance.rightHand, instance.actionsSet.VoiceInput);
                break;
            case Global.Shared_Hints.TUT_IAMREADY:
                ControllerButtonHints.ShowButtonHint(instance.rightHand, instance.actionsSet.VoiceInput);
                break;
            default:
                break;
        }
    }

    public void HideSpecificButtonHint(Global.Shared_Hints hint)
    {
        switch (hint)
        {
            case Global.Shared_Hints.TUT_TELEPORT:
                // Teleport.cs has its own hint in place
                break;
            case Global.Shared_Hints.TUT_HINTMENU:
                ControllerButtonHints.HideButtonHint(instance.leftHand, instance.actionsSet.ExtraMenu);
                break;
            case Global.Shared_Hints.TUT_SELECTIONRAY:
                // for some reason show button hint does not work here
                // ControllerButtonHints.HideButtonHint(instance.rightHand, instance.actionsSet.SelectionRay);
                ControllerButtonHints.HideTextHint(instance.rightHand, instance.actionsSet.SelectionRay);
                break;
            case Global.Shared_Hints.TUT_CANCELSELECTION:
                ControllerButtonHints.HideButtonHint(instance.rightHand, instance.actionsSet.CancelSelection);
                break;
            case Global.Shared_Hints.TUT_GRAB:
                ControllerButtonHints.HideButtonHint(instance.leftHand, instance.actionsSet.GrabPinch);
                ControllerButtonHints.HideButtonHint(instance.rightHand, instance.actionsSet.GrabPinch);
                break;
            case Global.Shared_Hints.TUT_THROW:
                ControllerButtonHints.HideButtonHint(instance.leftHand, instance.actionsSet.GrabPinch);
                ControllerButtonHints.HideButtonHint(instance.rightHand, instance.actionsSet.GrabPinch);
                break;
            case Global.Shared_Hints.TUT_GOAWAY:
                ControllerButtonHints.HideButtonHint(instance.rightHand, instance.actionsSet.VoiceInput);
                break;
            case Global.Shared_Hints.TUT_IAMREADY:
                ControllerButtonHints.HideButtonHint(instance.rightHand, instance.actionsSet.VoiceInput);
                break;
            default:
                break;
        }
    }

    #endregion

    private void Update() {
        if (Global.Shared_Controllers.HINTMENU) {
            if (!instance.isActivated && instance.actionsSet.ExtraMenu.GetState(SteamVR_Input_Sources.LeftHand)) {
                instance.isActivated = true;
                ShowTextHintMenu();

                // for tutorials in level 1
                EventManager.TriggerEvent(Global.GetSharedHintString(Global.Shared_Hints.TUT_HINTMENU));
            }
        }
        if (instance.actionsSet.ExtraMenu.GetStateUp(SteamVR_Input_Sources.LeftHand)) {
            instance.isActivated = false;
            HideTextHintMenu();
        }
    }

}
