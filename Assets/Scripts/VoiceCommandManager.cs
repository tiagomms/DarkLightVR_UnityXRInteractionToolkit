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
    private static VoiceCommandManager _instance;
    public static VoiceCommandManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<VoiceCommandManager>();
            }

            return _instance;
        }
    }

    public SteamVR_Action_Boolean voiceInputAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("VoiceInput");
    public ConfidenceLevel wordConfidenceLevel = ConfidenceLevel.Low;

    // TODO: with events
    public AudioClip turnOnClip;
    public AudioClip turnOffClip;
    public AudioClip successClip;

    // private Queue<AudioClip> audioClipQueue = new Queue<AudioClip>();

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    private bool isActivated = false;
    private bool isGoAwayHandled = false;
    private bool isIAmReadyHandled = false;
    private AudioSource voiceInputAudioSource;

    private void Awake()
    {
        _instance = this;
        instance.voiceInputAudioSource = gameObject.GetComponent<AudioSource>();
    }
    private void Start()
    {
        // go away iterations
        instance.keywords.Add("go", () => { HandleGoAwayVoiceInput(); });
        instance.keywords.Add("go away", () => { HandleGoAwayVoiceInput(); });
        instance.keywords.Add("goaway", () => { HandleGoAwayVoiceInput(); });
        instance.keywords.Add("goway", () => { HandleGoAwayVoiceInput(); });
        instance.keywords.Add("go way", () => { HandleGoAwayVoiceInput(); });
        instance.keywords.Add("way", () => { HandleGoAwayVoiceInput(); });
        instance.keywords.Add("away", () => { HandleGoAwayVoiceInput(); });

        // i am ready iterations
        instance.keywords.Add("am", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("ham", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("yam", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i am", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("eyam ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("eyeam ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("iyam redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("iyam ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i yam ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("yam redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("yam ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("teady", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("teddy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("seaby", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("steady", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("seady", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("seby", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("reby", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("reaby", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("raby", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("rady", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i am ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i m ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("im ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i ham ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("ham ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("m ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("em ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("am ready", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i'm raedy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("im raedy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i'm redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("im redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i am redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i m redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i ham redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("ham redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("m redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("em redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("am redy", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i'm redi", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("im redi", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i am redi", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i m redi", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i ham redi", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("ham redi", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("m redi", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("em redi", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("am redi", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i'm rede", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("im rede", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i am rede", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i m rede", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("i ham rede", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("ham rede", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("m rede", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("em rede", () => { HandleImReadyVoiceInput(); });
        instance.keywords.Add("am rede", () => { HandleImReadyVoiceInput(); });

        instance.keywordRecognizer = new KeywordRecognizer(instance.keywords.Keys.ToArray(), instance.wordConfidenceLevel);
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
        DebugManager.Info("Words Recognized: '" + args.text + "' - isRunning: " + instance.keywordRecognizer.IsRunning);
        System.Action keywordAction;
        if (instance.keywords.TryGetValue(args.text, out keywordAction) && instance.keywordRecognizer.IsRunning)
        {
            keywordAction.Invoke();
        }
    }


    private void HandleImReadyVoiceInput()
    {
        if (IsPlayerOnMeditationCircleScript.IsPlayerSittingOnMeditationCircle() && !instance.isIAmReadyHandled)
        {
            instance.isIAmReadyHandled = true;
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

    public void GoAwaySuccessVoiceInput()
    {
        StartCoroutine(SuccessKeywordRecognizer());
    }

    private void HandleGoAwayVoiceInput()
    {
        if (!instance.isGoAwayHandled) {
            instance.isGoAwayHandled = true;
            EventManager.TriggerEvent(Global.Shared_Events.GO_AWAY_INPUT);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!instance.isActivated && instance.voiceInputAction.GetState(SteamVR_Input_Sources.RightHand))
        {
            // TODO: make voice input ready sound
            StartCoroutine(StartKeywordRecognizer());
            DebugManager.Info("ON - instance.keywordRecognizer");
        }
        if (instance.voiceInputAction.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            // TODO: make voice input off
            StartCoroutine(StopKeywordRecognizer());
            DebugManager.Info("OFF - instance.keywordRecognizer");
        }
    }
    private IEnumerator StartKeywordRecognizer()
    {
        instance.isActivated = true;
        VoiceManagerPlayClip(instance.turnOnClip);
        yield return new WaitForSecondsRealtime(.5f);
        instance.keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPhraseRecognized;
        instance.keywordRecognizer.Start();
    }

    private IEnumerator StopKeywordRecognizer()
    {
        // if instance.successClip is playing, don't play turn off clip 
        if (!(instance.voiceInputAudioSource.isPlaying && instance.voiceInputAudioSource.clip.name == instance.successClip.name)) {
            VoiceManagerPlayClip(instance.turnOffClip);
        }
        yield return new WaitForSecondsRealtime(.5f);
        instance.keywordRecognizer.OnPhraseRecognized -= KeywordRecognizerOnPhraseRecognized;
        instance.keywordRecognizer.Stop();
        yield return new WaitForSecondsRealtime(.3f);
        instance.isIAmReadyHandled = false;
        instance.isGoAwayHandled = false;
        instance.isActivated = false;
    }
    private IEnumerator SuccessKeywordRecognizer()
    {
        yield return new WaitForSecondsRealtime(.3f);
        VoiceManagerPlayClip(instance.successClip);
        instance.isIAmReadyHandled = false;
        instance.isGoAwayHandled = false;
    }

    private void VoiceManagerPlayClip(AudioClip newClip)
    {
        if (instance.voiceInputAudioSource.isPlaying)
        {
            instance.voiceInputAudioSource.Stop();
        }
        instance.voiceInputAudioSource.clip = newClip;
        instance.voiceInputAudioSource.Play();
        DebugManager.Info("Now Playing: " + newClip.name);
    }



    private void OnApplicationQuit()
    {
        if (instance.keywordRecognizer != null && instance.keywordRecognizer.IsRunning)
        {
            instance.keywordRecognizer.OnPhraseRecognized -= KeywordRecognizerOnPhraseRecognized;
            instance.keywordRecognizer.Stop();
        }
    }
}
