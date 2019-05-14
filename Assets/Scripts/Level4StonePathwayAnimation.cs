using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Level4StonePathwayAnimation : MonoBehaviour {

	public float stoneAnimDuration = 2.0f;
	private class Stone {
		public string 		stoneName;
		public GameObject 	stoneObj;
		public Renderer 	stoneRend;
		public Stone(GameObject obj)
		{
			stoneName = obj.name;
			stoneObj = obj;
			stoneRend = obj.GetComponent<Renderer>();
		}
	}

	private List<Stone> stoneList;
	private Stone 		currentStone;
	private void Awake()
	{
		// it includes teleport points!
        Renderer[] allChildRenderers = gameObject.GetComponentsInChildren<Renderer>();
		DebugManager.Info("Stones total of renderers: " + allChildRenderers.Length);
		stoneList = new List<Stone>();
		for (int i = 0; i < allChildRenderers.Length; i++)
		{
			GameObject rendObj = allChildRenderers[i].gameObject;
			if (rendObj.transform.parent == gameObject.transform) {
				stoneList.Add(new Stone(rendObj));
				rendObj.SetActive(false);
			}
		}
		stoneList = stoneList.OrderBy(stone => stone.stoneName).ToList();
	}

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level4_Events.STONE_APPEAR, HandleStoneAnimation);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level4_Events.STONE_APPEAR, HandleStoneAnimation);
    }

    private void HandleStoneAnimation()
    {
        if (stoneList.Count != 0) {
			currentStone = stoneList.First();
			stoneList.RemoveAt(0);

			currentStone.stoneObj.SetActive(true);
			Color stoneCurColor = currentStone.stoneRend.material.color;
			stoneCurColor.a = 0f;
        	currentStone.stoneRend.material.color = stoneCurColor;

            LeanTween.alpha(currentStone.stoneObj, 1f, stoneAnimDuration)
                .setEase(LeanTweenType.animationCurve)
				.setOnComplete(HandleStoneAnimation);

		}
    }

}
