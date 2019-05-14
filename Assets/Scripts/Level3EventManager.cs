using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3EventManager : MonoBehaviour {

	private DontDestroyThisObjectOnChangingScene[] arrayObjectsDontDestroyOnLoad;

	private void Awake()
	{
        arrayObjectsDontDestroyOnLoad = GameObject.FindObjectsOfType<DontDestroyThisObjectOnChangingScene>();

		foreach (DontDestroyThisObjectOnChangingScene item in arrayObjectsDontDestroyOnLoad)
		{
			item.dontDestroyMeOnThisScene = false;
        }
	}
}
