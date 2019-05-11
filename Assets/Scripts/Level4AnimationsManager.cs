using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Level4AnimationsManager : MonoBehaviour {
    private static Level4AnimationsManager _instance;
    public static Level4AnimationsManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Level4AnimationsManager>();
            }

            return _instance;
        }
    }
    public float timeBetweenShootingRays = 3f;
    public Transform lSRayEndPosition;

    private int stopIndex = 0;

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level4_Events.LIGHT_SPIRITS_STOP, HandleLightSpiritsStop);
        EventManager.StartListening(Global.Level4_Events.LIGHT_SPIRITS_RAISE_ARM, UnlockControllerSelectionRay);
        EventManager.StartListening(Global.Level4_Events.PLAYER_HIT_TRASH, StartLightSpiritsRayAnimation);
        EventManager.StartListening(Global.GetSharedHintString(Global.Shared_Hints.TUT_GOAWAY), HideGoAwayHint);
        // EventManager.StartListening(Global.Level4_Events.AFTER_MIRACLE_OCCURED, HandleLightSpiritTransition);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level4_Events.LIGHT_SPIRITS_STOP, HandleLightSpiritsStop);
        EventManager.StopListening(Global.Level4_Events.LIGHT_SPIRITS_RAISE_ARM, UnlockControllerSelectionRay);
        EventManager.StopListening(Global.Level4_Events.PLAYER_HIT_TRASH, StartLightSpiritsRayAnimation);
        EventManager.StopListening(Global.GetSharedHintString(Global.Shared_Hints.TUT_GOAWAY), HideGoAwayHint);
        // EventManager.StopListening(Global.Level4_Events.AFTER_MIRACLE_OCCURED, HandleLightSpiritTransition);
    }

    private void Start() {
        // TODO: change procedure
        StartCoroutine(ForceTrashFallingStart());
        // StartCoroutine(ForcePlayerHitTrash());
    }

    private IEnumerator ForceTrashFallingStart()
    {
        // TODO: change timer
        yield return new WaitForSecondsRealtime(12f);
        DebugManager.Info(Global.Level4_Events.TRASH_FALLING_START);
        EventManager.TriggerEvent(Global.Level4_Events.TRASH_FALLING_START);
    }
    // private IEnumerator ForcePlayerHitTrash()
    // {
    //     // TODO: change procedure
    //     yield return new WaitForSecondsRealtime(40f);
    //     DebugManager.Info(Global.Level4_Events.PLAYER_HIT_TRASH);
    //     EventManager.TriggerEvent(Global.Level4_Events.PLAYER_HIT_TRASH);
    // }

    private void StartLightSpiritsRayAnimation()
    {
        StartCoroutine(LightSpiritsShootRayAnimation());
    }

    private IEnumerator LightSpiritsShootRayAnimation()
    {
        Dictionary<string, LightSpiritsController.LightSpirit> lsDict = LightSpiritsController.instance.LightSpiritsDict;

        yield return new WaitForSecondsRealtime(timeBetweenShootingRays);

        lsDict["Angel White"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Red"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Yellow"].lsSelectionRay.TriggerRay(lSRayEndPosition);

        yield return new WaitForSecondsRealtime(timeBetweenShootingRays);

        lsDict["Angel BabyBlue"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Green"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Orange"].lsSelectionRay.TriggerRay(lSRayEndPosition);

        yield return new WaitForSecondsRealtime(timeBetweenShootingRays);

        lsDict["Angel Purple"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Blue"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Gold"].lsSelectionRay.TriggerRay(lSRayEndPosition);

        yield return new WaitForSecondsRealtime(timeBetweenShootingRays);
        // hide hint
        ControllerHintsManager.instance.HideSpecificButtonHint(Global.Shared_Hints.TUT_SELECTIONRAY);

        // show go away hint
        Global.Shared_Controllers.VOICECOMMAND = true;
        EventManager.TriggerEvent(Global.Shared_Events.SET_VOICECOMMAND);

        yield return new WaitForSecondsRealtime(timeBetweenShootingRays);
        ControllerHintsManager.instance.ShowSpecificButtonHint(Global.Shared_Hints.TUT_GOAWAY);
    }

    private void HandleLightSpiritsStop()
    {
        DebugManager.Info("LightSpirits Stop: " + stopIndex);
        switch (stopIndex)
        {
            // after trash starts falling
            case 0:
                EventManager.TriggerEvent(Global.Level4_Events.LIGHT_SPIRITS_RAISE_ARM);
                break;
            default:
                break;
        }
        stopIndex++;
    }

    private void UnlockControllerSelectionRay()
    {
        Global.Shared_Controllers.SELECTION_RAY = true;
        EventManager.TriggerEvent(Global.Shared_Events.SET_SELECTION_RAY);

        StartCoroutine(ShowSelectionRayButtonHint());
    }

    private IEnumerator ShowSelectionRayButtonHint()
    {
        yield return new WaitForEndOfFrame();
        ControllerHintsManager.instance.ShowSpecificButtonHint(Global.Shared_Hints.TUT_SELECTIONRAY);
    }


    private void HideGoAwayHint()
    {
        ControllerHintsManager.instance.HideSpecificButtonHint(Global.Shared_Hints.TUT_GOAWAY);
    }

    private void HandleLightSpiritTransition()
    {

        // throw new NotImplementedException();
    }

    private void Awake()
	{
        _instance = this;
        instance.stopIndex = 0;
    }
}
