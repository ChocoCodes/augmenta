using UnityEngine;
using Oculus.Voice;
using Meta.WitAi.Json;

public class VoiceCommander : MonoBehaviour
{
    public AppVoiceExperience appVoice;
    public RobotFighter robotFighter;

    [SerializeField] private bool autoListen = true;
    [SerializeField] private float restartDelaySeconds = 0.2f;
    [SerializeField] private string jabIntentName = "Command_Jab";
    [SerializeField] private float minIntentConfidence = 0.4f;

    private float nextListenTime;

    void Awake()
    {
        if (appVoice == null) appVoice = FindFirstObjectByType<AppVoiceExperience>();

        if (appVoice == null)
        {
            Debug.LogError("VoiceCommander: AppVoiceExperience reference is missing.");
            return;
        }

        appVoice.VoiceEvents.OnResponse.AddListener(HandleVoiceResponse);

        appVoice.VoiceEvents.OnStoppedListening.AddListener(RestartListening);
    }

    void Start()
    {
        RestartListening();
    }

    void Update()
    {
        if (!autoListen || appVoice == null) return;

        if (!appVoice.Active && Time.time >= nextListenTime)
        {
            RestartListening();
        }
    }

    void RestartListening()
    {
        if (appVoice == null) return;

        if (!appVoice.Active)
        {
            Debug.Log("Listening for commands...");
            appVoice.Activate();
        }
    }

    void HandleVoiceResponse(WitResponseNode response)
    {
        string transcription = response["text"].Value;
        Debug.Log("AI Transcribed: " + transcription);

        var intents = response["intents"];
        bool jabTriggered = false;
        
        if (intents != null && intents.Count > 0)
        {
            string intentName = intents[0]["name"].Value;
            float confidence = intents[0]["confidence"].AsFloat;

            if (confidence > minIntentConfidence)
            {
                Debug.Log("AI Heard Intent: " + intentName + " (Conf: " + confidence + ")");

                if (string.Equals(intentName, jabIntentName, System.StringComparison.OrdinalIgnoreCase)
                    || intentName.ToLower().Contains("jab"))
                {
                    jabTriggered = TryTriggerJab("intent");
                }
            }
        }

        if (!jabTriggered && !string.IsNullOrEmpty(transcription)
            && transcription.ToLower().Contains("jab"))
        {
            jabTriggered = TryTriggerJab("transcription fallback");
        }

        if (!jabTriggered)
        {
            Debug.Log("VoiceCommander: Jab not triggered from this response.");
        }

        nextListenTime = Time.time + restartDelaySeconds;
    }

    bool TryTriggerJab(string source)
    {
        if (robotFighter == null)
        {
            Debug.LogError("VoiceCommander: RobotFighter reference is missing, cannot jab.");
            return false;
        }

        Debug.Log("VoiceCommander: Triggering jab via " + source + ".");
        robotFighter.PerformJab();
        return true;
    }

    void OnDestroy()
    {
        if (appVoice == null) return;

        appVoice.VoiceEvents.OnResponse.RemoveListener(HandleVoiceResponse);
        appVoice.VoiceEvents.OnStoppedListening.RemoveListener(RestartListening);
    }
}