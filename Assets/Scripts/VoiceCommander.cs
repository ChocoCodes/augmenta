using UnityEngine;
using Oculus.Voice;
using Meta.WitAi.Json;

public class VoiceCommander : MonoBehaviour
{
    public AppVoiceExperience appVoice;
    public RobotFighter robotFighter;

    [SerializeField] private bool autoListen = true;
    [SerializeField] private float restartDelaySeconds = 0.2f;
    [SerializeField] private string oneIntentName = "one";
    [SerializeField] private string twoIntentName = "two";
    [SerializeField] private string threeIntentName = "three";
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
        bool commandTriggered = false;
        
        if (intents != null && intents.Count > 0)
        {
            string intentName = intents[0]["name"].Value;
            float confidence = intents[0]["confidence"].AsFloat;

            if (confidence > minIntentConfidence)
            {
                Debug.Log("AI Heard Intent: " + intentName + " (Conf: " + confidence + ")");

                commandTriggered = TryTriggerCommand(intentName, "intent");
            }
        }

        if (!commandTriggered && !string.IsNullOrEmpty(transcription))
        {
            commandTriggered = TryTriggerCommand(transcription, "transcription fallback");
        }

        if (!commandTriggered)
        {
            Debug.Log("VoiceCommander: No command triggered from this response.");
        }

        nextListenTime = Time.time + restartDelaySeconds;
    }

    bool TryTriggerCommand(string commandText, string source)
    {
        if (robotFighter == null)
        {
            Debug.LogError("VoiceCommander: RobotFighter reference is missing, cannot trigger command.");
            return false;
        }

        string normalizedCommand = commandText.Trim().ToLowerInvariant();

        if (string.Equals(normalizedCommand, oneIntentName, System.StringComparison.OrdinalIgnoreCase)
            || normalizedCommand.Contains("one"))
        {
            Debug.Log("VoiceCommander: Triggering one via " + source + ".");
            robotFighter.PerformOne();
            return true;
        }

        if (string.Equals(normalizedCommand, twoIntentName, System.StringComparison.OrdinalIgnoreCase)
            || normalizedCommand.Contains("two"))
        {
            Debug.Log("VoiceCommander: Triggering two via " + source + ".");
            robotFighter.PerformTwo();
            return true;
        }

        if (string.Equals(normalizedCommand, threeIntentName, System.StringComparison.OrdinalIgnoreCase)
            || normalizedCommand.Contains("three"))
        {
            Debug.Log("VoiceCommander: Triggering three via " + source + ".");
            robotFighter.PerformThree();
            return true;
        }

        Debug.Log("VoiceCommander: Command not recognized: " + commandText);
        return false;
    }

    void OnDestroy()
    {
        if (appVoice == null) return;

        appVoice.VoiceEvents.OnResponse.RemoveListener(HandleVoiceResponse);
        appVoice.VoiceEvents.OnStoppedListening.RemoveListener(RestartListening);
    }
}