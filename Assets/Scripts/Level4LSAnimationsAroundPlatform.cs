using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Level4LSAnimationsAroundPlatform : MonoBehaviour {

    private static Level4LSAnimationsAroundPlatform _instance;
    public static Level4LSAnimationsAroundPlatform instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Level4LSAnimationsAroundPlatform>();
            }

            return _instance;
        }
    }
	public Transform[] spawnLocationHeights;
	public float 		radiusAroundPlatform = 11f;
	public float 		movingToDuration = 15f;
	public float 		rotationSpeed = 7f;
	public float 		moveUpSpeed = 0.3f;
	private static int LS_NBR = 0;
	private static float ANGLE_CONSTANT = Mathf.PI * 2f / 9;

	private static int MOVE_TO_SPAWN_LOCATION = 0;

	private Dictionary<string, LightSpiritsController.LightSpirit> lsDict;
	private int lsDictCount;
    private bool areLsReady = false;

	private List<int> spawnLocationLtIds = new List<int>();

    private void Awake()
	{
		_instance = this;
		// the first one is the parent :/
		instance.spawnLocationHeights = (gameObject.GetComponentsInChildren<Transform>()).Skip(1).ToArray();

		// foreach (Transform item in instance.spawnLocationHeights)
		// {
		// 	DebugManager.Info(item.name + " height - " + item.transform.position.y);
		// }
	}

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level4_Events.MOVE_SPAWN_LOCATIONS, TriggerSpawnLocationAnimations);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level4_Events.MOVE_SPAWN_LOCATIONS, TriggerSpawnLocationAnimations);
    }

    private void TriggerSpawnLocationAnimations()
    {
		MOVE_TO_SPAWN_LOCATION++;

		MoveUpSpawnLocationsBelow();

		// trigger narrator talking
		if (MOVE_TO_SPAWN_LOCATION == 1 || MOVE_TO_SPAWN_LOCATION == 5) {
			NarratorAudioFilePlayer.instance.PlayCurrentAudioFile();
		}
    }

    private void MoveUpSpawnLocationsBelow()
    {
		if (MOVE_TO_SPAWN_LOCATION < instance.spawnLocationHeights.Length) {
			// cancel all previous spawn Location LeanTweens
			foreach (int id in spawnLocationLtIds)
			{
				LeanTween.cancel(id);
			}
			// consequently clear them
			spawnLocationLtIds.Clear();

			// move each spawn location upward
			float targetPosY = instance.spawnLocationHeights[MOVE_TO_SPAWN_LOCATION].position.y;
			for (int i = 0; i < MOVE_TO_SPAWN_LOCATION; i++)
			{
				float distance = targetPosY - instance.spawnLocationHeights[i].position.y;
				LTDescr ltdescr = LeanTween.moveY(instance.spawnLocationHeights[i].gameObject, targetPosY, distance / moveUpSpeed)
					.setEase(LeanTweenType.linear);

				// add lean tween ids for next round
				spawnLocationLtIds.Add(ltdescr.id);
			}
		}
    }

    internal IEnumerator StartLsAnimationsAroundPlatform()
    {
        lsDict = LightSpiritsController.instance.LightSpiritsDict;
		lsDictCount = lsDict.Count;
        yield return new WaitForSecondsRealtime(LightSpiritAnimationManager.FADE_TO_DURATION);

        foreach (KeyValuePair<string, LightSpiritsController.LightSpirit> ls in lsDict)
        {
            MoveLSToRespectiveSpawnLocationHeight(ls.Value.lsGameObject);
        }

		StartCoroutine(LsAreReady());
    }

	public void MoveLSToRespectiveSpawnLocationHeight(GameObject ls) {
		string lsName = ls.name;
		Transform futureLSParent = transform;
		
		// top level
		if (lsName == "Angel White" || lsName == "Angel Gold") {
            futureLSParent = spawnLocationHeights[4];
		}
		// below top level
		if (lsName == "Angel Purple" || lsName == "Angel Blue" || lsName == "Angel BabyBlue") {
            futureLSParent = spawnLocationHeights[3];
		}
		// medium level
		if (lsName == "Angel Green") {
            futureLSParent = spawnLocationHeights[2];
		}
		// below medium level
		if (lsName == "Angel Yellow" || lsName == "Angel Orange") {
            futureLSParent = spawnLocationHeights[1];
		}
		// bottom level
		if (lsName == "Angel Red") {
            futureLSParent = spawnLocationHeights[0];
		}

		float spawnAngle = LS_NBR * ANGLE_CONSTANT;
		Vector3 lsNewPos = new Vector3(Mathf.Cos(spawnAngle) * radiusAroundPlatform, futureLSParent.position.y, Mathf.Sin(spawnAngle) * radiusAroundPlatform);
		LeanTween.move(ls, lsNewPos, movingToDuration)
			.setEase(LeanTweenType.easeInQuad)
			.setOnComplete((System.Action<object>)AppearAndSetNewLightSpiritParent)
			.setOnCompleteParam(ls);

		// set parent
		ls.transform.SetParent(futureLSParent);

		// to rotate the next one
		LS_NBR++;
		
	}

    private void AppearAndSetNewLightSpiritParent(object obj)
    {
		GameObject ls = (GameObject)obj;

		StartCoroutine(FadeInLsAroundPlatform(ls));
    }

    private IEnumerator FadeInLsAroundPlatform(GameObject ls)
    {
		// rotate light spirit towards parent
        var lookPos = ls.transform.parent.position - ls.transform.position;
        lookPos.y = 0;

		yield return new WaitForSeconds(.5f);
        ls.transform.rotation = Quaternion.LookRotation(lookPos);
		LeanTween.rotateZ(ls, 90f, 3).setEase(LeanTweenType.easeInSine);
		// ls.transform.Rotate(Vector3.forward * 90f);
		
		// fade back
		lsDict[ls.name].lsAnimationManager.FadeBackToOriginalAlphaLightSpirit();

        // set action to flying
        lsDict[ls.name].lsAnimationManager.SetActionByValue((int)LightSpiritsController.LSAnimations.FLYING);
    }

    private IEnumerator LsAreReady()
    {
		yield return new WaitForSecondsRealtime(movingToDuration + LightSpiritAnimationManager.FADE_TO_DURATION + 0.5f);
		areLsReady = true;
    }

    private void Update() {
        if (areLsReady)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
        }
	}
}
