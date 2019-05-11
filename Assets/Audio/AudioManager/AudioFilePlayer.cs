using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioFilePlayer : MonoBehaviour {

	public AudioFile[]	audioFiles;
	public float[] 		waitingPeriodBetweenFiles;

	public float 		stoppedRepeatRate = 1f;
	public float 		stoppedFirstInvokeDelay = 60f;
	public bool 		playFirstFileOnAwake = false;
	public bool 		automaticPlayEnabled = false;
	public bool 		isToRestartAfterPlayingAll = false;

	protected int 		currentAudioFileIndex = 0;
	protected bool  		playingCurrentFile = false;
    protected float       audioFilePausedAt = 0f;

	protected virtual void Awake()
	{
		audioFiles = gameObject.GetComponents<AudioFile>();
		// sorted by name to make sure no bugs
		audioFiles = audioFiles.OrderBy(s => s.audioName).ToArray();

		int waitingPeriodsLength = waitingPeriodBetweenFiles.Length;
		if (waitingPeriodsLength != audioFiles.Length) {

			waitingPeriodBetweenFiles = new float[audioFiles.Length];
			if (waitingPeriodsLength != 0) {
				DebugManager.Error("AudioFilePlayer: WaitingPeriods not matching audioFile array");
			}
		}
	}
	private void Start()
	{
		if (playFirstFileOnAwake) {
			PlayCurrentAudioFile(waitingPeriodBetweenFiles[currentAudioFileIndex]);
		}
	}

    #region AUDIO_FILE_PLAYER_METHODS

    protected virtual bool IsAudioFilePlayerStopped()
    {
        if (playingCurrentFile && !audioFiles[currentAudioFileIndex].source.isPlaying)
        {
            currentAudioFileIndex++;
            playingCurrentFile = false;
            CancelInvoke("IsAudioFilePlayerStopped");

            if (currentAudioFileIndex == audioFiles.Length && isToRestartAfterPlayingAll)
            {
                currentAudioFileIndex = 0;
            }
            if (automaticPlayEnabled && currentAudioFileIndex < audioFiles.Length)
            {
                PlayCurrentAudioFile(waitingPeriodBetweenFiles[currentAudioFileIndex]);
            }
            return true;
        }
        return false;
    }
    public void SkipCurrentAudioFile()
    {
        if (audioFiles != null)
        {
            StopCurrentAudioFile();
            currentAudioFileIndex++;
            if (isToRestartAfterPlayingAll && currentAudioFileIndex == audioFiles.Length)
            {
                currentAudioFileIndex = 0;
            }
        }
    }

    public void AutomaticPlay()
    {
        float delay = 0;
        for (int i = 0; i < audioFiles.Length; i++)
        {
            delay += waitingPeriodBetweenFiles[i];
            AudioManager.PlayDelayedAudioFile(audioFiles[i].audioName, delay);
            delay += audioFiles[i].audioClip.length;
        }
        InvokeRepeating("IsAudioFilePlayerStopped", delay, stoppedRepeatRate);
    }
    
    public void PlayByIndex(int index, float delay = 0)
    {
        if (audioFiles != null && index < audioFiles.Length)
        {
            AudioManager.PlayDelayedAudioFile(audioFiles[index].audioName, delay);
            InvokeRepeating("IsAudioFilePlayerStopped", audioFiles[index].audioClip.length + delay, stoppedRepeatRate);
        }
    }

    #endregion


    #region CALL_AUDIOMANAGER_METHODS

    public virtual bool PlayCurrentAudioFile(float delay = 0f)
    {
        if (audioFiles != null && currentAudioFileIndex < audioFiles.Length) {
			AudioManager.PlayDelayedAudioFile(audioFiles[currentAudioFileIndex].audioName, delay);
			playingCurrentFile = true;

            InvokeRepeating("IsAudioFilePlayerStopped", audioFiles[currentAudioFileIndex].audioClip.length + delay, stoppedRepeatRate);
            return true;
        }
        return false;
    }

    public void StopCurrentAudioFile()
    {
        if (audioFiles != null && currentAudioFileIndex < audioFiles.Length) {

			AudioManager.StopAudioFile(audioFiles[currentAudioFileIndex].audioName);
			playingCurrentFile = false;

            CancelInvoke("IsAudioFilePlayerStopped");
        }
    }

    public void PauseCurrentAudioFile()
    {
        if (audioFiles != null && currentAudioFileIndex < audioFiles.Length) {
			AudioManager.PauseAudioFile(audioFiles[currentAudioFileIndex].audioName);
            CancelInvoke("IsAudioFilePlayerStopped");
            
            // saved time it paused at
            audioFilePausedAt = audioFiles[currentAudioFileIndex].source.time;
		}
    }

    public void UnPauseCurrentAudioFile()
    {
        if (audioFiles != null && currentAudioFileIndex < audioFiles.Length) {
			AudioManager.UnPauseAudioFile(audioFiles[currentAudioFileIndex].audioName);

            InvokeRepeating("IsAudioFilePlayerStopped", audioFiles[currentAudioFileIndex].audioClip.length - audioFilePausedAt, stoppedRepeatRate);
        }
    }

    public void LowerVolumeCurrentAudioFile(float duration)
    {
		if (audioFiles != null && currentAudioFileIndex < audioFiles.Length) {
			AudioManager.LowerVolume(audioFiles[currentAudioFileIndex].audioName, duration);
		}
    }

    public void FadeOutCurrentAudioFile(float duration)
    {
		if (audioFiles != null && currentAudioFileIndex < audioFiles.Length) {
			AudioManager.FadeOut(audioFiles[currentAudioFileIndex].audioName, duration);
			playingCurrentFile = false;

            // reset invoke for after duration
            CancelInvoke("IsAudioFilePlayerStopped");
            InvokeRepeating("IsAudioFilePlayerStopped", duration, stoppedRepeatRate);

        }
    }

    public void FadeInCurrentAudioFile(float targetVolume, float duration)
    {
		if (audioFiles != null && currentAudioFileIndex < audioFiles.Length) {
			AudioManager.FadeIn(audioFiles[currentAudioFileIndex].audioName, targetVolume, duration);
			playingCurrentFile = true;

            InvokeRepeating("IsAudioFilePlayerStopped", audioFiles[currentAudioFileIndex].audioClip.length, stoppedRepeatRate);
        }
    }
	#endregion


}
