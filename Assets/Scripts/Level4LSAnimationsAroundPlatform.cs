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
	public float 		radiusAroundPlatform = 15f;
	public float 		movingToDuration = 15f;
	private static int LS_NBR = 0;
	private static float ANGLE_CONSTANT = Mathf.PI * 2f / 9;

	private void Awake()
	{
		_instance = this;
		instance.spawnLocationHeights = gameObject.GetComponentsInChildren<Transform>();
		instance.spawnLocationHeights = instance.spawnLocationHeights.OrderBy(t => t.name).ToArray();
	}

	public void MoveLSToRespectiveSpawnLocationHeight(object obj) {
		GameObject ls = (GameObject)obj;
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
			.setEase(LeanTweenType.easeInOutCirc)
			.setOnComplete((System.Action<object>)AppearAndSetNewLightSpiritParent)
			.setOnCompleteParam(ls);

		// set parent
		ls.transform.SetParent(futureLSParent);

		// increase for next spawn
		LS_NBR++;
	}

    private void AppearAndSetNewLightSpiritParent(object obj)
    {
		GameObject ls = (GameObject)obj;
		// fade back
		LightSpiritsController.instance.LightSpiritsDict[ls.name].lsAnimationManager.FadeBackToOriginalAlphaLightSpirit();
    }
}
