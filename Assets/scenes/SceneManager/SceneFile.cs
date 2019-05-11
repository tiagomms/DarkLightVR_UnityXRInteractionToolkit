using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SceneFile : MonoBehaviour
{
	public Global.ThisLevelNbr fromLevel;
    public string sceneName;
	public Color backgroundColor = Color.black;

	public float fadeInTime = 0.5f;
	public float fadeOutTime = 0.5f;

}

