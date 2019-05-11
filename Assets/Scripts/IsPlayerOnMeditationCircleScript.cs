using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerOnMeditationCircleScript : MonoBehaviour {
    private static IsPlayerOnMeditationCircleScript _instance;
    public static IsPlayerOnMeditationCircleScript instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<IsPlayerOnMeditationCircleScript>();
            }

            return _instance;
        }
    }
    private List<string> handsOnMeditationCircle = new List<string>();
    private void Awake() {
        _instance = this;
    }
    public static bool IsPlayerSittingOnMeditationCircle() {
        return instance.handsOnMeditationCircle.Count > 0;
    }

    private void OnTriggerStay(Collider other) {
        string tag = other.gameObject.tag;

        if ((tag == "LeftHand" || tag == "RightHand") && !instance.handsOnMeditationCircle.Contains(tag)) { // Players hands
            instance.handsOnMeditationCircle.Add(tag);
            DebugManager.Info(tag + " IN meditation circle");
            // EventManager.TriggerEvent(Global.Shared_Events.IN_MEDITATION_CIRCLE);
            // DebugManager.Info("ENTER GameObject name: " + tag);
            // DebugManager.Info("Is Player Sitting On Meditation Circle: " + IsPlayerSittingOnMeditationCircle());
        }
	}
	private void OnTriggerExit(Collider other)
	{
        string tag = other.gameObject.tag;
        if ((tag == "LeftHand" || tag == "RightHand") && instance.handsOnMeditationCircle.Contains(tag)) { // Players Hands
            instance.handsOnMeditationCircle.Remove(tag);
            DebugManager.Info(tag + " OUT meditation circle");
            // // DebugManager.Info("EXIT GameObject name: " + tag);
            // if (!IsPlayerSittingOnMeditationCircle()) {
            //     EventManager.TriggerEvent(Global.Shared_Events.OUT_MEDITATION_CIRCLE);
            // }
            // DebugManager.Info("Is Player Sitting On Meditation Circle: " + IsPlayerSittingOnMeditationCircle());
        }
	}
}
