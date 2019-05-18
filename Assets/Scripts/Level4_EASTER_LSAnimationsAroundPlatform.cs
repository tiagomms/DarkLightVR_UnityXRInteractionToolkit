using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Level4_EASTER_LSAnimationsAroundPlatform : MonoBehaviour {

    private static Level4_EASTER_LSAnimationsAroundPlatform _instance;
    public static Level4_EASTER_LSAnimationsAroundPlatform instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Level4_EASTER_LSAnimationsAroundPlatform>();
            }

            return _instance;
        }
    }
	public Transform[] spawnLocationHeights;
    public Transform    fallingTrashParent;
	public float 		radiusAroundPlatform = 11f;
	public float 		movingToDuration = 15f;
	public float 		rotationSpeed = 7f;
	public float 		moveUpSpeed = 0.3f;
	private static int LS_NBR = 0;
	private static float ANGLE_CONSTANT = Mathf.PI * 2f / 9;

	private int MOVE_TO_SPAWN_LOCATION = 0;

	private Dictionary<string, LightSpiritsController.LightSpirit> lsDict;
	private int lsDictCount;
    private bool areLsReady = false;

	private List<int> spawnLocationLtIds = new List<int>();

    private void Awake()
	{
		_instance = this;
	}

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level4_Events.MOVE_SPAWN_LOCATIONS, TriggerSpawnLocationAnimations);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level4_Events.MOVE_SPAWN_LOCATIONS, TriggerSpawnLocationAnimations);
    }    

    public void PlaceLightSpiritsAroundPlatform()
    {
        lsDict = LightSpiritsController.instance.LightSpiritsDict;
		lsDictCount = lsDict.Count;

        foreach (KeyValuePair<string, LightSpiritsController.LightSpirit> ls in lsDict)
        {
            MoveLSToRespectiveSpawnLocationHeight(ls.Value.lsGameObject);
        }
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


		// set position around radius
		float spawnAngle = LS_NBR * ANGLE_CONSTANT;
		Vector3 lsNewPos = new Vector3(Mathf.Cos(spawnAngle) * radiusAroundPlatform, futureLSParent.position.y, Mathf.Sin(spawnAngle) * radiusAroundPlatform);

		ls.transform.position = lsNewPos;

        // rotate light spirit towards the trash center
        var fallingTrashlookPos = fallingTrashParent.position - ls.transform.position;
        fallingTrashlookPos.y = 0;
        ls.transform.rotation = Quaternion.LookRotation(fallingTrashlookPos);

		// fade back
		lsDict[ls.name].lsAnimationManager.FadeBackToOriginalAlphaLightSpirit();

		// set parent
		ls.transform.SetParent(futureLSParent);

		// to rotate the next one
		LS_NBR++;
		
	}

    private void TriggerSpawnLocationAnimations()
    {
        instance.MOVE_TO_SPAWN_LOCATION++;

        MoveUpSpawnLocationsBelow();
    }

    private void MoveUpSpawnLocationsBelow()
    {
        if (instance.MOVE_TO_SPAWN_LOCATION < instance.spawnLocationHeights.Length)
        {
            // cancel all previous spawn Location LeanTweens
            foreach (int id in spawnLocationLtIds)
            {
                LeanTween.cancel(id);
            }
            // consequently clear them
            spawnLocationLtIds.Clear();

            // move each spawn location upward
            float targetPosY = instance.spawnLocationHeights[instance.MOVE_TO_SPAWN_LOCATION].position.y;
            for (int i = 0; i < instance.MOVE_TO_SPAWN_LOCATION; i++)
            {
                float distance = targetPosY - instance.spawnLocationHeights[i].position.y;
                LTDescr ltdescr = LeanTween.moveY(instance.spawnLocationHeights[i].gameObject, targetPosY, distance / moveUpSpeed)
                    .setEase(LeanTweenType.linear);

                // add lean tween ids for next round
                spawnLocationLtIds.Add(ltdescr.id);
            }
        }
    }
    internal void RotateLightSpiritsAroundCenter()
    {
        foreach (KeyValuePair<string, LightSpiritsController.LightSpirit> item in lsDict)
        {
            GameObject ls = item.Value.lsGameObject;
            // rotate light spirit towards the center
            var lookPos = ls.transform.parent.position - ls.transform.position;
            lookPos.y = 0;
            Vector3 finalRotation = Quaternion.LookRotation(lookPos).eulerAngles + Vector3.forward * 90f;
            
            // ls.transform.rotation = Quaternion.LookRotation(lookPos);

            LeanTween.rotate(ls, finalRotation, 3f)
                .setEase(LeanTweenType.easeInSine)
				.setOnComplete(SetAreLsReady);
			// LeanTween.rotateZ(ls, 90f, 3).setEase(LeanTweenType.easeInSine)
			// 	.setOnComplete(SetAreLsReady);
        }

    }

    private void SetAreLsReady()
    {
		areLsReady = true;
    }

    private void Update() {
        if (areLsReady)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
        }
	}

}
