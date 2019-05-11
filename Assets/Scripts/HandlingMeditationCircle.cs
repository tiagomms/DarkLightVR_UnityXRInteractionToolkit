using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlingMeditationCircle : MonoBehaviour {

	public GameObject	meditationCircle;
	public GameObject 	playerRig;
	public Transform 	spawnLocation;

	public bool 		alwaysShowing = false;
	public float 		showingAnimationDuration = 3.0f;

	private AudioFile	meditationAudioFile;
	private Renderer 	meditationCircleRenderer;

	private void Start()
	{
		if (meditationCircle != null) {
			meditationAudioFile = meditationCircle.GetComponent<AudioFile>();
			meditationCircleRenderer = meditationCircle.GetComponent<Renderer>();
			if (alwaysShowing) {
				Global.Shared_Controllers.MEDITATION_CIRCLE_READY = true;
				AudioManager.PlayAudioFile(meditationAudioFile.audioName);
			} else {
				// make it transparent
				Color colorWithoutAlpha = meditationCircleRenderer.material.color;
				colorWithoutAlpha.a = 0;
				meditationCircleRenderer.material.color = colorWithoutAlpha;
			}
			meditationCircle.SetActive(alwaysShowing);
		}
	}

    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.SHOW_MEDITATION_CIRCLE, ShowMeditationCircle);
    }
    private void OnDisable()
    {
        EventManager.StartListening(Global.Shared_Events.SHOW_MEDITATION_CIRCLE, ShowMeditationCircle);
    }

    private void ShowMeditationCircle()
    {
		if (meditationCircle != null) {
			// always enable voice command
			Global.Shared_Controllers.VOICECOMMAND = true;
			EventManager.TriggerEvent(Global.Shared_Events.SET_VOICECOMMAND);

			// display to others it is ready
			Global.Shared_Controllers.MEDITATION_CIRCLE_READY = true;
			// if (spawnLocation != null) {
			// 	meditationCircle.transform.position = spawnLocation.position;
			// } else if (playerRig != null) {
			// 	meditationCircle.transform.position = playerRig.transform.position;
			// }
			meditationCircle.SetActive(true);
			LeanTween.alpha(meditationCircle, 1f, showingAnimationDuration).setEase(LeanTweenType.easeInQuad);
			AudioManager.FadeIn(meditationAudioFile.audioName, meditationAudioFile.volume, showingAnimationDuration);
		}
    }
}
