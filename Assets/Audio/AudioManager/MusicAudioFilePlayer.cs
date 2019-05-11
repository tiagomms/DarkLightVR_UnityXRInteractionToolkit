using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MusicAudioFilePlayer : AudioFilePlayer {
    private static MusicAudioFilePlayer _instance;
    public static MusicAudioFilePlayer instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MusicAudioFilePlayer>();
            }

            return _instance;
        }
    }
    protected override void Awake()
    {
        _instance = this;
        // call parent awake method
        base.Awake();

        // base.playFirstFileOnAwake = true;
        base.isToRestartAfterPlayingAll = true;
        base.automaticPlayEnabled = true;
    }

}