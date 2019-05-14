using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Windows.Speech;
using System.Linq;
using System;

public class VoiceCommandManager : MonoBehaviour
{

    public SteamVR_Action_Boolean voiceInputAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("VoiceInput");
    public TrashObjectsHandling trashObjectsHandling;
    public ConfidenceLevel wordConfidenceLevel = ConfidenceLevel.Low;

    // TODO: with events
    public AudioClip turnOnClip;
    public AudioClip turnOffClip;
    public AudioClip successClip;

    // private Queue<AudioClip> audioClipQueue = new Queue<AudioClip>();

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    private bool isInMeditationCircle = false;
    private bool isActivated = false;
    private bool isGoAwayHandled = false;
    private bool isIAmReadyHandled = false;
    private AudioSource voiceInputAudioSource;

    private void Awake()
    {
        voiceInputAudioSource = gameObject.GetComponent<AudioSource>();
    }
    private void Start()
    {
        // go away iterations
        keywords.Add("go", () => { HandleGoAwayVoiceInput(); });
        keywords.Add("go away", () => { HandleGoAwayVoiceInput(); });
        keywords.Add("goaway", () => { HandleGoAwayVoiceInput(); });
        keywords.Add("goway", () => { HandleGoAwayVoiceInput(); });
        keywords.Add("go way", () => { HandleGoAwayVoiceInput(); });
        keywords.Add("way", () => { HandleGoAwayVoiceInput(); });
        keywords.Add("away", () => { HandleGoAwayVoiceInput(); });

        // i am ready iterations
        keywords.Add("am", () => { HandleImReadyVoiceInput(); });
        keywords.Add("ham", () => { HandleImReadyVoiceInput(); });
        keywords.Add("yam", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i am", () => { HandleImReadyVoiceInput(); });
        keywords.Add("eyam ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("eyeam ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("iyam redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("iyam ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i yam ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("yam redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("yam ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("teady", () => { HandleImReadyVoiceInput(); });
        keywords.Add("teddy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("seaby", () => { HandleImReadyVoiceInput(); });
        keywords.Add("steady", () => { HandleImReadyVoiceInput(); });
        keywords.Add("seady", () => { HandleImReadyVoiceInput(); });
        keywords.Add("seby", () => { HandleImReadyVoiceInput(); });
        keywords.Add("reby", () => { HandleImReadyVoiceInput(); });
        keywords.Add("reaby", () => { HandleImReadyVoiceInput(); });
        keywords.Add("raby", () => { HandleImReadyVoiceInput(); });
        keywords.Add("rady", () => { HandleImReadyVoiceInput(); });
        keywords.Add("redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("ready", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I am ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i am ready", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I m ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i m ready", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("Im ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("im ready", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I ham ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i ham ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("ham ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("m ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("em ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("am ready", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I'm raedy", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("Im raedy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i'm raedy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("im raedy", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I'm redy", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("Im redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i'm redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("im redy", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I am redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i am redy", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I m redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i m redy", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("Im redy", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I ham redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i ham redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("ham redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("m redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("em redy", () => { HandleImReadyVoiceInput(); });
        keywords.Add("am redy", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I'm redi", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("Im redi", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i'm redi", () => { HandleImReadyVoiceInput(); });
        keywords.Add("im redi", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I am redi", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i am redi", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I m redi", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i m redi", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("Im redi", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I ham redi", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i ham redi", () => { HandleImReadyVoiceInput(); });
        keywords.Add("ham redi", () => { HandleImReadyVoiceInput(); });
        keywords.Add("m redi", () => { HandleImReadyVoiceInput(); });
        keywords.Add("em redi", () => { HandleImReadyVoiceInput(); });
        keywords.Add("am redi", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I'm rede", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("Im rede", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i'm rede", () => { HandleImReadyVoiceInput(); });
        keywords.Add("im rede", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I am rede", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i am rede", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I m rede", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i m rede", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("Im rede", () => { HandleImReadyVoiceInput(); });
        // keywords.Add("I ham rede", () => { HandleImReadyVoiceInput(); });
        keywords.Add("i ham rede", () => { HandleImReadyVoiceInput(); });
        keywords.Add("ham rede", () => { HandleImReadyVoiceInput(); });
        keywords.Add("m rede", () => { HandleImReadyVoiceInput(); });
        keywords.Add("em rede", () => { HandleImReadyVoiceInput(); });
        keywords.Add("am rede", () => { HandleImReadyVoiceInput(); });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), wordConfidenceLevel);
    }

    private void OnEnable()
    {
        EventManager.StartListening(Global.GetSharedHintString(Global.Shared_Hints.TUT_GOAWAY), GoAwaySuccessVoiceInput);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.GetSharedHintString(Global.Shared_Hints.TUT_GOAWAY), GoAwaySuccessVoiceInput);
    }

    private void KeywordRecognizerOnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        DebugManager.Info("Words Recognized: '" + args.text + "' - isRunning: " + keywordRecognizer.IsRunning);
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction) && keywordRecognizer.IsRunning)
        {
            keywordAction.Invoke();
        }
    }


    private void HandleImReadyVoiceInput()
    {
        if (IsPlayerOnMeditationCircleScript.IsPlayerSittingOnMeditationCircle() && !isIAmReadyHandled)
        {
            isIAmReadyHandled = true;
            StartCoroutine(SuccessKeywordRecognizer());
            StartCoroutine(TriggerChangeScene());
            
            // for tutorials in level 1
            EventManager.TriggerEvent(Global.GetSharedHintString(Global.Shared_Hints.TUT_IAMREADY));
        }
    }

    private IEnumerator TriggerChangeScene()
    {
        yield return new WaitForSecondsRealtime(1f);
        EventManager.TriggerEvent(Global.Shared_Events.CHANGE_SCENE);
    }

    private void GoAwaySuccessVoiceInput()
    {
        StartCoroutine(SuccessKeywordRecognizer());
    }

    private void HandleGoAwayVoiceInput()
    {
        if (!isGoAwayHandled) {
            isGoAwayHandled = true;
            EventManager.TriggerEvent(Global.Shared_Events.GO_AWAY_INPUT);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActivated && voiceInputAction.GetState(SteamVR_Input_Sources.RightHand))
        {
            // TODO: make voice input ready sound
            StartCoroutine(StartKeywordRecognizer());
            DebugManager.Info("ON - keywordRecognizer");
        }
        if (voiceInputAction.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            // TODO: make voice input off
            StartCoroutine(StopKeywordRecognizer());
            DebugManager.Info("OFF - keywordRecognizer");
        }
    }
    private IEnumerator StartKeywordRecognizer()
    {
        isActivated = true;
        VoiceManagerPlayClip(turnOnClip);
        yield return new WaitForSecondsRealtime(.5f);
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private IEnumerator StopKeywordRecognizer()
    {
        // if successClip is playing, don't play turn off clip 
        if (!(voiceInputAudioSource.isPlaying && voiceInputAudioSource.clip.name == successClip.name)) {
            VoiceManagerPlayClip(turnOffClip);
        }
        yield return new WaitForSecondsRealtime(.5f);
        keywordRecognizer.OnPhraseRecognized -= KeywordRecognizerOnPhraseRecognized;
        keywordRecognizer.Stop();
        yield return new WaitForSecondsRealtime(.3f);
        isActivated = false;
    }
    private IEnumerator SuccessKeywordRecognizer()
    {
        yield return new WaitForSecondsRealtime(.3f);
        VoiceManagerPlayClip(successClip);
        isIAmReadyHandled = false;
        isGoAwayHandled = false;
    }

    private void VoiceManagerPlayClip(AudioClip newClip)
    {
        if (voiceInputAudioSource.isPlaying)
        {
            voiceInputAudioSource.Stop();
        }
        voiceInputAudioSource.clip = newClip;
        voiceInputAudioSource.Play();
        DebugManager.Info("Now Playing: " + newClip.name);
    }



    private void OnApplicationQuit()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.OnPhraseRecognized -= KeywordRecognizerOnPhraseRecognized;
            keywordRecognizer.Stop();
        }
    }
}
