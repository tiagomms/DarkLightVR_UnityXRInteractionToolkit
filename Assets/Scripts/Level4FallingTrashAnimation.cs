using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4FallingTrashAnimation : MonoBehaviour {
    /*
     * not efficient at all, it is a copy of the TrashObjectHandling class but for this problem
     */
	private const string TRASH_MATERIALS_PATH = "Materials/Shared/TrashMaterials/";
    private static Level4FallingTrashAnimation _instance;
    public static Level4FallingTrashAnimation instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Level4FallingTrashAnimation>();
            }

            return _instance;
        }
    }

    public Color animationStartingColor;
    public Color animationEndingColor;
    public float changingColorAnimationDuration = 15f;
    public GameObject fallingTrashParent;

    public Dictionary<string, TrashGODetails> fallingTrashDict = new Dictionary<string, TrashGODetails>();
    private TrashMaterials trashMaterials;

    // utility if all objects are normal and disabled, no need to run the update function
    private int nbrNormalOrInactiveObjects = 0;
    private float beforeFadingRimPower = 3.0f;
    private float minObjectFadingAlpha = 0f;

    private bool hasMiracleOccurred = false;
    public GameObject bogusGameObject; // to change its color, but its not viewable from the game's perspective

    private void Awake()
	{
		_instance = this;
		// currentConsciousnessLevel = Global.ConsciousnessLevel.BECOMING;

		SetupFallingTrashDictionary();	
	}
    private void Start() {
        instance.fallingTrashParent.SetActive(false);
        SelectedMaterialsAnimationUpdate(3f);
    }

    private void SetupFallingTrashDictionary()
    {
        string fallingObjectsMaterialsPath = TRASH_MATERIALS_PATH + "FALLING_OBJECTS/";

		instance.trashMaterials = new TrashMaterials();
        // setup materials - load them from their respective folders
        instance.trashMaterials.normalMaterials = Resources.LoadAll<Material>(TRASH_MATERIALS_PATH + "0_NORMAL/");
        instance.trashMaterials.selectedMaterials = Resources.LoadAll<Material>(fallingObjectsMaterialsPath + "1_SELECTED/");
        instance.trashMaterials.fadingMaterials = Resources.LoadAll<Material>(fallingObjectsMaterialsPath + "2_FADING/");

        Renderer[] childTrashObjRenderers = instance.fallingTrashParent.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in childTrashObjRenderers)
        {
            GameObject obj = rend.gameObject;
			int objMatLength = rend.sharedMaterials.Length;

            TrashGODetails fTrashGODetails = new TrashGODetails();
            fTrashGODetails.GONormalMats = new Material[objMatLength];
            fTrashGODetails.GOSelectedMats = new Material[objMatLength];
            fTrashGODetails.GOFadingMats = new Material[objMatLength];
            fTrashGODetails.GOAlmostGoneMats = new Material[objMatLength];


            int curMatIndex = 0;
            int i = 0;

            while (curMatIndex < objMatLength)
            {
                if (rend.sharedMaterials[curMatIndex].name.Contains(instance.trashMaterials.normalMaterials[i].name))
                {

                    fTrashGODetails.GONormalMats[curMatIndex] = instance.trashMaterials.normalMaterials[i];
                    fTrashGODetails.GOSelectedMats[curMatIndex] = instance.trashMaterials.selectedMaterials[i];
                    fTrashGODetails.GOFadingMats[curMatIndex] = instance.trashMaterials.fadingMaterials[i];

                    curMatIndex++;
                    i = -1;
                }
                i++;
            } 

            fTrashGODetails.GORender = rend;
            fTrashGODetails.GObject = obj;

            instance.fallingTrashDict.Add(obj.name, fTrashGODetails);
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level4_Events.TRASH_FALLING_START, TrashFallingAnimation);
        EventManager.StartListening(Global.Level4_Events.PLAYER_HIT_TRASH, SetFallingTrashAnimations);
        EventManager.StartListening(Global.Shared_Events.GO_AWAY_INPUT, TriggerFadingAnimation);

    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level4_Events.TRASH_FALLING_START, TrashFallingAnimation);
        EventManager.StopListening(Global.Level4_Events.PLAYER_HIT_TRASH, SetFallingTrashAnimations);
        EventManager.StopListening(Global.Shared_Events.GO_AWAY_INPUT, TriggerFadingAnimation);

        TrashMaterialsColorUpdate(instance.animationStartingColor);
        
        // rim power
        SelectedMaterialsAnimationUpdate(1.0f);
        // alpha
        FadingMaterialsAnimationUpdate(1.0f);
    }



    private void TrashFallingAnimation()
    {
        instance.fallingTrashParent.SetActive(true);
        LeanTween.moveY(instance.fallingTrashParent, 7f, 60f)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(VerySlowTrashFallingAnimation);
    }

    private void VerySlowTrashFallingAnimation()
    {
        LeanTween.moveY(instance.fallingTrashParent, 5f, 1200f)
            .setEase(LeanTweenType.linear);
    }    

    private void SwapTrashMaterials(TrashGOMode mode) {
        foreach (KeyValuePair<string, TrashGODetails> keyValuePair in instance.fallingTrashDict)
        {
            TrashGODetails objDetails = keyValuePair.Value;

            if (mode == TrashGOMode.NORMAL) {
                objDetails.GORender.sharedMaterials = objDetails.GONormalMats;
            }

            if (mode == TrashGOMode.SELECTED) {
                objDetails.GORender.sharedMaterials = objDetails.GOSelectedMats;
            }

            if (mode == TrashGOMode.FADING) {
                objDetails.GORender.sharedMaterials = objDetails.GOFadingMats;
            }
        }

    }

    private void SetFallingTrashAnimations()
    {
        SwapTrashMaterials(TrashGOMode.SELECTED);

        SetSelectedMaterialsAnimation();
        
        SetColorTransitionAnimation();
    }

    /*
    * SelectedMaterialsAnimation is a continuous animation
    */
    private void SetSelectedMaterialsAnimation()
    {
        LeanTween.value(instance.fallingTrashParent, 3f, 0.1f, 2f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((System.Action<float>)SelectedMaterialsAnimationUpdate)
            .setRepeat(-1)
            .setLoopPingPong();
    }

    private void SelectedMaterialsAnimationUpdate(float value)
    {
        foreach (Material sMat in instance.trashMaterials.selectedMaterials)
        {
            sMat.SetFloat("_RimPower", value);
        }
    }
    private void SetColorTransitionAnimation()
    {
        LeanTween.color(instance.bogusGameObject, instance.animationEndingColor, instance.changingColorAnimationDuration)
            .setDelay(5f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdateColor((System.Action<Color>)TrashMaterialsColorUpdate);
    }

    private void TrashMaterialsColorUpdate(Color colorUpdate)
    {
        foreach (Material sMat in instance.trashMaterials.selectedMaterials)
        {
            sMat.SetColor("_RimColor", colorUpdate);
            sMat.SetColor("_OutlineColor", colorUpdate);
        }

        foreach (Material fMat in instance.trashMaterials.fadingMaterials)
        {
            fMat.SetColor("_RimColor", colorUpdate);
            fMat.SetColor("_OutlineColor", colorUpdate);
        }
    }

    private void TriggerFadingAnimation()
    {
        SwapTrashMaterials(TrashGOMode.FADING);

        CreateFadingMaterialsAnimation();
        
        EventManager.TriggerEvent(Global.GetSharedHintString(Global.Shared_Hints.TUT_GOAWAY));
    }

    private void CreateFadingMaterialsAnimation()
    {
        // before fading animation, get the rim power of all objects
        beforeFadingRimPower = instance.trashMaterials.selectedMaterials[0].GetFloat("_RimPower");

        LeanTween.value(gameObject, 1f, 0f, 5f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((System.Action<float>)FadingMaterialsAnimationUpdate)
            .setOnComplete(FadingMaterialsAnimationComplete);

    }

    private void FadingMaterialsAnimationUpdate(float value)
    {
        foreach (Material fMat in instance.trashMaterials.fadingMaterials)
        {
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
            fMat.SetFloat("_RimPower", beforeFadingRimPower);
        }
    }

    private void FadingMaterialsAnimationComplete()
    {
        instance.fallingTrashParent.SetActive(false);
        if (!instance.hasMiracleOccurred) {
            hasMiracleOccurred = true;
            EventManager.TriggerEvent(Global.Level4_Events.AFTER_MIRACLE_OCCURED);
        }
    }
}
