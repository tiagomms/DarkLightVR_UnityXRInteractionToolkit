using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level5EventManager : MonoBehaviour {

	public LightSpiritAnimationManager lightSpiritNarrator;
	public Transform vrPlayerRig;
	public GameObject musicPlayer;

	private void Awake()
	{
		if (lightSpiritNarrator != null) {
			DontDestroyOnLoad(lightSpiritNarrator.gameObject);
		}

		if (musicPlayer != null) {
			DontDestroyOnLoad(musicPlayer);
		}
	}
	private void Start()
	{
		// Vector3 targetDir = vrPlayerRig.transform.position - lightSpiritNarrator.transform.position;
		// Vector3 lSDir = Vector3.RotateTowards(lightSpiritNarrator.transform.forward, targetDir, 2*Mathf.PI, 2 * Mathf.PI);


		// lightSpiritNarrator.transform.LookAt(vrPlayerRig, Vector3.up);
		lightSpiritNarrator.SetActionByValue((int)LightSpiritsController.LSAnimations.TALKING);
	}

	private void OnEnable() {
        EventManager.StartListening(Global.Shared_Events.NARRATOR_EVENT_BETWEEN_CLIPS, TriggerAnimationsBetweenClips);
        EventManager.StartListening(Global.Shared_Events.CHANGE_SCENE, TriggerFinalLineOfDialog);
        EventManager.StartListening(Global.Shared_Events.NARRATOR_ENDED, HandleNarratorEnded);
    }
	private void OnDisable() {
        EventManager.StopListening(Global.Shared_Events.NARRATOR_EVENT_BETWEEN_CLIPS, TriggerAnimationsBetweenClips);
        EventManager.StopListening(Global.Shared_Events.CHANGE_SCENE, TriggerFinalLineOfDialog);
        EventManager.StopListening(Global.Shared_Events.NARRATOR_ENDED, HandleNarratorEnded);
    }

    private void TriggerAnimationsBetweenClips()
    {
		switch (NarratorAudioFilePlayer.instance.CLIP_INDEX)
		{
			case 1:
				StartCoroutine(TriggerShowMeditationCircleAndEnableTeleport());
				break;
			default:
				break;
		}
    }

    private IEnumerator TriggerShowMeditationCircleAndEnableTeleport()
    {
		/*
		 * Sequence of events:
		 * - enable teleport
		 * - show meditation circle
		 * - Play Remember clip
		 * - Start Music & let light spirit do random animations
		 */
		Global.Shared_Controllers.TELEPORT = true;
		EventManager.TriggerEvent(Global.Shared_Events.SET_TELEPORT);
		yield return new WaitForSecondsRealtime(2.0f);
		EventManager.TriggerEvent(Global.Shared_Events.SHOW_MEDITATION_CIRCLE);
		yield return new WaitForSecondsRealtime(2.0f);
		NarratorAudioFilePlayer.instance.PlayCurrentAudioFile();

		yield return new WaitForSecondsRealtime(5.0f);
		// music 
		MusicAudioFilePlayer.instance.PlayCurrentAudioFile();

		// start random animations
		lightSpiritNarrator.IsRandomAnimOn = true;
		StartCoroutine(lightSpiritNarrator.StartRandomAnimationCycle());
    }
    private void TriggerFinalLineOfDialog()
    {
		NarratorAudioFilePlayer.instance.PlayCurrentAudioFile();
    }

    private void HandleNarratorEnded()
    {
		LeanTween.alpha(lightSpiritNarrator.gameObject, 0, 3f)
			.setEase(LeanTweenType.easeInQuad)
			.setOnComplete(DestroyNarrator);
    }

    private void DestroyNarrator()
    {
		Destroy(lightSpiritNarrator.gameObject);
    }
}
