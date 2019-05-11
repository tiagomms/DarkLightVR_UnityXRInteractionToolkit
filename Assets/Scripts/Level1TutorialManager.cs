using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Level1TutorialManager : MonoBehaviour {
    private static Level1TutorialManager _instance;
    public static Level1TutorialManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Level1TutorialManager>();
            }

            return _instance;
        }
    }

	// private Dictionary<Global.Level1_Tutorials, SteamVR_Action_Boolean> tutorialsDict;
	private HashSet<Global.Shared_Hints> activatedTutorialsSet = new HashSet<Global.Shared_Hints>();

	private bool isFirstGoAway = true;
	private bool isFirstNiceGoing = true;

    void Awake()
    {
        _instance = this;

		CreateActivatedTutorialsSet();
    }

	private void OnEnable()
	{
		EventManager.StartListening(Global.GetSharedHintString(Global.Shared_Hints.TUT_GOAWAY), PlayPepTalkGoodJob);
		EventManager.StartListening(Global.GetSharedHintString(Global.Shared_Hints.TUT_TELEPORT), PlayPepTalkNiceGoing);
	}
	private void OnDisable()
	{
		EventManager.StopListening(Global.GetSharedHintString(Global.Shared_Hints.TUT_GOAWAY), PlayPepTalkGoodJob);
		EventManager.StopListening(Global.GetSharedHintString(Global.Shared_Hints.TUT_TELEPORT), PlayPepTalkNiceGoing);
	}

    private void PlayPepTalkNiceGoing()
    {
		if (isFirstNiceGoing) {
			AudioManager.PlayDelayedAudioFile("NiceGoing", 1f);
			EventManager.StopListening(Global.GetSharedHintString(Global.Shared_Hints.TUT_TELEPORT), PlayPepTalkNiceGoing);
			isFirstNiceGoing = false;
		}
    }

    private void PlayPepTalkGoodJob()
    {
		if (isFirstGoAway) {
			AudioManager.PlayDelayedAudioFile("GoodJob", 3f);
			isFirstGoAway = false;
		}
    }

    public void HideGrabHint() {
        // for tutorials in level 1
        EventManager.TriggerEvent(Global.GetSharedHintString(Global.Shared_Hints.TUT_GRAB));
	}
	public void HideThrowHint() {
        // for tutorials in level 1
        EventManager.TriggerEvent(Global.GetSharedHintString(Global.Shared_Hints.TUT_THROW));
	}

    private void CreateActivatedTutorialsSet()
    {
		activatedTutorialsSet.Add(Global.Shared_Hints.TUT_TELEPORT);
		activatedTutorialsSet.Add(Global.Shared_Hints.TUT_SELECTIONRAY);
		activatedTutorialsSet.Add(Global.Shared_Hints.TUT_CANCELSELECTION);
		activatedTutorialsSet.Add(Global.Shared_Hints.TUT_GRAB);
		activatedTutorialsSet.Add(Global.Shared_Hints.TUT_THROW);
		activatedTutorialsSet.Add(Global.Shared_Hints.TUT_HINTMENU);
		activatedTutorialsSet.Add(Global.Shared_Hints.TUT_GOAWAY);
		activatedTutorialsSet.Add(Global.Shared_Hints.TUT_IAMREADY);
    }
	public void RemoveActivatedTutorial(Global.Shared_Hints tut) {
		if (activatedTutorialsSet.Contains(tut)) {
			activatedTutorialsSet.Remove(tut);
			DebugManager.Info("Nbr of activated tutorials: " + activatedTutorialsSet.Count);
		}
	}

	public bool ContainsTutorial(Global.Shared_Hints tut) { return activatedTutorialsSet.Contains(tut); }
	private void Start() {
		InvokeRepeating("TriggerTutorialsComplete", 60f, 5f);
	}

	public void TriggerTutorialsComplete() {
		if (activatedTutorialsSet.Count == 1 || (activatedTutorialsSet.Count == 2 && activatedTutorialsSet.Contains(Global.Shared_Hints.TUT_CANCELSELECTION))) {
			EventManager.TriggerEvent(Global.Shared_Events.SHOW_MEDITATION_CIRCLE);
			CancelInvoke("TriggerTutorialsComplete");
		}
	}
}
