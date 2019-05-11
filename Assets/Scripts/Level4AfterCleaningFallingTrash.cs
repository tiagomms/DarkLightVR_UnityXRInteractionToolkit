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
	public float 		timeBetweenAnimations = 5f;
	public float 		timeForSkyboxBlendAnimation = 10f;
	private int 		currentMatIndex = 0;
	
	private void Awake()
	{
		// platformMat.color = platformStartingColor;
		if (platform != null) {
			platform.GetComponent<Renderer>().material.color = platformStartingColor;

			// setting all mats alpha to 0
			for (currentMatIndex = 0; currentMatIndex < treeMaterialsToAnimate.Length; currentMatIndex++)
			{
				AppearingMaterialAnimationUpdate(0f);
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
        for (currentMatIndex = 0; currentMatIndex < treeMaterialsToAnimate.Length; currentMatIndex++)
        {
            AppearingMaterialAnimationUpdate(1f);
        }
        BlendSkyboxUpdate(0f);
    }

    private void StartAfterMiracleAnimations()
    {
		StartCoroutine(HandleAfterMiracleAnimations());
    }

    private IEnumerator HandleAfterMiracleAnimations()
    {

		// change platform color
		LeanTween.color(platform, platformRealColor, timeForSkyboxBlendAnimation).setEase(LeanTweenType.easeInExpo);
		
		// blend skybox
		LeanTween.value(gameObject, 0f, 1f, timeForSkyboxBlendAnimation)
			.setEase(LeanTweenType.easeInExpo)
			.setOnUpdate((System.Action<float>)BlendSkyboxUpdate);

        yield return new WaitForSecondsRealtime(timeForSkyboxBlendAnimation);

		// show meditation circle
		EventManager.TriggerEvent(Global.Shared_Events.SHOW_MEDITATION_CIRCLE);

        // path starts appearing
        EventManager.TriggerEvent(Global.Level4_Events.STONE_APPEAR);

		for (currentMatIndex = 0; currentMatIndex < treeMaterialsToAnimate.Length; currentMatIndex++)
		{
            LeanTween.value(gameObject, 0f, 1f, timeBetweenAnimations)
				.setEase(LeanTweenType.easeInExpo)
				.setOnUpdate((System.Action<float>)AppearingMaterialAnimationUpdate);
			yield return new WaitForSecondsRealtime(timeBetweenAnimations);
		}

    }

    private void BlendSkyboxUpdate(float blendValue)
    {
		if (blendedSkybox != null) {
        	blendedSkybox.SetFloat("_Blend", blendValue);
		}
    }

    private void AppearingMaterialAnimationUpdate(float alpha)
    {
		Color curColor = treeMaterialsToAnimate[currentMatIndex].color;
		curColor.a = alpha;
        treeMaterialsToAnimate[currentMatIndex].color = curColor;
    }
}
