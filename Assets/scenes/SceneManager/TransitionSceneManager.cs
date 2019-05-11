using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;
public class TransitionSceneManager : MonoBehaviour {
	private SteamVR_LoadLevel steamVR_LoadLevel;
    private Dictionary<int, SceneFile> sceneFileDict;

    private static TransitionSceneManager transitionSceneManager;
    public static TransitionSceneManager instance
    {
        get
        {
            if (!transitionSceneManager)
            {
                transitionSceneManager = FindObjectOfType(typeof(TransitionSceneManager)) as TransitionSceneManager;

                if (!transitionSceneManager)
                {
                    Debug.LogError("There needs to be one active TransitionSceneManager script on a GameObject in your scene.");
                }
                else
                {
                    transitionSceneManager.Init();
                }
            }

            return transitionSceneManager;
        }
    }    

	private void Init() {
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
        SceneFile file = instance.sceneFileDict[(int)Global.currentLevel];
        instance.steamVR_LoadLevel.levelName = file.sceneName;
        instance.steamVR_LoadLevel.fadeInTime = file.fadeInTime;
        instance.steamVR_LoadLevel.fadeOutTime = file.fadeOutTime;
        instance.steamVR_LoadLevel.backgroundColor = file.backgroundColor;
    }
}
