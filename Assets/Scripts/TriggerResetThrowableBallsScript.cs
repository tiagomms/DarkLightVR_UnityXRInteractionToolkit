using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerResetThrowableBallsScript : MonoBehaviour {

	string triggerName = "ThrowableBall";
	private void OnTriggerEnter(Collider other) {
		GameObject obj = other.gameObject;
		if (obj.name.Contains(triggerName)) {
			EventManager.TriggerEvent(Global.Level2_Events.RESET_BALL);
		}
	}
}
