using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    #region VARIABLES

    private static AudioManager _instance;
    public static AudioManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AudioManager>();
            }

            return _instance;
        }
    }

	public AudioFile[] audioFiles;
    public Dictionary<string, AudioFile> audioFilesDict;

    // private List<bool> timerIsSet;
    private List<float> timesToResetLoweredVols = new List<float>();

    private List<string> tmpLoweredNames = new List<string>();
    private List<float> originalVols = new List<float>();

    private bool isLowered = false;
    private bool fadeOut = false;
    private bool fadeIn = false;

    private string fadeInUsedString;
    private string fadeOutUsedString;

    #endregion


    // Use this for initialization
    void Awake()
    {
        _instance = this;

		audioFilesDict = new Dictionary<string, AudioFile>();

        // tagged
        GameObject[] taggedAudioFilesObjs = GameObject.FindGameObjectsWithTag("AudioFile");
        foreach (GameObject obj in taggedAudioFilesObjs)
        {
            AudioFile[] taggedAudioFiles = obj.GetComponents<AudioFile>();
            foreach (AudioFile file in taggedAudioFiles)
            {
                AddNewAudioFileToDictionary(file);
            }
        }

        // not tagged
        foreach (AudioFile s in audioFiles)
        {
            AddNewAudioFileToDictionary(s);
        }
    }

    public void AddNewAudioFileToDictionary(AudioFile s) {
        // insert audioName if null
        if (s.audioName == null || s.audioName.Length == 0) {
            s.audioName = s.audioClip.name;
        }

        if (!s.hasBeenCreatedBefore) {
            s.source = s.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.priority = s.priority;
            s.source.volume = s.volume;
            s.source.spatialBlend = s.spatialBlend;
            s.source.rolloffMode = s.volumeRolloff;
            s.source.minDistance = s.minDistance;
            s.source.maxDistance = s.maxDistance;
            s.source.playOnAwake = s.playOnAwake;
            s.source.loop = s.isLooping;
        }
        if (s.playOnAwake)
        {
            s.source.Play();
        }

        s.hasBeenCreatedBefore = true;
        audioFilesDict.Add(s.audioName, s);
    } 
    #region METHODS

    public static void PlayAudioFile(string name)
    {
        try {
            AudioFile s = AudioManager.instance.audioFilesDict[name];
            s.source.Play();
        } catch (KeyNotFoundException exception) {
            DebugManager.Error("Sound name " + name + " - not found!");
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }
    public static void PlayDelayedAudioFile(string name, float delay)
    {
        try {
            AudioFile s = AudioManager.instance.audioFilesDict[name];
            s.source.PlayDelayed(delay);
        } catch (KeyNotFoundException exception) {
            Debug.LogError("AUDIOMANAGER: Sound name " + name + " - not found!");
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }
    public static void PlayDelayedAudioFileWRandomPitch(string name, float delay = 0f)
    {
        try {
            AudioFile s = AudioManager.instance.audioFilesDict[name];
            s.source.pitch = UnityEngine.Random.Range(s.lowPitchRange, s.highPitchRange);
            s.source.PlayDelayed(delay);
        } catch (KeyNotFoundException exception) {
            DebugManager.Error("Sound name " + name + " - not found!");
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }


    public static void StopAudioFile(String name)
    {
        try {
            AudioFile s = AudioManager.instance.audioFilesDict[name];
            s.source.Stop();
        } catch (KeyNotFoundException exception) {
            DebugManager.Error("Sound name " + name + " - not found!");
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    public static void PauseAudioFile(String name)
    {
        try {
            AudioFile s = AudioManager.instance.audioFilesDict[name];
            s.source.Pause();
        } catch (KeyNotFoundException exception) {
            DebugManager.Error("Sound name " + name + " - not found!");
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    public static void UnPauseAudioFile(String name)
    {
        try {
            AudioFile s = AudioManager.instance.audioFilesDict[name];
            s.source.UnPause();
        } catch (KeyNotFoundException exception) {
            DebugManager.Error("Sound name " + name + " - not found!");
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    public static void LowerVolume(String name, float _duration)
    {
        try {
            AudioFile s = AudioManager.instance.audioFilesDict[name];
            instance.tmpLoweredNames.Add(name);
            instance.originalVols.Add(s.volume);
            instance.timesToResetLoweredVols.Add(Time.time + _duration);
            
            // instance.timerIsSet = true;
            s.source.volume = s.source.volume / 3;

            instance.isLowered = true;
        } catch (KeyNotFoundException exception) {
            DebugManager.Error("Sound name " + name + " - not found!");
        } catch (Exception e) {
            Debug.LogError(e);
        }
        // if (instance.isLowered == false)
        // {
        // }
    }

    public static void FadeOut(String name, float duration)
    {
        instance.StartCoroutine(instance.IFadeOut(name, duration));
    }

    public static void FadeIn(String name, float targetVolume, float duration)
    {
        instance.StartCoroutine(instance.IFadeIn(name, targetVolume, duration));
    }



    //not for use
    private IEnumerator IFadeOut(String name, float duration)
    {
        AudioFile s = null;
        try {
            s = AudioManager.instance.audioFilesDict[name];
        } catch (KeyNotFoundException exception) {
            DebugManager.Error("Sound name " + name + " - not found!");
        } catch (Exception e) {
            Debug.LogError(e);
        }
        if (s == null)
        {
            yield return null;
        }
        else
        {
            if (fadeOut == false)
            {
                fadeOut = true;
                float startVol = s.source.volume;
                fadeOutUsedString = name;
                while (s.source.volume > 0)
                {
                    s.source.volume -= startVol * Time.deltaTime / duration;
                    yield return null;
                }

                s.source.Stop();
                yield return new WaitForSeconds(duration);
                fadeOut = false;
            }

            else
            {
                DebugManager.Info("Could not handle two fade outs at once : " + name + " , " + fadeOutUsedString + "! Stopped the music " + name);
                StopAudioFile(name);
            }
        }
    }

    public IEnumerator IFadeIn(string name, float targetVolume, float duration)
    {
        AudioFile s = null;
        try {
            s = AudioManager.instance.audioFilesDict[name];
        } catch (KeyNotFoundException exception) {
            DebugManager.Error("Sound name " + name + " - not found!");
        } catch (Exception e) {
            Debug.LogError(e);
        }
        if (s == null)
        {
            yield return null;
        }
        else
        {
            if (fadeIn == false)
            {
                fadeIn = true;
                instance.fadeInUsedString = name;
                s.source.volume = 0f;
                s.source.Play();
                while (s.source.volume < targetVolume)
                {
                    s.source.volume += Time.deltaTime / duration;
                    yield return null;
                }

                yield return new WaitForSeconds(duration);
                fadeIn = false;
            }
            else
            {
                DebugManager.Info("Could not handle two fade ins at once: " + name + " , " + fadeInUsedString + "! Played the music " + name);
                StopAudioFile(fadeInUsedString);
                PlayAudioFile(name);
            }
        }
    }

    public static void ResetVol(string name = null)
    {
        if (instance.tmpLoweredNames.Count != 0) {
            int fileIndex = (name != null ? instance.tmpLoweredNames.IndexOf(name) : 0);

            // value saved
            AudioFile s = AudioManager.instance.audioFilesDict[instance.tmpLoweredNames[fileIndex]];
            s.source.volume = instance.originalVols[fileIndex];

            instance.tmpLoweredNames.RemoveAt(fileIndex);
            instance.originalVols.RemoveAt(fileIndex);
            instance.timesToResetLoweredVols.RemoveAt(fileIndex);

            if (instance.tmpLoweredNames.Count == 0) {
                instance.isLowered = false;
            }
        }
    }

    public static void ResetAllVols() {
        while (instance.tmpLoweredNames.Count != 0) {
            ResetVol();
        }
    }


    private void Update()
    {
        if (instance.isLowered) {
            int i = 0;
            while (i < tmpLoweredNames.Count) {
                if (Time.time >= timesToResetLoweredVols[i])
                {
                    ResetVol(tmpLoweredNames[i]);
                } 
                else {
                    i++;
                }
            }
        }
    }

    #endregion
}