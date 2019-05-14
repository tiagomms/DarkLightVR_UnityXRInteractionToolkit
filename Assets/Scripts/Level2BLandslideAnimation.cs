using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2BLandslideAnimation : MonoBehaviour {
    private static Level2BLandslideAnimation _instance;
    public static Level2BLandslideAnimation instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Level2BLandslideAnimation>();
            }

            return _instance;
        }
    }
	public GameObject triggerGravityPlane;
	public GameObject dynamicObjectsParent;

	public Dictionary<string, Rigidbody> dynamicObjectsDict;
	
	private void Awake()
	{
		_instance = this;
		if (dynamicObjectsParent != null) {
			dynamicObjectsDict = new Dictionary<string, Rigidbody>();
			Rigidbody[] allRigidBodies = dynamicObjectsParent.GetComponentsInChildren<Rigidbody>(true);
			foreach(Rigidbody rb in allRigidBodies)
			{
				dynamicObjectsDict.Add(rb.gameObject.name, rb);
			}
		}
		
		if (triggerGravityPlane != null) {
			triggerGravityPlane.SetActive(false);
		}
	}

    private void OnEnable()
    {
		// TODO: change event
        EventManager.StartListening(Global.Shared_Events.LOAD_SCENE, MoveTriggerGravityPlane);
    }
    private void OnDisable()
    {
        // TODO: change event
        EventManager.StopListening(Global.Shared_Events.LOAD_SCENE, MoveTriggerGravityPlane);
    }

	void MoveTriggerGravityPlane() {
		triggerGravityPlane.SetActive(true);
		LeanTween.moveY(triggerGravityPlane, -36f, 15f)
			.setEaseLinear()
			.setOnComplete(HideTriggerGravityPlane);
	}

    private void HideTriggerGravityPlane()
    {
		triggerGravityPlane.SetActive(false);
    }
}

