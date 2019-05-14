#if UNITY_EDITOR 
using UnityEditor;
#endif
using UnityEngine;

[System.Serializable]
public class AudioFile : MonoBehaviour
{

    public string audioName;
    public AudioClip audioClip;


    [Range(0f,1f)]
    public float volume;
    [Range(0f,1f)]
    public float spatialBlend = 0f;
    public AudioRolloffMode volumeRolloff = AudioRolloffMode.Linear;
    public float minDistance = 3f;
    public float maxDistance = 50f;

    [Range(0.25f, 1f)]
    public float lowPitchRange = .75f;
    [Range(1f, 2f)]
    public float highPitchRange = 1.5f;

	[Range(0, 255)]
	public int priority = 128;

    [HideInInspector]
    public AudioSource source;

    public bool isLooping = false;
    public bool playOnAwake = false;

}

