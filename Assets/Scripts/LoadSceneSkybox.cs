using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneSkybox : MonoBehaviour {

	public Color solidBackgroundColor = Color.black;
	private Camera cam;
	private void Awake()
	{
		cam = GetComponent<Camera>();
	}

    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.LOAD_SCENE, LoadThisSceneSkybox);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Shared_Events.LOAD_SCENE, LoadThisSceneSkybox);
    }

	private void LoadThisSceneSkybox() {
		int cLevel = (int)Global.currentLevel;
		if (cLevel == 1 || cLevel == 2 || cLevel == 7 || cLevel == 8) { //2A, 2B, 2A_Easter, 2B_Easter
			cam.clearFlags = CameraClearFlags.SolidColor;
			cam.backgroundColor = solidBackgroundColor;
		}
		else {
			cam.clearFlags = CameraClearFlags.Skybox;
		}
	}
}
