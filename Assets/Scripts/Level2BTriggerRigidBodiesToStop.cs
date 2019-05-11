using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2BTriggerRigidBodiesToStop : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        try
        {
            Rigidbody rb = Level2BLandslideAnimation.instance.dynamicObjectsDict[other.gameObject.name];
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        catch (System.Exception)
        {
            DebugManager.Error(other.gameObject.name + " not in dictionary");
        }
    }

}
