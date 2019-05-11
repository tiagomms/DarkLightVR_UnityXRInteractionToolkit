using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallAndStopChildRigidBodies : MonoBehaviour {

	public float 		timeUntilStoppage = 3.0f;
	private Rigidbody[] childRBs;

	private void Awake()
	{
		childRBs = gameObject.GetComponentsInChildren<Rigidbody>();
		
		StartCoroutine(StopChildRBObjects(0f));
	}

    private IEnumerator StopChildRBObjects(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        foreach (Rigidbody rb in childRBs)
        {
            rb.isKinematic = true;
			rb.useGravity = false;
        }
    }

    public void FallAndStopChildRigidbodiesAnimation() {
		foreach (Rigidbody rb in childRBs) { rb.isKinematic = false; rb.useGravity = true; }
		StartCoroutine(StopChildRBObjects(timeUntilStoppage));
	}
}
