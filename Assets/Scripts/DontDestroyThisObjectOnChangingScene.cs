using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyThisObjectOnChangingScene : MonoBehaviour {
	public bool dontDestroyMeOnThisScene = true; 
	private Global.ThisLevelNbr thisLevel;
	
	private void Awake() {
		thisLevel = Global.currentLevel;
        SetDontDestroyGameObjectInThisScene();
	}

	private void LateUpdate()
	{
		if (thisLevel != Global.currentLevel) {
			thisLevel = Global.currentLevel;
        	SetDontDestroyGameObjectInThisScene();
		}
	}

    private void SetDontDestroyGameObjectInThisScene()
    {
		if (dontDestroyMeOnThisScene) {
			DontDestroyOnLoad(gameObject);
		} else {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        }
    }
}
