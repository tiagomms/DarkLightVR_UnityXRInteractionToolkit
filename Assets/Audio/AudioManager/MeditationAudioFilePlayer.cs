using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeditationAudioFilePlayer : AudioFilePlayer
{
    private static MeditationAudioFilePlayer _instance;
    public static MeditationAudioFilePlayer instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MeditationAudioFilePlayer>();
            }

            return _instance;
        }
    }
    protected override void Awake()
    {
        _instance = this;
        // call parent awake method
        base.Awake();

        // always loop meditation songs
        base.isToRestartAfterPlayingAll = true;
        base.automaticPlayEnabled = true;
    }

}