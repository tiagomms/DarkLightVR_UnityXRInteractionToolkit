using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLightBeam : MonoBehaviour {
	public GameObject[] lightBeamObjects;

    private static ToggleLightBeam _instance;
    public static ToggleLightBeam instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ToggleLightBeam>();
            }

            return _instance;
        }
    }

	private void Awake()
	{
		_instance = this;
	}

    private void Start() {
        EnableThisLevelLightBeam();
	}
		
	public void EnableThisLevelLightBeam() {
		int consciousLevel = (int)Global.ConsciousLevel;
		for (int i = 0; i < lightBeamObjects.Length; i++)
		{
			if (i == consciousLevel) {
				lightBeamObjects[i].SetActive(true);
			} else {
				lightBeamObjects[i].SetActive(false);
			}
		}
	}
}
