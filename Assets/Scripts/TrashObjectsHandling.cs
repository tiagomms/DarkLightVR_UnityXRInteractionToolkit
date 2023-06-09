﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashObjectsHandling : MonoBehaviour {

    private static TrashObjectsHandling _instance;
    public static TrashObjectsHandling instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TrashObjectsHandling>();
            }

            return _instance;
        }
    }
    private const string TRASH_MATERIALS_PATH = "Materials/Shared/TrashMaterials/";
    public const float notConsciousScaleIncrease = 1.2f;
    private const float notConsciousMaxScale = 2.1f;

    public Dictionary<string, TrashGODetails> trashGODict = new Dictionary<string, TrashGODetails>();
    public Material[] normalMaterials;
    public Material[] selectedMaterials;
    public Material[] fadingMaterials;
    public Material[] almostGoneMaterials;

    // utility if all objects are normal and disabled, no need to run the update function
    private int nbrNormalOrInactiveObjects = 0;    
    private float beforeFadingRimPower = 3.0f;
    public float becomingConsciousFadingAlphaDecrease = 0.75f;
    private float minObjectFadingAlpha = 0f;


    private void Awake()
    {
        _instance = this;
        SetupTrashMaterials();
        SetupTrashObjects();
    }

    private void SetupTrashMaterials()
    {
        string animatedMaterialsPath = TRASH_MATERIALS_PATH + "CONSCIOUSNESS_LEVEL/" + Global.GetConsciousnessLevelString(Global.ConsciousLevel) + "/";
        // setup materials - load them from their respective folders
        instance.normalMaterials = Resources.LoadAll<Material>(TRASH_MATERIALS_PATH + "0_NORMAL/");
        instance.selectedMaterials = Resources.LoadAll<Material>(animatedMaterialsPath + "1_SELECTED/");
        instance.fadingMaterials = Resources.LoadAll<Material>(animatedMaterialsPath + "2_FADING/");

        // if consciousness level is becoming, objects do not fade completely
        if (Global.ConsciousLevel == Global.ConsciousnessLevel.BECOMING) {
            instance.minObjectFadingAlpha = instance.becomingConsciousFadingAlphaDecrease;
            instance.almostGoneMaterials = Resources.LoadAll<Material>(animatedMaterialsPath + "3_ALMOST_GONE/");
        }
    }

    public void SetupTrashObjects()
    {
        GameObject[] trashObjects = GameObject.FindGameObjectsWithTag("objectsToClean");
        foreach (GameObject obj in trashObjects)
        {
            AddTrashObjectsDictionaryEntry(obj);
        }
        instance.nbrNormalOrInactiveObjects = instance.trashGODict.Count;
    }

    public void AddTrashObjectsDictionaryEntry(GameObject obj)
    {
        if (!instance.trashGODict.ContainsKey(obj.name)) {

            TrashGODetails trashGODetails = new TrashGODetails();
            Renderer rend = obj.GetComponent<Renderer>();
            int[] matIndexes = new int[rend.sharedMaterials.Length]; // to Delete

            trashGODetails.GONormalMats = new Material[rend.sharedMaterials.Length];
            trashGODetails.GOSelectedMats = new Material[rend.sharedMaterials.Length];
            trashGODetails.GOFadingMats = new Material[rend.sharedMaterials.Length];
            trashGODetails.GOAlmostGoneMats = new Material[rend.sharedMaterials.Length];

            int curMatIndex = 0;
            int i = 0;

            while (curMatIndex < matIndexes.Length)
            {
                if (rend.sharedMaterials[curMatIndex].name.Contains(instance.normalMaterials[i].name))
                {
                    matIndexes[curMatIndex] = i;// to Delete
                    trashGODetails.GONormalMats[curMatIndex] = instance.normalMaterials[i];
                    trashGODetails.GOSelectedMats[curMatIndex] = instance.selectedMaterials[i];
                    trashGODetails.GOFadingMats[curMatIndex] = instance.fadingMaterials[i];

                    if (Global.ConsciousLevel == Global.ConsciousnessLevel.BECOMING)
                    {
                        trashGODetails.GOAlmostGoneMats[curMatIndex] = instance.almostGoneMaterials[i];
                    }
                    curMatIndex++;

                    i = -1;
                }
                i++;
            }

            trashGODetails.GORender = rend;
            trashGODetails.GObject = obj;
            trashGODetails.MaxGOScale = obj.transform.localScale * notConsciousMaxScale;

            instance.trashGODict.Add(obj.name, trashGODetails);
        }
        else {
            Debug.Log("TrashObjectsHandling - object already exists in Dict: '" + obj.name + "'");
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.GO_AWAY_INPUT, TriggerFading);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Shared_Events.GO_AWAY_INPUT, TriggerFading);

        // reset animations to previous values
        SelectedMaterialsAnimationUpdate(1f);

        // before fading animation, get the rim power of all objects
        instance.beforeFadingRimPower = instance.selectedMaterials[0].GetFloat("_RimPower");

        FadingMaterialsAnimationUpdate(1f);
        AlmostGoneMaterialsAnimationUpdate(1f);
    }

    private void Start()
    {
        SelectedMaterialsAnimation();
    }

    /*
     * SelectedMaterialsAnimation is a continuous animation
     */
    private void SelectedMaterialsAnimation()
    {
        LeanTween.value(gameObject, 3f, 0.1f, 2f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((System.Action<float>)SelectedMaterialsAnimationUpdate)
            .setRepeat(-1)
            .setLoopPingPong();
    }

    private void SelectedMaterialsAnimationUpdate(float value)
    {
        foreach (Material sMat in instance.selectedMaterials)
        {
            sMat.SetFloat("_RimPower", value);
        }
    }


    private void CreateFadingMaterialsAnimation()
    {   
        // before fading animation, get the rim power of all objects
        instance.beforeFadingRimPower = instance.selectedMaterials[0].GetFloat("_RimPower");

        LeanTween.value(gameObject, 1f, instance.minObjectFadingAlpha, 3f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((System.Action<float>)FadingMaterialsAnimationUpdate)
            .setOnComplete(FadingMaterialsAnimationComplete);
        
    }
    private void FadingMaterialsAnimationComplete()
    {
        if (Global.ConsciousLevel == Global.ConsciousnessLevel.FULLY) {
            // trigger fading objects to inactive
            TriggerInactive();
            // reset fading materials - set alpha to 1
            FadingMaterialsAnimationUpdate(1f);
        } else if (Global.ConsciousLevel == Global.ConsciousnessLevel.NOT) {
            TriggerReappearing();
        } else if (Global.ConsciousLevel == Global.ConsciousnessLevel.BECOMING) {
            TriggerAlmostGone();
            AlmostGoneMaterialsAnimationUpdate(instance.minObjectFadingAlpha);
            // reset fading materials
            FadingMaterialsAnimationUpdate(1f);
            // decrease min Object Fading Alpha
            instance.minObjectFadingAlpha = Mathf.Max(instance.minObjectFadingAlpha * instance.becomingConsciousFadingAlphaDecrease, 0.2f);
        }
    }

    private void FadingMaterialsAnimationUpdate(float value)
    {
        foreach(Material fMat in instance.fadingMaterials) {
            Color tintColor = fMat.GetColor("_ColorTint");
            Color rimColor = fMat.GetColor("_RimColor");
            Color outlineColor = fMat.GetColor("_OutlineColor");

            tintColor.a = value;
            outlineColor.a = value;
            rimColor.a = value;

            fMat.SetColor("_ColorTint", tintColor);
            fMat.SetColor("_RimColor", rimColor);
            fMat.SetColor("_OutlineColor", outlineColor);

            // rim power based on before fading value;
            fMat.SetFloat("_RimPower", instance.beforeFadingRimPower);
        }
    }
    private void AlmostGoneMaterialsAnimationUpdate(float value)
    {
        foreach(Material agMat in instance.almostGoneMaterials) {
            Color tintColor = agMat.GetColor("_ColorTint");
            Color rimColor = agMat.GetColor("_RimColor");
            Color outlineColor = agMat.GetColor("_OutlineColor");

            tintColor.a = value;
            outlineColor.a = value;
            rimColor.a = value;

            agMat.SetColor("_ColorTint", tintColor);
            agMat.SetColor("_RimColor", rimColor);
            agMat.SetColor("_OutlineColor", outlineColor);

            // rim power based on before fading value;
            agMat.SetFloat("_RimPower", instance.beforeFadingRimPower);
        }
    }

    // only for levels with no conscience, the object appears to reappear
    private void CreateReappearingMaterialsAnimation()
    {
        LeanTween.value(gameObject, 0f, 1f, 1.5f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((System.Action<float>)FadingMaterialsAnimationUpdate)
            .setOnComplete(ReappearingMaterialsAnimationComplete);
        
    }
    private void ReappearingMaterialsAnimationComplete()
    {
        TriggerBackToNormal(TrashGOMode.REAPPEARING);
    }

    private void ChangeTrashObjectToNormal(TrashGODetails objDetails)
    {
        objDetails.PreviousGOMode = objDetails.GOMode;
        objDetails.GOMode = TrashGOMode.NORMAL;
        objDetails.GORender.sharedMaterials = objDetails.GONormalMats;
        
        // increase Normal Objects;
        instance.nbrNormalOrInactiveObjects++;
    }

    private void ChangeTrashObjectToSelected(TrashGODetails objDetails)
    {
        objDetails.PreviousGOMode = objDetails.GOMode;
        objDetails.GOMode = TrashGOMode.SELECTED;
        objDetails.GORender.sharedMaterials = objDetails.GOSelectedMats;

        // decrease normal objects;
        instance.nbrNormalOrInactiveObjects--;        
    }
    private void ChangeTrashObjectToFading(TrashGODetails objDetails)
    {
        objDetails.PreviousGOMode = objDetails.GOMode;
        objDetails.GOMode = TrashGOMode.FADING;
        objDetails.GORender.sharedMaterials = objDetails.GOFadingMats;
    }
    private void ChangeTrashObjectToAlmostGone(TrashGODetails objDetails)
    {
        objDetails.PreviousGOMode = objDetails.GOMode;
        objDetails.GOMode = TrashGOMode.ALMOST_GONE;
        objDetails.GORender.sharedMaterials = objDetails.GOAlmostGoneMats;

        // these can be selected
        instance.nbrNormalOrInactiveObjects++;
    }
    private void ChangeTrashObjectToInactive(TrashGODetails objDetails)
    {
        objDetails.PreviousGOMode = objDetails.GOMode;
        objDetails.GObject.SetActive(false);
        objDetails.GORender.sharedMaterials = objDetails.GONormalMats;
        
        // increase Normal Objects;
        instance.nbrNormalOrInactiveObjects++;
    }


    internal void HitObject(string name)
    {
        if (instance.trashGODict.ContainsKey(name)) {
            instance.trashGODict[name].IsHit = true;
        } else {
            Debug.Log("TrashObjectsHandling - object DOES NOT EXIST in Dict: '" + name + "'");
        }
    }

    internal void TriggerSelection(bool forceCancel = false)
    {
        foreach(KeyValuePair<string, TrashGODetails> keyValuePair in instance.trashGODict) {
            TrashGODetails objDetails = keyValuePair.Value;
            
            // handle Hit
            if (objDetails.IsHit && !forceCancel) {
                if (objDetails.GOMode == TrashGOMode.NORMAL || objDetails.GOMode == TrashGOMode.ALMOST_GONE)
                {
                    ChangeTrashObjectToSelected(objDetails);
                }

                // when super power is on, and no request for cancelling was made - object stays it
                if (Global.Shared_Controllers.ENDED_GAME) {
                    continue;
                }

            } else {
                if (objDetails.GOMode == TrashGOMode.SELECTED)
                {
                    if (objDetails.PreviousGOMode == TrashGOMode.NORMAL) {
                        ChangeTrashObjectToNormal(objDetails);
                    } else if (objDetails.PreviousGOMode == TrashGOMode.ALMOST_GONE) {
                        ChangeTrashObjectToAlmostGone(objDetails);
                    }
                }
            }

            // default behaviour - reset booleans
            objDetails.IsHit = false;


        }
    }
    internal void TriggerFading()
    {
        bool anyObjectFading = false;
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in instance.trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            // handle Transparency
            if (objDetails.GOMode == TrashGOMode.SELECTED) {
                ChangeTrashObjectToFading(objDetails);
                anyObjectFading = true;
            }
        }
        // trigger fading animation
        if (anyObjectFading) {
            CreateFadingMaterialsAnimation();
            // trigger hint go away
            EventManager.TriggerEvent(Global.GetSharedHintString(Global.Shared_Hints.TUT_GOAWAY));
        }
    }
    internal void TriggerInactive()
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in instance.trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            // handle Inactive
            if (objDetails.GOMode == TrashGOMode.FADING) {
                ChangeTrashObjectToInactive(objDetails);
            }
        }
    }
    internal void TriggerReappearing()
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in instance.trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            if (objDetails.GOMode == TrashGOMode.FADING) {
                // increase object scale
                objDetails.GOMode = TrashGOMode.REAPPEARING;
                objDetails.GObject.transform.localScale *= notConsciousScaleIncrease;
                // to stop increasing indefinitively - max
                if (objDetails.GObject.transform.localScale.x > objDetails.MaxGOScale.x) {
                    objDetails.GObject.transform.localScale = objDetails.MaxGOScale;
                }
            }
        }
        CreateReappearingMaterialsAnimation();
    }
    internal void TriggerBackToNormal(TrashGOMode trashMode)
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in instance.trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            if (objDetails.GOMode == trashMode) {
                ChangeTrashObjectToNormal(objDetails);
            }
        }
    }

    private void TriggerAlmostGone()
    {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in instance.trashGODict)
        {
            TrashGODetails objDetails = keyValuePair.Value;
            if (objDetails.GOMode == TrashGOMode.FADING)
            {
                ChangeTrashObjectToAlmostGone(objDetails);
            }
        }
    }

    internal bool anyObjectSelected() {
        return instance.nbrNormalOrInactiveObjects != instance.trashGODict.Count;
    }
}
