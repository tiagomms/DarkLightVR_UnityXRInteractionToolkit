using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;

public class TransitionSceneManager : MonoBehaviour {
	private SteamVR_LoadLevel steamVR_LoadLevel;
    private Dictionary<int, SceneFile> sceneFileDict;

    private static TransitionSceneManager _instance;
    public static TransitionSceneManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TransitionSceneManager>();
            }

            return _instance;
        }
    }    

	private void Awake() {
        _instance = this;
        if (instance.steamVR_LoadLevel == null) {
    		instance.steamVR_LoadLevel = GetComponent<SteamVR_LoadLevel>();
        }

        if (instance.sceneFileDict == null) {
            SceneFile[] sceneFiles = GetComponents<SceneFile>();
            instance.sceneFileDict = new Dictionary<int, SceneFile>();
            foreach (SceneFile file in sceneFiles)
            {
                instance.sceneFileDict.Add((int)file.fromLevel, file);
            }
        }
	}
    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.LOAD_SCENE, LoadSceneFile);
        EventManager.StartListening(Global.Shared_Events.CHANGE_SCENE, HandleChangeScene);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Shared_Events.LOAD_SCENE, LoadSceneFile);
        EventManager.StopListening(Global.Shared_Events.CHANGE_SCENE, HandleChangeScene);
    }

    private void HandleChangeScene()
    {
        StartCoroutine(TriggerNextScene());
    }

    private IEnumerator TriggerNextScene()
    {
        yield return new WaitForEndOfFrame();
        instance.steamVR_LoadLevel.Trigger();
    }

    public void LoadSceneFile() {
        SceneFile file = instance.sceneFileDict[SceneManager.GetActiveScene().buildIndex];
        instance.steamVR_LoadLevel.levelName = file.sceneName;
        instance.steamVR_LoadLevel.fadeInTime = file.fadeInTime;
        instance.steamVR_LoadLevel.fadeOutTime = file.fadeOutTime;
        instance.steamVR_LoadLevel.backgroundColor = file.backgroundColor;
        instance.steamVR_LoadLevel.loadingScreenWidthInMeters = 0;
    }
}
