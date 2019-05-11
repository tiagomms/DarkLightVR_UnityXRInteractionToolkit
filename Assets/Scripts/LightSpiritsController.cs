using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpiritsController : MonoBehaviour {
	private static int IT_NBR = 0; // TODO: delete once it is not necessary
    private static LightSpiritsController _instance;
    public static LightSpiritsController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LightSpiritsController>();
            }

            return _instance;
        }
    }

	private Dictionary<string, LightSpirit> lightSpiritsDict = new Dictionary<string, LightSpirit>();
	private bool isRandomAnimationsOn = true;
	private bool areAllStopped = true;

    public bool AreAllStopped
    {
        get
        {
            return areAllStopped;
        }
    }

    public Dictionary<string, LightSpirit> LightSpiritsDict
    {
        get
        {
            return lightSpiritsDict;
        }
    }

    public enum LSAnimations {
		STILL = -1,
		IDLE = 0,
		TALKING = 1,
		FLYING = 2,
		PREPARE_LIGHT_RAY = 3,
		UNLOAD_LIGHT_RAY = 4
	}
	// Use this for initialization
	public string GetLSAnimationName(int action)
    {
        return Enum.GetName(typeof(LSAnimations), action);
    }

	public class LightSpirit {
		public GameObject lsGameObject;
        public float        lsMatAlphaValue;
		public LightSpiritAnimationManager lsAnimationManager;
		public LightSpiritSelectionRay lsSelectionRay;
	}
	private void Awake()
	{
		_instance = this;
		// generate random seed everytime
		UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

		GameObject[] allLightSpirits = GameObject.FindGameObjectsWithTag("LightSpirit");
		foreach (GameObject obj in allLightSpirits)
		{
			LightSpirit newLightSpirit = new LightSpirit();
			newLightSpirit.lsGameObject = obj;
            // newLightSpirit.lsMatAlphaValue = obj.GetComponentInChildren<Renderer>().material.color.a;
			newLightSpirit.lsAnimationManager = obj.GetComponent<LightSpiritAnimationManager>();
			newLightSpirit.lsSelectionRay = obj.GetComponentInChildren<LightSpiritSelectionRay>();
			instance.lightSpiritsDict.Add(obj.name, newLightSpirit);

			DebugManager.Info("LightSpirit: '"+ obj.name + "'" + ", alpha value: " + newLightSpirit.lsAnimationManager.LsOriginalMatAlphaValue);
		}
	}
    private void OnEnable()
    {
        EventManager.StartListening(Global.Level4_Events.TRASH_FALLING_START, HandleTrashFallingStart);
        EventManager.StartListening(Global.Level4_Events.LIGHT_SPIRITS_RAISE_ARM, RaiseLightSpiritsArms);
        // EventManager.StartListening(Global.Level4_Events.PLAYER_HIT_TRASH, HandlePlayerHitTrashParent);
        EventManager.StartListening(Global.Level4_Events.AFTER_MIRACLE_OCCURED, HandleTrashFallingDisappearance);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level4_Events.TRASH_FALLING_START, HandleTrashFallingStart);
        EventManager.StopListening(Global.Level4_Events.LIGHT_SPIRITS_RAISE_ARM, RaiseLightSpiritsArms);
        // EventManager.StopListening(Global.Level4_Events.PLAYER_HIT_TRASH, HandlePlayerHitTrashParent);
        EventManager.StopListening(Global.Level4_Events.AFTER_MIRACLE_OCCURED, HandleTrashFallingDisappearance);
    }
    private void Start()
    {
        SetActionToAllLightSpirits((int)LSAnimations.IDLE);

        // on all other levels, the white angel will do the talking
        if (Global.currentLevel != Global.ThisLevelNbr.L6) {
            SetActionToLightSpirit("Angel White", (int)LSAnimations.TALKING);
            SetLockedAnimToLightSpirit("Angel White", true);
            StartCoroutine(TriggerRandomLightSpiritAnimations());
        } else {
            SetActionToAllLightSpirits((int)LSAnimations.FLYING);
        }

    }	

	public void SetActionToAllLightSpirits(int actionValue, bool forceAnimationToAll = false) {
		foreach(KeyValuePair<string, LightSpirit> item in instance.LightSpiritsDict)
		{
            item.Value.lsAnimationManager.SetActionByValue(actionValue, forceAnimationToAll);
		}
	}
	
	public void SetActionToLightSpirit(string lsName, int actionValue, bool forceAnimation = false) {
		LightSpirit currentLS = instance.LightSpiritsDict[lsName];
		if (currentLS != null) {
			currentLS.lsAnimationManager.SetActionByValue(actionValue, forceAnimation);
		}
	}

	public void SetLockedAnimToLightSpirit(string lsName, bool lockedAnim = false) {
		LightSpirit currentLS = instance.LightSpiritsDict[lsName];
		if (currentLS != null) {
			currentLS.lsAnimationManager.IsLockedAnimOn = lockedAnim;
		}
	}

    private void RaiseLightSpiritsArms()
    {
        SetActionToAllLightSpirits((int)LSAnimations.PREPARE_LIGHT_RAY, true);
    }

    private void HandleTrashFallingStart()
    {
		instance.isRandomAnimationsOn = false;
    }

    private void HandleTrashFallingDisappearance()
    {
        // stop rays
        DisableAllLSRays();

        // lower arms
        SetActionToAllLightSpirits((int)LSAnimations.UNLOAD_LIGHT_RAY);

        // fading light spirits
        FadeOutAllLightSpirits();

        // pop up in next location
    }

    private void FadeOutAllLightSpirits()
    {
        foreach (KeyValuePair<string, LightSpirit> item in instance.LightSpiritsDict)
        {
            item.Value.lsAnimationManager.FadeToAlphaLightSpirit();
        }
    }



    private void DisableAllLSRays()
    {
        foreach (KeyValuePair<string, LightSpirit> item in instance.LightSpiritsDict)
        {
            item.Value.lsSelectionRay.DisableSelectionRay();
        }
    }

    private IEnumerator TriggerRandomLightSpiritAnimations()
    {
		if (instance.isRandomAnimationsOn) {
			foreach (LightSpirit ls in RandomValues(instance.LightSpiritsDict).Take(instance.LightSpiritsDict.Count))
			{
				float randProb = UnityEngine.Random.value;
				int action = 0;
				if (randProb < 0.01f) {
					action = (int)LSAnimations.STILL;
				} else if (randProb < 0.85f) {
					action = (int)LSAnimations.IDLE;
				} else {
					action = (int)LSAnimations.FLYING;
				}

				SetActionToLightSpirit(ls.lsGameObject.name, action);
			}
			instance.areAllStopped = false;
			yield return new WaitForSeconds(5f);
			yield return TriggerRandomLightSpiritAnimations();
		} else {
			yield return ForceStopAllLightSpirits();
		}
    }

    private IEnumerator ForceStopAllLightSpirits() {
		SetActionToAllLightSpirits((int)LSAnimations.STILL, true);
		yield return new WaitForSeconds(1f);
		
		instance.areAllStopped = AreAllLightSpiritsInState("Still");
		// DebugManager.Info("IT Nbr: " + IT_NBR + " - Are all Still: " + instance.areAllStopped);
		// IT_NBR++;
		
		if (!instance.areAllStopped) {
			yield return ForceStopAllLightSpirits();
		} else {
			EventManager.TriggerEvent(Global.Level4_Events.LIGHT_SPIRITS_STOP);
		}
	}

    private bool AreAllLightSpiritsInState(string stateName)
    {
		bool result = true;
        foreach (KeyValuePair<string, LightSpirit> item in instance.LightSpiritsDict)
        {
            result &= item.Value.lsAnimationManager.IsCurrentAnimState(stateName);
        }
		return result;
    }

    public IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        System.Random rand = new System.Random();
        List<TValue> values = Enumerable.ToList(dict.Values);
        int size = dict.Count;
        while (true)
        {
            yield return values[rand.Next(size)];
        }
    }
}
