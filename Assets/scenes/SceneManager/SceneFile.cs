#if UNITY_EDITOR 
using UnityEditor;
#endif

using UnityEngine;

[System.Serializable]
public class SceneFile : MonoBehaviour
{
	public Global.ThisLevelNbr fromLevel;
    public string sceneName;
	public Color backgroundColor = Color.black;

	public float fadeInTime = 2f;
	public float fadeOutTime = 2f;

}

