using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTrashAnimationBlocksManager : MonoBehaviour {

    private static FallingTrashAnimationBlocksManager _instance;
    public static FallingTrashAnimationBlocksManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<FallingTrashAnimationBlocksManager>();
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
        foreach (FallAndStopChildRigidBodies item in trashBlocks)
        {
            item.gameObject.SetActive(false);
        }
	}

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level2_Events.THROW_BALL, PopTrashBlock);
        EventManager.StartListening(Global.Level2_Events.TRASH_PILLING_UP, PileUpMoreTrashAnimation);
        EventManager.StartListening(Global.Shared_Events.NARRATOR_EVENT_BETWEEN_CLIPS, TriggerTrashPileUp);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level2_Events.THROW_BALL, PopTrashBlock);
        EventManager.StopListening(Global.Level2_Events.TRASH_PILLING_UP, PileUpMoreTrashAnimation);
        EventManager.StopListening(Global.Shared_Events.NARRATOR_EVENT_BETWEEN_CLIPS, TriggerTrashPileUp);
    }

    private void TriggerTrashPileUp()
    {
        StartCoroutine(WaitAndTriggerTrashPileUp());
    }

    private IEnumerator WaitAndTriggerTrashPileUp()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        EventManager.TriggerEvent(Global.Level2_Events.TRASH_PILLING_UP);
    }

    private void PopTrashBlock()
    {
		StartCoroutine(TrashBlockAnimation());
    }
    private void PileUpMoreTrashAnimation()
    {
        if (trashSoundFile != null) {
    		trashSoundFile.source.volume = 0.5f;
        }
        StartCoroutine(MoreTrashPilesAtOnceAnimation());
    }

    private IEnumerator MoreTrashPilesAtOnceAnimation()
    {
		PopTrashBlock();
        yield return new WaitForSecondsRealtime(1.5f);
        StartCoroutine(TrashBlockAnimation(false));
        yield return new WaitForSecondsRealtime(1.5f);
		PopTrashBlock();
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
		yield return new WaitForSecondsRealtime(0.5f);

		if (playSound && trashSoundFile != null) {
			AudioManager.PlayDelayedAudioFileWRandomPitch(trashSoundFile.audioName);
		}

	}
}
