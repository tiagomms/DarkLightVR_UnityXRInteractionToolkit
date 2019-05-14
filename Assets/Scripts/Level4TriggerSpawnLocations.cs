using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4TriggerSpawnLocations : MonoBehaviour {

	private BoxCollider boxCollider;
	private void Awake()
	{
		boxCollider = gameObject.GetComponent<BoxCollider>();
	}
	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Body") {
			EventManager.TriggerEvent(Global.Level4_Events.MOVE_SPAWN_LOCATIONS);

			StartCoroutine(DestroyBoxCollider());
		}
	}

    private IEnumerator DestroyBoxCollider()
    {
		DebugManager.Info("Destroy boxCollider of " + gameObject.name);
		yield return new WaitForEndOfFrame();
		Destroy(boxCollider);
    }
}
