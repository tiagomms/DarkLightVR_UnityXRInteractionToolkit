using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NarratorAudioFilePlayer : AudioFilePlayer {
    private static NarratorAudioFilePlayer _instance;
    public static NarratorAudioFilePlayer instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<NarratorAudioFilePlayer>();
            }

            return _instance;
        }
    }

    public bool triggerEventsBetweenClips = false;
    public bool triggerEventOnceOver = false;

    [HideInInspector]
    public int CLIP_INDEX = 0;

    protected override void Awake()
    {
        _instance = this;
        // call parent awake method
        base.Awake();
        base.isToRestartAfterPlayingAll = false;
    }

    protected override bool IsAudioFilePlayerStopped()
    {
        bool result = base.IsAudioFilePlayerStopped();
        if (result) {
            instance.CLIP_INDEX = currentAudioFileIndex;
            if (currentAudioFileIndex < audioFiles.Length && triggerEventsBetweenClips) {
                DebugManager.Info(Global.Shared_Events.NARRATOR_EVENT_BETWEEN_CLIPS);
                EventManager.TriggerEvent(Global.Shared_Events.NARRATOR_EVENT_BETWEEN_CLIPS);
            }
            if (currentAudioFileIndex == audioFiles.Length && triggerEventOnceOver)
            {
                DebugManager.Info(Global.Shared_Events.NARRATOR_ENDED);
                EventManager.TriggerEvent(Global.Shared_Events.NARRATOR_ENDED);
            }
        }
        return result;
    }

    public override bool PlayCurrentAudioFile(float delay = 0f)
    {
        if (base.PlayCurrentAudioFile(delay)) {
            AudioFile s = audioFiles[currentAudioFileIndex];
            float lowerVolumeDuration = s.audioClip.length;
            
            if (MusicAudioFilePlayer.instance != null && s.volume != 0f) {
                MusicAudioFilePlayer.instance.LowerVolumeCurrentAudioFile(lowerVolumeDuration);
            }
            if (MeditationAudioFilePlayer.instance != null && s.volume != 0f) {
                MeditationAudioFilePlayer.instance.LowerVolumeCurrentAudioFile(lowerVolumeDuration);
            }
            
            return true;
        }
        return false;
    }    

}