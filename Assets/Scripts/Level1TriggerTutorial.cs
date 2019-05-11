using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Level1TriggerTutorial : MonoBehaviour
{
		public bool alwaysTriggerSound = false;
    [Range(0f, 10f)]
    public float delayBeforeSoundSeconds;

    public float delayBeforeEvent;

    public Global.Shared_Hints thisTutorial = Global.Shared_Hints.NONE;
		public bool onceActionDoneBlockSound = false;
    private SteamVR_Action_Boolean thisTutorialAction = null;
    private string tutorialName;
		private string enableActionEventName;

    private AudioFile audioFile;

    private bool soundWasTriggeredBefore = false;
    private bool actionWasTriggeredBefore = false;
    // Use this for initialization
    void Awake()
    {
        audioFile = gameObject.GetComponent<AudioFile>();
        SetTutorialParameters();
    }

		private void OnDisable()
		{
        EventManager.StopListening(tutorialName, HideHintAndStopListeningThisSpotEvent);
    }

    private void SetTutorialParameters()
    {
        tutorialName = thisTutorial != 0 ? Global.GetSharedHintString(thisTutorial) : null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Body")
        {
						DebugManager.Info("In Tutorial Name: " + tutorialName);
						if (!actionWasTriggeredBefore)
            {
                actionWasTriggeredBefore = true;                
								StartCoroutine(ShowHintAndStartListeningThisSpotEvent());
            }
						if (alwaysTriggerSound || !soundWasTriggeredBefore) {
								soundWasTriggeredBefore = true;
								StartCoroutine(PlayThisSpotSound());
						}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Body")
        {
            if (audioFile != null && audioFile.source != null && audioFile.source.isPlaying) {
								AudioManager.StopAudioFile(audioFile.audioName);
                // get music back
                AudioManager.ResetAllVols();
								soundWasTriggeredBefore = false;
								if (!Level1TutorialManager.instance.ContainsTutorial(thisTutorial) && onceActionDoneBlockSound) {
										soundWasTriggeredBefore = true;
								}
						}  
						// if (tutorialName != null && !EventManager.IsEventInDictionary(tutorialName)) {
						if (tutorialName != null && Level1TutorialManager.instance.ContainsTutorial(thisTutorial)) {
								actionWasTriggeredBefore = false;
						}

						// only works for teleport because they are leaving the area
						if (thisTutorial == Global.Shared_Hints.TUT_TELEPORT) {
								EventManager.TriggerEvent(Global.GetSharedHintString(Global.Shared_Hints.TUT_TELEPORT));
						}
						
        }
    }

    private IEnumerator ShowHintAndStartListeningThisSpotEvent()
    {
        // if (tutorialName != null && 
				// 		(Level1TutorialManager.instance.ContainsTutorial(thisTutorial) ||
				// 		thisTutorial == Global.Shared_Hints.TUT_TELEPORT)) {
        if (Level1TutorialManager.instance.ContainsTutorial(thisTutorial)) {
						// delay
						yield return new WaitForSecondsRealtime(delayBeforeSoundSeconds + delayBeforeEvent);
						if (actionWasTriggeredBefore) {
								// enable control
								switch (thisTutorial)
								{
										case Global.Shared_Hints.TUT_TELEPORT:
												Global.Shared_Controllers.TELEPORT = true;
												break;
										case Global.Shared_Hints.TUT_HINTMENU:
												Global.Shared_Controllers.HINTMENU = true;
												break;
										case Global.Shared_Hints.TUT_SELECTIONRAY:
												Global.Shared_Controllers.SELECTION_RAY = true;
												break;
										case Global.Shared_Hints.TUT_CANCELSELECTION:
												Global.Shared_Controllers.SELECTION_RAY = true;
												break;
										case Global.Shared_Hints.TUT_GOAWAY:
												Global.Shared_Controllers.VOICECOMMAND = true;
												break;
										case Global.Shared_Hints.TUT_IAMREADY:
												Global.Shared_Controllers.VOICECOMMAND = true;
												break;
										default:
												break;
								}

								EventManager.TriggerEvent(Global.Shared_Events.SET_TELEPORT);
								EventManager.TriggerEvent(Global.Shared_Events.SET_HINTMENU);
								EventManager.TriggerEvent(Global.Shared_Events.SET_SELECTION_RAY);
								EventManager.TriggerEvent(Global.Shared_Events.SET_VOICECOMMAND);

								yield return new WaitForEndOfFrame();
								ControllerHintsManager.instance.ShowSpecificTextHint(thisTutorial);

								// Even though Teleport has its own hint in Teleport.cs, once it is done, it shouldn't appear again
								// the event helps to guarantee that
								EventManager.StartListening(tutorialName, HideHintAndStopListeningThisSpotEvent);
								DebugManager.Info("Start Listening Event: " + tutorialName);
						}
        }
				yield return null;
    }

    private void HideHintAndStopListeningThisSpotEvent()
    {
        // TODO: nice job sound?
        ControllerHintsManager.instance.HideSpecificTextHint(thisTutorial);
				Level1TutorialManager.instance.RemoveActivatedTutorial(thisTutorial);
    }

    private IEnumerator PlayThisSpotSound()
    {
        if (audioFile != null && audioFile.audioName != null && audioFile.audioName.Length != 0) {
            // delay
            yield return new WaitForSecondsRealtime(delayBeforeSoundSeconds);

						AudioManager.PlayAudioFile(audioFile.audioName);
            MusicAudioFilePlayer.instance.LowerVolumeCurrentAudioFile(audioFile.audioClip.length);
            MeditationAudioFilePlayer.instance.LowerVolumeCurrentAudioFile(audioFile.audioClip.length);
						DebugManager.Info("Sound Played: " + audioFile.audioName);
				}
				yield return null;
    }
}
