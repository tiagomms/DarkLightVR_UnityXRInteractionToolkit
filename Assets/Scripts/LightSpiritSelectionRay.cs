using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpiritSelectionRay : MonoBehaviour {

    private static int ITERATION = 0;
	public Gradient lightRayGradient;
	public Gradient surroundingRayGradientOverLifetime;
	public Gradient surroundingRayTrailGradient;

	public Transform 	aimingEndPosition;
    public Transform    lSUpperArmBone;

    public GameObject lineRendererObject;
    public GameObject surroundingPSObject;

    private LineRenderer lineRendererBeam;
    private Vector3 finalLRPosition;
    private float maxParticleSurBeamLifetime = 5.0f;


    private bool isRayActivated = false;

    private ParticleSystem surroundingPSBeam;
    private ParticleSystem.MainModule surroundingPSBeamMain;

    private ParticleSystem startingPointPSBeam;
    private ParticleSystem.MainModule startingPointPSBeamMain;

	private void Awake()
	{
		lineRendererBeam = lineRendererObject.GetComponent<LineRenderer>();

		surroundingPSBeam = surroundingPSObject.GetComponent<ParticleSystem>();
		surroundingPSBeamMain = surroundingPSBeam.main;

		ParticleSystem.TrailModule surroundingPSBeamTrail = surroundingPSBeam.trails;
		ParticleSystem.ColorOverLifetimeModule surroundingPSBeamColorOverLifetime = surroundingPSBeam.colorOverLifetime;

		lineRendererBeam.colorGradient = lightRayGradient;

		surroundingPSBeamColorOverLifetime.color = surroundingRayGradientOverLifetime;
        surroundingPSBeamTrail.colorOverLifetime = surroundingRayTrailGradient;
		surroundingPSBeamTrail.colorOverTrail = surroundingRayTrailGradient;
	}
	// Use this for initialization
	void Start () {
        DisableSelectionRay();		
	}
	
    private void UpdateRayFinalPosition()
    {   
        // opposite direction of the red axis
        gameObject.transform.LookAt(aimingEndPosition);

        // distance between the two points
        finalLRPosition.z = Vector3.Distance(aimingEndPosition.position, gameObject.transform.position);
        
        lineRendererBeam.SetPosition(1, finalLRPosition);
        surroundingPSBeamMain.startSpeed = finalLRPosition.z / maxParticleSurBeamLifetime;
    }

    public void TriggerRay(Transform endPosTransform = null) {
        if (endPosTransform != null) {
            aimingEndPosition = endPosTransform;
        }
		isRayActivated = true;

		lineRendererObject.SetActive(true);
        surroundingPSBeam.Play();
        // surroundingPSObject.SetActive(true);
		InvokeRepeating("UpdateRayFinalPosition", 0f, 1f);
        // DebugManager.Info("LS Ray Triggered");
	}

    public void DisableSelectionRay()
    {
        isRayActivated = false;
        lineRendererObject.SetActive(false);
        surroundingPSBeam.Stop();
        // surroundingPSObject.SetActive(false);
		CancelInvoke();
    }
}
