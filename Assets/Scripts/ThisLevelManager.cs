using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThisLevelManager : MonoBehaviour
{
    public bool cheatMode = false;

    private void Awake()
    {
        SetCurrentLevel();
    }
    private void SetCurrentLevel()
    {
        // int sceneIndex = Mathf.Min(SceneManager.GetActiveScene().buildIndex, 6);
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        Global.currentLevel = (Global.ThisLevelNbr)sceneIndex;

        SetLevelControls();
        SetLevelScripts();
        SetLevelGravity();
    }

    private void SetLevelGravity()
    {
        if (Global.currentLevel == Global.ThisLevelNbr.L2B) {
            Physics.gravity = new Vector3(0f, -0.18f, 0f);
        } else {
            Physics.gravity = new Vector3(0f, -9.81f, 0f);
        }
    }

    private void SetLevelControls()
    {
        if (!cheatMode) {

            Global.Shared_Controllers.MEDITATION_CIRCLE_READY = false;
            if (Global.currentLevel == Global.ThisLevelNbr.L1)
            {
                Global.Shared_Controllers.TELEPORT = false;
                Global.Shared_Controllers.VOICECOMMAND = false;
                Global.Shared_Controllers.SELECTION_RAY = false;
                Global.Shared_Controllers.HINTMENU = false;
            }
            else if (Global.currentLevel == Global.ThisLevelNbr.L2A)
            {
                Global.Shared_Controllers.TELEPORT = true;
                Global.Shared_Controllers.VOICECOMMAND = false;
                Global.Shared_Controllers.SELECTION_RAY = false;
                Global.Shared_Controllers.HINTMENU = true;
            }
            else if (Global.currentLevel == Global.ThisLevelNbr.L6)
            {
                Global.Shared_Controllers.TELEPORT = true;
                Global.Shared_Controllers.VOICECOMMAND = true;
                Global.Shared_Controllers.SELECTION_RAY = true;
                Global.Shared_Controllers.HINTMENU = true;
            }
            else // 2B, 3, 4, 5
            {
                Global.Shared_Controllers.TELEPORT = false;
                Global.Shared_Controllers.VOICECOMMAND = false;
                Global.Shared_Controllers.SELECTION_RAY = false;
                Global.Shared_Controllers.HINTMENU = true;
            }
        } 
        else {
                Global.Shared_Controllers.TELEPORT = true;
                Global.Shared_Controllers.VOICECOMMAND = true;
                Global.Shared_Controllers.SELECTION_RAY = true;
                Global.Shared_Controllers.HINTMENU = true;
        }
    }

    private void SetLevelScripts()
    {
        // toggle level scripts
        switch (Global.currentLevel)
        {
            case Global.ThisLevelNbr.L1:
                Global.ConsciousLevel = Global.ConsciousnessLevel.FULLY;
                break;
            case Global.ThisLevelNbr.L2A:
                Global.ConsciousLevel = Global.ConsciousnessLevel.NOT;
                break;
            case Global.ThisLevelNbr.L2B:
                Global.ConsciousLevel = Global.ConsciousnessLevel.NOT;
                break;
            case Global.ThisLevelNbr.L3:
                Global.ConsciousLevel = Global.ConsciousnessLevel.BECOMING; // not needed
                break;
            case Global.ThisLevelNbr.L4:
                Global.ConsciousLevel = Global.ConsciousnessLevel.BECOMING;
                break;
            case Global.ThisLevelNbr.L5:
                Global.ConsciousLevel = Global.ConsciousnessLevel.BECOMING; // not needed
                break;
            case Global.ThisLevelNbr.L6:
                Global.ConsciousLevel = Global.ConsciousnessLevel.FULLY;
                break;
            default:
                break;
        }
    }


    // Use this for initialization
    void Start()
    {
        EventManager.TriggerEvent(Global.Shared_Events.SET_TELEPORT);
        EventManager.TriggerEvent(Global.Shared_Events.SET_VOICECOMMAND);
        EventManager.TriggerEvent(Global.Shared_Events.SET_SELECTION_RAY);
        EventManager.TriggerEvent(Global.Shared_Events.LOAD_SCENE);		
    }
}
