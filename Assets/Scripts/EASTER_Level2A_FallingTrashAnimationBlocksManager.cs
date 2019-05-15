using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EASTER_Level2A_FallingTrashAnimationBlocksManager : MonoBehaviour {

    private static EASTER_Level2A_FallingTrashAnimationBlocksManager _instance;
    public static EASTER_Level2A_FallingTrashAnimationBlocksManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<EASTER_Level2A_FallingTrashAnimationBlocksManager>();
            }

            return _instance;
        }
    }

	private AudioFile trashSoundFile;
	private FallAndStopChildRigidBodies[] trashBlocks;
	private static int BLOCK_INDEX = 0;
	private void Awake()
	{
		_instance = this;
		trashSoundFile = gameObject.GetComponent<AudioFile>();

		trashBlocks = gameObject.GetComponentsInChildren<FallAndStopChildRigidBodies>();
	}
	private void Start() {
        StartCoroutine(StartEasterAnimation_Coroutine());
	}

    private IEnumerator StartEasterAnimation_Coroutine()
    {
        // make sure names are changed
        yield return new WaitForEndOfFrame();
        
        // set them all for starters
        foreach (FallAndStopChildRigidBodies item in trashBlocks)
        {
            item.gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(2.0f);
        // first half pops 
        for (int i = 0; i < trashBlocks.Length / 2; i++)
        {
            PopTrashBlock();
            yield return new WaitForEndOfFrame();
        }


        yield return new WaitForSecondsRealtime(10.0f);
        EventManager.TriggerEvent(Global.Shared_Events.SHOW_MEDITATION_CIRCLE);
    }

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level2_Events.THROW_BALL, PopTrashBlock);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level2_Events.THROW_BALL, PopTrashBlock);
    }

    public void PopTrashBlock()
    {
		StartCoroutine(TrashBlockAnimation());
    }

    private IEnumerator TrashBlockAnimation(bool playSound = true)
    {
        FallAndStopChildRigidBodies currentBlock = trashBlocks[BLOCK_INDEX];
        currentBlock.gameObject.SetActive(false);

        // int formerBlockIndex = BLOCK_INDEX - 1 < 0 ? trashBlocks.Length - 1 : BLOCK_INDEX - 1; 
        currentBlock.transform.position += Vector3.up * (30f);
        
		BLOCK_INDEX++;
		// trashSoundFile.source.volume += 0.05f;
        // reset
        if (BLOCK_INDEX == trashBlocks.Length)
        {
            BLOCK_INDEX = 0;
        }

		yield return new WaitForSecondsRealtime(1.0f);
		currentBlock.gameObject.SetActive(true);

		currentBlock.FallAndStopChildRigidbodiesAnimation();
		yield return new WaitForSecondsRealtime(1.0f);

		if (playSound && trashSoundFile != null) {
			AudioManager.PlayDelayedAudioFileWRandomPitch(trashSoundFile.audioName);
		}

	}
}
