using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level6AnimationsManager : MonoBehaviour {

	public FallAndStopChildRigidBodies trashOutsideParent;
	// Use this for initialization
	void Start () {
		StartCoroutine(FallTrashOutside());
	}

    private IEnumerator FallTrashOutside()
    {
        yield return new WaitForSeconds(0.5f);
		if (trashOutsideParent != null) {
			trashOutsideParent.FallAndStopChildRigidbodiesAnimation();
		}
    }
}
