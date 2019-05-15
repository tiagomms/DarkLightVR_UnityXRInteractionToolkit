using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Level4_EASTER_AnimationsManager : MonoBehaviour {
    private static Level4_EASTER_AnimationsManager _instance;
    public static Level4_EASTER_AnimationsManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Level4_EASTER_AnimationsManager>();
            }

            return _instance;
        }
    }
    public float timeBetweenShootingRays = 0.5f;
    public Transform lSRayEndPosition;

    public float timeAfterLsDisablingRays = 10f;

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level4_Events.PLAYER_HIT_TRASH, StartLightSpiritsRayAnimation);
        EventManager.StartListening(Global.Level4_Events.AFTER_MIRACLE_OCCURED, StartAfterMiracleAnimations);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level4_Events.PLAYER_HIT_TRASH, StartLightSpiritsRayAnimation);
        EventManager.StartListening(Global.Level4_Events.AFTER_MIRACLE_OCCURED, StartAfterMiracleAnimations);
    }
    private void Awake()
    {
        _instance = this;
    }

    private void Start() {
        Level4_EASTER_LSAnimationsAroundPlatform.instance.PlaceLightSpiritsAroundPlatform();
        StartCoroutine(TrashFallingStart_Coroutine());
    }

    private IEnumerator TrashFallingStart_Coroutine()
    {
        yield return new WaitForSecondsRealtime(5f);
        LightSpiritsController.instance.RaiseLightSpiritsArms();
        EventManager.TriggerEvent(Global.Level4_Events.TRASH_FALLING_START);
    }

    private void StartLightSpiritsRayAnimation()
    {
        StartCoroutine(LightSpiritsShootRayAnimation());
    }

    private IEnumerator LightSpiritsShootRayAnimation()
    {
        Dictionary<string, LightSpiritsController.LightSpirit> lsDict = LightSpiritsController.instance.LightSpiritsDict;

        yield return new WaitForEndOfFrame();

        lsDict["Angel White"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Red"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Yellow"].lsSelectionRay.TriggerRay(lSRayEndPosition);

        yield return new WaitForEndOfFrame();

        lsDict["Angel BabyBlue"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Green"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Orange"].lsSelectionRay.TriggerRay(lSRayEndPosition);

        yield return new WaitForEndOfFrame();

        lsDict["Angel Purple"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Blue"].lsSelectionRay.TriggerRay(lSRayEndPosition);
        lsDict["Angel Gold"].lsSelectionRay.TriggerRay(lSRayEndPosition);

    }

    private void StartAfterMiracleAnimations()
    {
        StartCoroutine(HandleAfterMiracleAnimations());
    }

    private IEnumerator HandleAfterMiracleAnimations()
    {
        // stop ls rays
        LightSpiritsController.instance.DisableAllLSRays();

        // path starts appearing
        EventManager.TriggerEvent(Global.Level4_Events.STONE_APPEAR);

        yield return new WaitForSecondsRealtime(timeAfterLsDisablingRays);

        // lightspirits - lower their arms
        LightSpiritsController.instance.SetActionToAllLightSpirits((int)LightSpiritsController.LSAnimations.UNLOAD_LIGHT_RAY);

        yield return new WaitForSecondsRealtime(3f);

        LightSpiritsController.instance.SetActionToAllLightSpirits((int)LightSpiritsController.LSAnimations.FLYING);

        //rotate, once completed start going in circles
        yield return new WaitForSeconds(.5f);

        Level4_EASTER_LSAnimationsAroundPlatform.instance.RotateLightSpiritsAroundCenter();

        // show meditation circle
        EventManager.TriggerEvent(Global.Shared_Events.SHOW_MEDITATION_CIRCLE);
    }
}
