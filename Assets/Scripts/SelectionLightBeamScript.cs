using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SelectionLightBeamScript : MonoBehaviour
{

    public Hand rightHand;
    public SteamVR_Input_Sources thisInputSource = SteamVR_Input_Sources.RightHand;

    public SteamVR_Action_Boolean selectionRayAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("SelectionRay");
    public SteamVR_Action_Boolean cancelSelectionAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("CancelSelection");

    public TrashObjectsHandling trashObjectHandling;

    //public GameObject parentHand;
    public GameObject lineRendererObject;
    public GameObject surroundingPSObject;
    public GameObject startingPointPSObject;

    // variables to hold data
    public float maxLRWidth = 2.0f;
    public float maxLRLength = 10.0f;
    public float maxParticleSurBeamLifetime = 5.0f;
    public float minParticleSurBeamLifetime = 1.0f;
    public float maxSpeedParticle = 3.0f;

    public float startingSpeedParticle = 0.0f;
    public float startingMaxSurroundingPSLifetime = 1.0f;
    public float startingLRLength = 0.2f;
    public float startingLRWidth = 1.0f;

    // time it takes to skip level
    public float timeStarting = 5.0f;
    public float timeProgressing = 10.0f;
    public float timeTurn_Off = 2.0f;

    // current times
    public float currentTimeStarting = 0.0f;
    public float currentTimeProgressing = 0.0f;
    public float currentTimeTurn_Off = 0.0f;

    private float currentLRLength = 0.0f;
    // will not be used

    public enum SelectionRayMode
    {
        STARTING,
        PROGRESSING,
        MAX_REACH,
        TURN_OFF,
        DISABLED
    }

    public SelectionRayMode currentMode = SelectionRayMode.DISABLED;

    private LineRenderer lineRendererBeam;
    private Vector3 finalLRPosition;

    private ParticleSystem surroundingPSBeam;
    private ParticleSystem.MainModule surroundingPSBeamMain;

    private ParticleSystem startingPointPSBeam;
    private ParticleSystem.MainModule startingPointPSBeamMain;

    public bool isRayActivated = false;
    public bool isHittingObjects = false;
    private int trashLayer;
    private int layersToIgnoreWhenRaycastingDistance;
    // private int 
    private float rayRadius = 0.2f;
    private RaycastHit[] hitObjects;
    private bool otherObjectsNotHitBefore = false;

    private void Awake() {
        if (rightHand == null) {
            rightHand = GameObject.FindGameObjectWithTag("RightHand").GetComponent<Hand>();
        }
    }
    
    // Use this for initialization
    void Start()
    {
        lineRendererBeam = lineRendererObject.GetComponent<LineRenderer>();
        finalLRPosition = lineRendererBeam.GetPosition(1);

        surroundingPSBeam = surroundingPSObject.GetComponent<ParticleSystem>();
        surroundingPSBeamMain = surroundingPSBeam.main;

        startingPointPSBeam = startingPointPSObject.GetComponent<ParticleSystem>();
        startingPointPSBeamMain = startingPointPSBeam.main;

        trashLayer = LayerMask.GetMask("trash");
        
        // layers to ignore: 
        // 2 - Ignore Raycast;
        // 8 - trash;
        // 13 - Ignore everything except player;
        layersToIgnoreWhenRaycastingDistance = ~( (1 << 2) | (1 << 8) | (1 << 13) );
        disableSelectionRay();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectionRayAction.GetStateDown(thisInputSource))
        {
            // enable things to start Ray
            lineRendererObject.SetActive(true);
            surroundingPSObject.SetActive(true);
            currentLRLength = maxLRLength;
            surroundingPSBeamMain.startSpeed = maxSpeedParticle;

            finalLRPosition.z = maxLRLength;
            lineRendererBeam.SetPosition(1, finalLRPosition);
            isRayActivated = true;
            isHittingObjects = true;
        }
        if (selectionRayAction.GetState(thisInputSource))
        {
            // TODO: ray animation - starting, progressing, max reach

            // haptics
            rightHand.TriggerHapticPulse(750);
        }
        if (selectionRayAction.GetStateUp(thisInputSource))
        {
            // TODO: animation turnOff
            StartCoroutine(TurnOffRayAnimation());
        }
        if (isRayActivated) {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            float maxDistance = maxLRLength;

            // calculate distance based On Hit
            if (Physics.Raycast(ray, out hit, maxLRLength, layersToIgnoreWhenRaycastingDistance)) {
                maxDistance = hit.distance;
            }

            // animation - line length and surrounding beam distance
            finalLRPosition.z = maxDistance;
            lineRendererBeam.SetPosition(1, finalLRPosition);
            surroundingPSBeamMain.startSpeed = maxDistance / maxParticleSurBeamLifetime;
            
            // hit trash objects
            if (isHittingObjects) {
                StartCoroutine(HitTrashObjects(ray, maxDistance));
                HitOtherObjects(ray, maxDistance);
            }
        }

        if (cancelSelectionAction.GetStateDown(thisInputSource) && trashObjectHandling.anyObjectSelected()) {
            // since there is no hits, it will cancel all selected objects back to normal            
            trashObjectHandling.TriggerSelection();
            // haptics
            rightHand.TriggerHapticPulse(0.5f, 10.0f, 15.0f);
            
            // for tutorials in level 1
            EventManager.TriggerEvent(Global.GetSharedHintString(Global.Shared_Hints.TUT_CANCELSELECTION));
        }
    }

    private void HitOtherObjects(Ray ray, float maxDistance)
    {
        // different rays regarding different situations
        if (!otherObjectsNotHitBefore && Global.currentLevel == Global.ThisLevelNbr.L4) {
            
            // level 4, trigger animations when player hits falling trash
            // ignore everything except player
            if (Physics.SphereCast(ray, rayRadius, maxDistance, (1 << 13))) {
                DebugManager.Info(Global.Level4_Events.PLAYER_HIT_TRASH);
                EventManager.TriggerEvent(Global.Level4_Events.PLAYER_HIT_TRASH);
                otherObjectsNotHitBefore = true;
            }

        }
    }

    private IEnumerator HitTrashObjects(Ray ray, float maxDistance)
    {
        if (trashObjectHandling != null) {
            isHittingObjects = false;
            hitObjects = Physics.SphereCastAll(ray, rayRadius, maxDistance, trashLayer);
            if (hitObjects.Length != 0) {
                foreach (RaycastHit obj in hitObjects)
                {
                    trashObjectHandling.HitObject(obj.transform.name);
                }
                trashObjectHandling.TriggerSelection();
                
                // for tutorials in level 1
                EventManager.TriggerEvent(Global.GetSharedHintString(Global.Shared_Hints.TUT_SELECTIONRAY));
            }
            yield return new WaitForSeconds(.5f);
            isHittingObjects = true;
        }
    }

    private IEnumerator TurnOffRayAnimation()
    {
        yield return new WaitForEndOfFrame();
        disableSelectionRay();
        // throw new NotImplementedException();
    }

    public void disableSelectionRay()
    {
        finalLRPosition.z = startingLRLength;
        lineRendererBeam.SetPosition(1, finalLRPosition);

        surroundingPSBeamMain.startSpeed = startingSpeedParticle;
        lineRendererObject.SetActive(false);
        surroundingPSObject.SetActive(false);
        isRayActivated = false;
        isHittingObjects = false;
    }
}
