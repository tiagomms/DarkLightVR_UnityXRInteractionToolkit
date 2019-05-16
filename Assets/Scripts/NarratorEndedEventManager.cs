using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarratorEndedEventManager : MonoBehaviour {
    public float timeBeforeTriggeringEvent = 3f;
    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.NARRATOR_ENDED, HandleNarratorEnded);
    }
    private void OnDisable()
    {
        EventManager.StartListening(Global.Shared_Events.NARRATOR_ENDED, HandleNarratorEnded);
    }

    private void HandleNarratorEnded()
    {
        if (Global.currentLevel == Global.ThisLevelNbr.L2B || Global.currentLevel == Global.ThisLevelNbr.L2B_EASTER || Global.currentLevel == Global.ThisLevelNbr.L3) {
            StartCoroutine(TriggerShowMeditationCircle());
        }
        if (Global.currentLevel == Global.ThisLevelNbr.L2A)
        {
            StartCoroutine(TriggerChangeScene());
        }

    }

    private IEnumerator TriggerShowMeditationCircle()
    {
        yield return new WaitForSecondsRealtime(timeBeforeTriggeringEvent);
        EventManager.TriggerEvent(Global.Shared_Events.SHOW_MEDITATION_CIRCLE);
    }
    private IEnumerator TriggerChangeScene()
    {
        yield return new WaitForSecondsRealtime(timeBeforeTriggeringEvent);
        EventManager.TriggerEvent(Global.Shared_Events.CHANGE_SCENE);
    }
}
