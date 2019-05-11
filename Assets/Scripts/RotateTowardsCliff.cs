using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsCliff : MonoBehaviour {

	// Use this for initialization
	// public float lowerRotationLimit = 120f;
	// public float higherRotationLimit = 240f;
	public GameObject 	vrPlayerRig;
	public float 		initialYRotation;
	private void Awake()
	{
		vrPlayerRig = transform.root.gameObject;
		initialYRotation = vrPlayerRig.transform.rotation.eulerAngles.y;
	}

    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.LOAD_SCENE, RotatePlayer);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Shared_Events.LOAD_SCENE, RotatePlayer);
    }
    private void RotatePlayer()
    {
		if (vrPlayerRig != null) {
			if (Global.currentLevel == Global.ThisLevelNbr.L2A || Global.currentLevel == Global.ThisLevelNbr.L2B) {
        		vrPlayerRig.transform.Rotate(Vector3.up, initialYRotation - transform.rotation.eulerAngles.y); //cliff is at 0 degrees
        		// vrPlayerRig.transform.Rotate(Vector3.up, -180f - transform.rotation.eulerAngles.y); //cliff is at 0 degrees
			}
			// if (Global.currentLevel == Global.ThisLevelNbr.L2B) {
        	// 	vrPlayerRig.transform.Rotate(Vector3.up, 0f - transform.rotation.eulerAngles.y); //cliff is at 0 degrees
			// }
		}
    }
}
