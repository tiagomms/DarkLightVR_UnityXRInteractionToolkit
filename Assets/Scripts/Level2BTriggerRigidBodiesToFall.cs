using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2BTriggerRigidBodiesToFall : MonoBehaviour {
	private void OnTriggerExit(Collider other) {
		try
		{
			Rigidbody rb = Level2BLandslideAnimation.instance.dynamicObjectsDict[other.gameObject.name];
			StartCoroutine(StartObjectFall(rb));
		}
		catch (System.Exception)
		{
			DebugManager.Error(other.gameObject.name + " not in dictionary");
		}
	}

    private IEnumerator StartObjectFall(Rigidbody rb)
    {
		yield return new WaitForFixedUpdate();
		rb.isKinematic = false;
		rb.useGravity = true;
    }
}
