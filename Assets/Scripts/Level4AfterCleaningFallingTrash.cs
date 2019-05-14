using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4AfterCleaningFallingTrash : MonoBehaviour {

	public GameObject 	platform;
	public Color 		platformStartingColor;
	public Color 		platformRealColor;

	private Material 	blendedSkybox;

	public Material[] 	treeMaterialsToAnimate;
	public float 		timeBetweenAnimations = 4f;
	public float 		timeForSkyboxBlendAnimation = 10f;
	
	private void Awake()
	{
		// platformMat.color = platformStartingColor;
		if (platform != null) {
			platform.GetComponent<Renderer>().material.color = platformStartingColor;

			// setting all mats alpha to 0
			for (int i = 0; i < treeMaterialsToAnimate.Length; i++)
			{
				AppearingMaterialAnimationUpdate(0f, treeMaterialsToAnimate[i]);
			}

			blendedSkybox = RenderSettings.skybox;
		}
	}

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level4_Events.AFTER_MIRACLE_OCCURED, StartAfterMiracleAnimations);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level4_Events.AFTER_MIRACLE_OCCURED, StartAfterMiracleAnimations);

		// to make sure materials stay ok
        for (int i = 0; i < treeMaterialsToAnimate.Length; i++)
        {
            AppearingMaterialAnimationUpdate(1f, treeMaterialsToAnimate[i]);
        }
        BlendSkyboxUpdate(0f);
    }

    private void StartAfterMiracleAnimations()
    {
		StartCoroutine(HandleAfterMiracleAnimations());
    }

    private IEnumerator HandleAfterMiracleAnimations()
    {
		// set trashobjects handling in player selection ray
		Level4AnimationsManager.instance.SetTrashObjectsHandlingIntoPlayerSelectionRay();

		// enable player on everything
        ToggleControllersManager.instance.EnablePlayerOnEverything();
		
        // stop ls rays
        LightSpiritsController.instance.DisableAllLSRays();

		// change platform color
		LeanTween.color(platform, platformRealColor, timeForSkyboxBlendAnimation).setEase(LeanTweenType.easeInExpo);
		
		// blend skybox
		LeanTween.value(gameObject, 0f, 1f, timeForSkyboxBlendAnimation)
			.setEase(LeanTweenType.easeInExpo)
			.setOnUpdate((System.Action<float>)BlendSkyboxUpdate);

        yield return new WaitForSecondsRealtime(timeForSkyboxBlendAnimation);

        // lightspirits - lower their arms
        LightSpiritsController.instance.SetActionToAllLightSpirits((int)LightSpiritsController.LSAnimations.UNLOAD_LIGHT_RAY);

        // fade light spirits
        LightSpiritsController.instance.FadeOutAllLightSpirits();

        // pop light spirits in next location
        StartCoroutine(Level4LSAnimationsAroundPlatform.instance.StartLsAnimationsAroundPlatform());

        // show meditation circle
        EventManager.TriggerEvent(Global.Shared_Events.SHOW_MEDITATION_CIRCLE);

		for (int i = 0; i < treeMaterialsToAnimate.Length; i++)
		{
            LeanTween.value(gameObject, 0f, 1f, timeBetweenAnimations)
				.setEase(LeanTweenType.easeInExpo)
				.setOnUpdate((System.Action<float, object>)AppearingMaterialAnimationUpdate)
				.setOnUpdateParam(treeMaterialsToAnimate[i]);
			yield return new WaitForSecondsRealtime(timeBetweenAnimations);
		}

        // path starts appearing
        EventManager.TriggerEvent(Global.Level4_Events.STONE_APPEAR);
    }
    private void BlendSkyboxUpdate(float blendValue)
    {
		if (blendedSkybox != null) {
        	blendedSkybox.SetFloat("_Blend", blendValue);
		}
    }

    private void AppearingMaterialAnimationUpdate(float alpha, object matObj)
    {
		Material mat = (Material)matObj;
		Color curColor = mat.color;
		curColor.a = alpha;
        mat.color = curColor;
    }
}
